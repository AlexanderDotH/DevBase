using System.Buffers;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using AngleSharp;
using AngleSharp.Dom;
using DevBase.Net.Constants;
using DevBase.Net.Metrics;
using DevBase.Net.Parsing;
using Newtonsoft.Json;

namespace DevBase.Net.Core;

public sealed class Response : IDisposable, IAsyncDisposable
{
    private readonly HttpResponseMessage _httpResponse;
    private readonly MemoryStream _contentStream;
    private bool _disposed;
    private byte[]? _cachedContent;

    public HttpStatusCode StatusCode => this._httpResponse.StatusCode;
    public bool IsSuccessStatusCode => this._httpResponse.IsSuccessStatusCode;
    public HttpResponseHeaders Headers => this._httpResponse.Headers;
    public HttpContentHeaders? ContentHeaders => this._httpResponse.Content?.Headers;
    public string? ContentType => this.ContentHeaders?.ContentType?.MediaType;
    public long? ContentLength => this.ContentHeaders?.ContentLength;
    public Version HttpVersion => this._httpResponse.Version;
    public string? ReasonPhrase => this._httpResponse.ReasonPhrase;
    public RequestMetrics Metrics { get; }
    public bool FromCache { get; init; }
    public Uri? RequestUri { get; init; }


    internal Response(HttpResponseMessage httpResponse, MemoryStream contentStream, RequestMetrics metrics)
    {
        this._httpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));
        this._contentStream = contentStream ?? throw new ArgumentNullException(nameof(contentStream));
        this.Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
    }

    public async Task<byte[]> GetBytesAsync(CancellationToken cancellationToken = default)
    {
        if (this._cachedContent != null)
            return this._cachedContent;

        this._contentStream.Position = 0;
        this._cachedContent = this._contentStream.ToArray();
        return this._cachedContent;
    }

    public async Task<string> GetStringAsync(Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        byte[] bytes = await this.GetBytesAsync(cancellationToken);
        encoding ??= this.DetectEncoding() ?? Encoding.UTF8;
        return encoding.GetString(bytes);
    }

    public Stream GetStream()
    {
        this._contentStream.Position = 0;
        return this._contentStream;
    }

    private Encoding? DetectEncoding()
    {
        string? charset = this.ContentHeaders?.ContentType?.CharSet;
        if (string.IsNullOrEmpty(charset))
            return null;

        try
        {
            return Encoding.GetEncoding(charset);
        }
        catch
        {
            return null;
        }
    }

    public async Task<T> GetAsync<T>(CancellationToken cancellationToken = default)
    {
        Type targetType = typeof(T);

        if (targetType == typeof(string))
            return (T)(object)await this.GetStringAsync(cancellationToken: cancellationToken);

        if (targetType == typeof(byte[]))
            return (T)(object)await this.GetBytesAsync(cancellationToken);

        if (targetType == typeof(Stream) || targetType == typeof(MemoryStream))
            return (T)(object)this.GetStream();

        if (targetType == typeof(XDocument))
            return (T)(object)await this.ParseXmlAsync(cancellationToken);

        if (targetType == typeof(IDocument))
            return (T)(object)await this.ParseHtmlAsync(cancellationToken);

        if (targetType == typeof(JsonDocument))
            return (T)(object)await this.ParseJsonDocumentAsync(cancellationToken);

        return await this.ParseJsonAsync<T>(true, cancellationToken);
    }

    public async Task<T> ParseJsonAsync<T>(bool useSystemTextJson = true, CancellationToken cancellationToken = default)
    {
        string content = await this.GetStringAsync(cancellationToken: cancellationToken);

        if (useSystemTextJson)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }

        return JsonConvert.DeserializeObject<T>(content)!;
    }

    public async Task<JsonDocument> ParseJsonDocumentAsync(CancellationToken cancellationToken = default)
    {
        this._contentStream.Position = 0;
        return await JsonDocument.ParseAsync(this._contentStream, cancellationToken: cancellationToken);
    }

    public async Task<XDocument> ParseXmlAsync(CancellationToken cancellationToken = default)
    {
        this._contentStream.Position = 0;
        return await XDocument.LoadAsync(this._contentStream, LoadOptions.None, cancellationToken);
    }

    public async Task<IDocument> ParseHtmlAsync(CancellationToken cancellationToken = default)
    {
        string content = await this.GetStringAsync(cancellationToken: cancellationToken);
        IConfiguration config = AngleSharp.Configuration.Default;
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(req => req.Content(content), cancellationToken);
    }

    public async Task<T> ParseJsonPathAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        byte[] bytes = await this.GetBytesAsync(cancellationToken);
        JsonPathParser parser = new JsonPathParser();
        return parser.Parse<T>(bytes, path);
    }

    public async Task<List<T>> ParseJsonPathListAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        byte[] bytes = await this.GetBytesAsync(cancellationToken);
        
        // Use fast path for primitive types (string, int, long, double, decimal, bool)
        StreamingJsonPathParser fastParser = new StreamingJsonPathParser();
        
        if (typeof(T) == typeof(string))
            return (List<T>)(object)fastParser.ParseAllFast<string>(bytes, path);
        if (typeof(T) == typeof(int))
            return (List<T>)(object)fastParser.ParseAllFast<int>(bytes, path);
        if (typeof(T) == typeof(long))
            return (List<T>)(object)fastParser.ParseAllFast<long>(bytes, path);
        if (typeof(T) == typeof(double))
            return (List<T>)(object)fastParser.ParseAllFast<double>(bytes, path);
        if (typeof(T) == typeof(decimal))
            return (List<T>)(object)fastParser.ParseAllFast<decimal>(bytes, path);
        if (typeof(T) == typeof(bool))
            return (List<T>)(object)fastParser.ParseAllFast<bool>(bytes, path);
        
        JsonPathParser parser = new JsonPathParser();
        return parser.ParseList<T>(bytes, path);
    }

    public async IAsyncEnumerable<string> StreamLinesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        this._contentStream.Position = 0;
        using StreamReader reader = new StreamReader(this._contentStream, leaveOpen: true);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            string? line = await reader.ReadLineAsync(cancellationToken);
            if (line != null)
                yield return line;
        }
    }

    public async IAsyncEnumerable<byte[]> StreamChunksAsync(int chunkSize = 4096, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(chunkSize, 0);

        this._contentStream.Position = 0;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(chunkSize);

        try
        {
            int bytesRead;
            while ((bytesRead = await this._contentStream.ReadAsync(buffer.AsMemory(0, chunkSize), cancellationToken)) > 0)
            {
                byte[] chunk = new byte[bytesRead];
                Buffer.BlockCopy(buffer, 0, chunk, 0, bytesRead);
                yield return chunk;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public string? GetHeader(string name)
    {
        if (this.Headers.TryGetValues(name, out IEnumerable<string>? values))
            return string.Join(", ", values);

        if (this.ContentHeaders?.TryGetValues(name, out values) == true)
            return string.Join(", ", values);

        return null;
    }

    public IEnumerable<string> GetHeaderValues(string name)
    {
        if (this.Headers.TryGetValues(name, out IEnumerable<string>? values))
            return values;

        if (this.ContentHeaders?.TryGetValues(name, out values) == true)
            return values;

        return [];
    }

    public CookieCollection GetCookies()
    {
        CookieCollection cookies = new CookieCollection();

        if (!this.Headers.TryGetValues(HeaderConstants.SetCookie.ToString(), out IEnumerable<string>? cookieHeaders))
            return cookies;

        foreach (string header in cookieHeaders)
        {
            try
            {
                string[] parts = header.Split(';')[0].Split('=', 2);
                if (parts.Length == 2)
                {
                    cookies.Add(new Cookie(parts[0].Trim(), parts[1].Trim()));
                }
            }
            catch
            {
                // Ignore malformed cookies
            }
        }

        return cookies;
    }

    public bool IsRedirect => this.StatusCode is HttpStatusCode.MovedPermanently 
        or HttpStatusCode.Found 
        or HttpStatusCode.SeeOther 
        or HttpStatusCode.TemporaryRedirect 
        or HttpStatusCode.PermanentRedirect;

    public bool IsClientError => (int)this.StatusCode >= 400 && (int)this.StatusCode < 500;
    public bool IsServerError => (int)this.StatusCode >= 500;
    public bool IsRateLimited => this.StatusCode == HttpStatusCode.TooManyRequests;

    public void EnsureSuccessStatusCode()
    {
        if (!this.IsSuccessStatusCode)
            throw new HttpRequestException($"Response status code does not indicate success: {(int)this.StatusCode} ({this.ReasonPhrase})");
    }

    public void Dispose()
    {
        if (this._disposed) return;
        this._disposed = true;

        this._contentStream.Dispose();
        this._httpResponse.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (this._disposed) return;
        this._disposed = true;

        await this._contentStream.DisposeAsync();
        this._httpResponse.Dispose();
    }
}
