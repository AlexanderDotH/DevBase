using System.Buffers;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using AngleSharp;
using AngleSharp.Dom;
using DevBase.Requests.Metrics;
using DevBase.Requests.Parsing;
using Newtonsoft.Json;

namespace DevBase.Requests;

public sealed class Response : IDisposable, IAsyncDisposable
{
    private readonly HttpResponseMessage _httpResponse;
    private readonly MemoryStream _contentStream;
    private bool _disposed;
    private byte[]? _cachedContent;

    public HttpStatusCode StatusCode => _httpResponse.StatusCode;
    public bool IsSuccessStatusCode => _httpResponse.IsSuccessStatusCode;
    public HttpResponseHeaders Headers => _httpResponse.Headers;
    public HttpContentHeaders? ContentHeaders => _httpResponse.Content?.Headers;
    public string? ContentType => ContentHeaders?.ContentType?.MediaType;
    public long? ContentLength => ContentHeaders?.ContentLength;
    public Version HttpVersion => _httpResponse.Version;
    public string? ReasonPhrase => _httpResponse.ReasonPhrase;
    public RequestMetrics Metrics { get; }
    public bool FromCache { get; init; }
    public Uri? RequestUri { get; init; }

    internal Response(HttpResponseMessage httpResponse, MemoryStream contentStream, RequestMetrics metrics)
    {
        _httpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));
        _contentStream = contentStream ?? throw new ArgumentNullException(nameof(contentStream));
        Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
    }

    #region Content Access

    public async Task<byte[]> GetBytesAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedContent != null)
            return _cachedContent;

        _contentStream.Position = 0;
        _cachedContent = _contentStream.ToArray();
        return _cachedContent;
    }

    public async Task<string> GetStringAsync(Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        var bytes = await GetBytesAsync(cancellationToken);
        encoding ??= DetectEncoding() ?? Encoding.UTF8;
        return encoding.GetString(bytes);
    }

    public Stream GetStream()
    {
        _contentStream.Position = 0;
        return _contentStream;
    }

    private Encoding? DetectEncoding()
    {
        var charset = ContentHeaders?.ContentType?.CharSet;
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

    #endregion

    #region Generic Parsing

    public async Task<T> GetAsync<T>(CancellationToken cancellationToken = default)
    {
        var targetType = typeof(T);

        if (targetType == typeof(string))
            return (T)(object)await GetStringAsync(cancellationToken: cancellationToken);

        if (targetType == typeof(byte[]))
            return (T)(object)await GetBytesAsync(cancellationToken);

        if (targetType == typeof(Stream) || targetType == typeof(MemoryStream))
            return (T)(object)GetStream();

        if (targetType == typeof(XDocument))
            return (T)(object)await ParseXmlAsync(cancellationToken);

        if (targetType == typeof(IDocument))
            return (T)(object)await ParseHtmlAsync(cancellationToken);

        if (targetType == typeof(JsonDocument))
            return (T)(object)await ParseJsonDocumentAsync(cancellationToken);

        return await ParseJsonAsync<T>(true, cancellationToken);
    }

    public async Task<T> ParseJsonAsync<T>(bool useSystemTextJson = true, CancellationToken cancellationToken = default)
    {
        var content = await GetStringAsync(cancellationToken: cancellationToken);

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
        _contentStream.Position = 0;
        return await JsonDocument.ParseAsync(_contentStream, cancellationToken: cancellationToken);
    }

    public async Task<XDocument> ParseXmlAsync(CancellationToken cancellationToken = default)
    {
        _contentStream.Position = 0;
        return await XDocument.LoadAsync(_contentStream, LoadOptions.None, cancellationToken);
    }

    public async Task<IDocument> ParseHtmlAsync(CancellationToken cancellationToken = default)
    {
        var content = await GetStringAsync(cancellationToken: cancellationToken);
        var config = AngleSharp.Configuration.Default;
        var context = BrowsingContext.New(config);
        return await context.OpenAsync(req => req.Content(content), cancellationToken);
    }

    #endregion

    #region JSON Path Parsing

    public async Task<T> ParseJsonPathAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        var bytes = await GetBytesAsync(cancellationToken);
        var parser = new JsonPathParser();
        return parser.Parse<T>(bytes, path);
    }

    public async Task<List<T>> ParseJsonPathListAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        var bytes = await GetBytesAsync(cancellationToken);
        var parser = new JsonPathParser();
        return parser.ParseList<T>(bytes, path);
    }

    #endregion

    #region Streaming

    public async IAsyncEnumerable<string> StreamLinesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _contentStream.Position = 0;
        using var reader = new StreamReader(_contentStream, leaveOpen: true);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line != null)
                yield return line;
        }
    }

    public async IAsyncEnumerable<byte[]> StreamChunksAsync(int chunkSize = 4096, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(chunkSize, 0);

        _contentStream.Position = 0;
        var buffer = ArrayPool<byte>.Shared.Rent(chunkSize);

        try
        {
            int bytesRead;
            while ((bytesRead = await _contentStream.ReadAsync(buffer.AsMemory(0, chunkSize), cancellationToken)) > 0)
            {
                var chunk = new byte[bytesRead];
                Buffer.BlockCopy(buffer, 0, chunk, 0, bytesRead);
                yield return chunk;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    #endregion

    #region Header Access

    public string? GetHeader(string name)
    {
        if (Headers.TryGetValues(name, out var values))
            return string.Join(", ", values);

        if (ContentHeaders?.TryGetValues(name, out values) == true)
            return string.Join(", ", values);

        return null;
    }

    public IEnumerable<string> GetHeaderValues(string name)
    {
        if (Headers.TryGetValues(name, out var values))
            return values;

        if (ContentHeaders?.TryGetValues(name, out values) == true)
            return values;

        return [];
    }

    public CookieCollection GetCookies()
    {
        var cookies = new CookieCollection();

        if (!Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
            return cookies;

        foreach (var header in cookieHeaders)
        {
            try
            {
                var parts = header.Split(';')[0].Split('=', 2);
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

    #endregion

    #region Status Helpers

    public bool IsRedirect => StatusCode is HttpStatusCode.MovedPermanently 
        or HttpStatusCode.Found 
        or HttpStatusCode.SeeOther 
        or HttpStatusCode.TemporaryRedirect 
        or HttpStatusCode.PermanentRedirect;

    public bool IsClientError => (int)StatusCode >= 400 && (int)StatusCode < 500;
    public bool IsServerError => (int)StatusCode >= 500;
    public bool IsRateLimited => StatusCode == HttpStatusCode.TooManyRequests;

    public void EnsureSuccessStatusCode()
    {
        if (!IsSuccessStatusCode)
            throw new HttpRequestException($"Response status code does not indicate success: {(int)StatusCode} ({ReasonPhrase})");
    }

    #endregion

    #region Disposal

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _contentStream.Dispose();
        _httpResponse.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        await _contentStream.DisposeAsync();
        _httpResponse.Dispose();
    }

    #endregion
}
