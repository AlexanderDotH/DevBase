using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AngleSharp.Dom;
using DevBase.Net.Render;

namespace DevBase.Net.Core;

/// <summary>
/// Abstract base class for HTTP responses providing core response properties and content access.
/// </summary>
public abstract class BaseResponse : IDisposable, IAsyncDisposable
{
    protected readonly HttpResponseMessage _httpResponse;
    protected readonly MemoryStream _contentStream;
    protected bool _disposed;
    protected byte[]? _cachedContent;

    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode => this._httpResponse.StatusCode;
    
    /// <summary>
    /// Gets whether the response indicates success.
    /// </summary>
    public bool IsSuccessStatusCode => this._httpResponse.IsSuccessStatusCode;
    
    /// <summary>
    /// Gets the response headers.
    /// </summary>
    public HttpResponseHeaders Headers => this._httpResponse.Headers;
    
    /// <summary>
    /// Gets the content headers.
    /// </summary>
    public HttpContentHeaders? ContentHeaders => this._httpResponse.Content?.Headers;
    
    /// <summary>
    /// Gets the content type.
    /// </summary>
    public string? ContentType => this.ContentHeaders?.ContentType?.MediaType;
    
    /// <summary>
    /// Gets the content length.
    /// </summary>
    public long? ContentLength => this.ContentHeaders?.ContentLength;
    
    /// <summary>
    /// Gets the HTTP version.
    /// </summary>
    public Version HttpVersion => this._httpResponse.Version;
    
    /// <summary>
    /// Gets the reason phrase.
    /// </summary>
    public string? ReasonPhrase => this._httpResponse.ReasonPhrase;

    /// <summary>
    /// Gets whether this is a redirect response.
    /// </summary>
    public bool IsRedirect => this.StatusCode is HttpStatusCode.MovedPermanently 
        or HttpStatusCode.Found 
        or HttpStatusCode.SeeOther 
        or HttpStatusCode.TemporaryRedirect 
        or HttpStatusCode.PermanentRedirect;

    /// <summary>
    /// Gets whether this is a client error (4xx).
    /// </summary>
    public bool IsClientError => (int)this.StatusCode >= 400 && (int)this.StatusCode < 500;
    
    /// <summary>
    /// Gets whether this is a server error (5xx).
    /// </summary>
    public bool IsServerError => (int)this.StatusCode >= 500;
    
    /// <summary>
    /// Gets whether this response indicates rate limiting.
    /// </summary>
    public bool IsRateLimited => this.StatusCode == HttpStatusCode.TooManyRequests;

    protected BaseResponse(HttpResponseMessage httpResponse, MemoryStream contentStream)
    {
        this._httpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));
        this._contentStream = contentStream ?? throw new ArgumentNullException(nameof(contentStream));
    }

    /// <summary>
    /// Gets the response content as a byte array.
    /// </summary>
    public virtual async Task<byte[]> GetBytesAsync(CancellationToken cancellationToken = default)
    {
        if (this._cachedContent != null)
            return this._cachedContent;

        this._contentStream.Position = 0;
        this._cachedContent = this._contentStream.ToArray();
        return this._cachedContent;
    }

    /// <summary>
    /// Gets the response content as a string.
    /// </summary>
    public virtual async Task<string> GetStringAsync(Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        byte[] bytes = await this.GetBytesAsync(cancellationToken);
        encoding ??= this.DetectEncoding() ?? Encoding.UTF8;
        return encoding.GetString(bytes);
    }

    /// <summary>
    /// Gets the response content stream.
    /// </summary>
    public virtual Stream GetStream()
    {
        this._contentStream.Position = 0;
        return this._contentStream;
    }

    /// <summary>
    /// Gets cookies from the response headers.
    /// </summary>
    public virtual CookieCollection GetCookies()
    {
        CookieCollection cookies = new CookieCollection();

        if (!this.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? cookieHeaders))
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

    /// <summary>
    /// Detects the encoding from content headers.
    /// </summary>
    protected Encoding? DetectEncoding()
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

    /// <summary>
    /// Gets a header value by name.
    /// </summary>
    public virtual string? GetHeader(string name)
    {
        if (this.Headers.TryGetValues(name, out IEnumerable<string>? values))
            return string.Join(", ", values);

        if (this.ContentHeaders?.TryGetValues(name, out values) == true)
            return string.Join(", ", values);

        return null;
    }

    /// <summary>
    /// Throws if the response does not indicate success.
    /// </summary>
    public virtual void EnsureSuccessStatusCode()
    {
        if (!this.IsSuccessStatusCode)
            throw new HttpRequestException($"Response status code does not indicate success: {(int)this.StatusCode} ({this.ReasonPhrase})");
    }

    public virtual void Dispose()
    {
        if (this._disposed)
            return;

        this._disposed = true;
        this._contentStream.Dispose();
        this._httpResponse.Dispose();
        GC.SuppressFinalize(this);
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (this._disposed)
            return;

        this._disposed = true;
        await this._contentStream.DisposeAsync();
        this._httpResponse.Dispose();
        GC.SuppressFinalize(this);
    }
}