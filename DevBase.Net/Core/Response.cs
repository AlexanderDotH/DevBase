using System.Buffers;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using AngleSharp;
using AngleSharp.Dom;
using DevBase.Net.Configuration;
using DevBase.Net.Constants;
using DevBase.Net.Metrics;
using DevBase.Net.Parsing;
using DevBase.Net.Security.Token;
using DevBase.Net.Validation;
using Newtonsoft.Json;

namespace DevBase.Net.Core;

/// <summary>
/// HTTP response class that extends BaseResponse with parsing and streaming capabilities.
/// </summary>
public sealed class Response : BaseResponse
{
    /// <summary>
    /// Gets the request metrics for this response.
    /// </summary>
    public RequestMetrics Metrics { get; }
    
    /// <summary>
    /// Gets whether this response was served from cache.
    /// </summary>
    public bool FromCache { get; init; }
    
    /// <summary>
    /// Gets the original request URI.
    /// </summary>
    public Uri? RequestUri { get; init; }

    internal Response(HttpResponseMessage httpResponse, MemoryStream contentStream, RequestMetrics metrics)
        : base(httpResponse, contentStream)
    {
        this.Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
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

    public async Task<MultiSelectorResult> ParseMultipleJsonPathsAsync(
        MultiSelectorConfig config,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = await this.GetBytesAsync(cancellationToken);
        MultiSelectorParser parser = new MultiSelectorParser();
        return parser.Parse(bytes, config);
    }
    
    public async Task<MultiSelectorResult> ParseMultipleJsonPathsAsync(
        CancellationToken cancellationToken = default,
        params (string name, string path)[] selectors)
    {
        byte[] bytes = await this.GetBytesAsync(cancellationToken);
        MultiSelectorParser parser = new MultiSelectorParser();
        return parser.Parse(bytes, selectors);
    }
    
    public async Task<MultiSelectorResult> ParseMultipleJsonPathsOptimizedAsync(
        CancellationToken cancellationToken = default,
        params (string name, string path)[] selectors)
    {
        byte[] bytes = await this.GetBytesAsync(cancellationToken);
        MultiSelectorParser parser = new MultiSelectorParser();
        return parser.ParseOptimized(bytes, selectors);
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

    public IEnumerable<string> GetHeaderValues(string name)
    {
        if (this.Headers.TryGetValues(name, out IEnumerable<string>? values))
            return values;

        if (this.ContentHeaders?.TryGetValues(name, out values) == true)
            return values;

        return [];
    }

    public AuthenticationToken? ParseBearerToken()
    {
        string? authHeader = this.GetHeader("Authorization");
        if (string.IsNullOrWhiteSpace(authHeader))
            return null;
        
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;
        
        string token = authHeader.Substring(7);
        return HeaderValidator.ParseJwtToken(token);
    }

    public AuthenticationToken? ParseAndVerifyBearerToken(string secret)
    {
        string? authHeader = this.GetHeader("Authorization");
        if (string.IsNullOrWhiteSpace(authHeader))
            return null;
        
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;
        
        string token = authHeader.Substring(7);
        return HeaderValidator.ParseAndVerifyJwtToken(token, secret);
    }

    public ValidationResult ValidateContentLength()
    {
        string? contentLengthHeader = this.ContentLength?.ToString();
        long actualLength = this._contentStream.Length;
        return HeaderValidator.ValidateContentLength(contentLengthHeader, actualLength);
    }

}
