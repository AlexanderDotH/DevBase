using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AngleSharp.Dom;
using DevBase.Requests.Render;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DevBase.Requests.Core;

public class BaseResponse : IDisposable, IAsyncDisposable
{
    private readonly HttpResponseMessage _response;
    private readonly MemoryStream _contentStream;
    private bool _disposed;
    private byte[]? _cachedBuffer;
    private string? _cachedContent;
    private IDocument? _cachedDocument;

    public HttpStatusCode StatusCode => _response.StatusCode;
    public bool IsSuccessStatusCode => _response.IsSuccessStatusCode;
    public HttpResponseHeaders Headers => _response.Headers;
    public HttpContentHeaders? ContentHeaders => _response.Content?.Headers;
    public string? ContentType => ContentHeaders?.ContentType?.MediaType;
    public long? ContentLength => ContentHeaders?.ContentLength;
    public Version HttpVersion => _response.Version;
    public string? ReasonPhrase => _response.ReasonPhrase;

    protected BaseResponse(HttpResponseMessage response, MemoryStream contentStream)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
        _contentStream = contentStream ?? throw new ArgumentNullException(nameof(contentStream));
    }

    public async Task<byte[]> GetBufferAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedBuffer != null)
            return _cachedBuffer;

        _contentStream.Position = 0;
        _cachedBuffer = _contentStream.ToArray();
        return _cachedBuffer;
    }

    public async Task<string> GetContentAsync(Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        if (_cachedContent != null)
            return _cachedContent;

        byte[] bytes = await GetBufferAsync(cancellationToken);
        encoding ??= DetectEncoding() ?? Encoding.UTF8;
        _cachedContent = encoding.GetString(bytes);
        return _cachedContent;
    }

    public async Task<IDocument> GetRenderedHtmlAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedDocument != null)
            return _cachedDocument;

        string content = await GetContentAsync(cancellationToken: cancellationToken);
        _cachedDocument = await HtmlRenderer.ParseAsync(content, cancellationToken);
        return _cachedDocument;
    }

    public Stream GetStream()
    {
        _contentStream.Position = 0;
        return _contentStream;
    }

    public CookieCollection GetCookies()
    {
        CookieCollection cookies = new CookieCollection();

        if (!Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? cookieHeaders))
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

    private Encoding? DetectEncoding()
    {
        string? charset = ContentHeaders?.ContentType?.CharSet;
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

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _contentStream.Dispose();
        _response.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        await _contentStream.DisposeAsync();
        _response.Dispose();
        GC.SuppressFinalize(this);
    }
}