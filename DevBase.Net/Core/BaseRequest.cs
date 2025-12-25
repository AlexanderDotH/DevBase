using System.Text;
using DevBase.Net.Configuration;
using DevBase.Net.Data.Body;
using DevBase.Net.Data.Header;
using DevBase.Net.Interfaces;
using DevBase.Net.Proxy;

namespace DevBase.Net.Core;

/// <summary>
/// Abstract base class for HTTP requests providing core request properties and lifecycle management.
/// </summary>
public abstract class BaseRequest : IDisposable, IAsyncDisposable
{
    protected HttpMethod _method = HttpMethod.Get;
    protected TimeSpan _timeout = TimeSpan.FromSeconds(30);
    protected CancellationToken _cancellationToken = CancellationToken.None;
    protected TrackedProxyInfo? _proxy;
    protected RetryPolicy _retryPolicy = RetryPolicy.None;
    protected bool _validateCertificates = true;
    protected bool _followRedirects = true;
    protected int _maxRedirects = 50;
    protected bool _isBuilt;
    protected bool _disposed;
    
    protected readonly List<IRequestInterceptor> _requestInterceptors = new List<IRequestInterceptor>();
    protected readonly List<IResponseInterceptor> _responseInterceptors = new List<IResponseInterceptor>();

    /// <summary>
    /// Gets the HTTP method for this request.
    /// </summary>
    public HttpMethod Method => this._method;
    
    /// <summary>
    /// Gets the timeout duration for this request.
    /// </summary>
    public TimeSpan Timeout => this._timeout;
    
    /// <summary>
    /// Gets the cancellation token for this request.
    /// </summary>
    public CancellationToken CancellationToken => this._cancellationToken;
    
    /// <summary>
    /// Gets the proxy configuration for this request.
    /// </summary>
    public TrackedProxyInfo? Proxy => this._proxy;
    
    /// <summary>
    /// Gets the retry policy for this request.
    /// </summary>
    public RetryPolicy RetryPolicy => this._retryPolicy;
    
    /// <summary>
    /// Gets whether certificate validation is enabled.
    /// </summary>
    public bool ValidateCertificates => this._validateCertificates;
    
    /// <summary>
    /// Gets whether redirects are followed automatically.
    /// </summary>
    public bool FollowRedirects => this._followRedirects;
    
    /// <summary>
    /// Gets the maximum number of redirects to follow.
    /// </summary>
    public int MaxRedirects => this._maxRedirects;
    
    /// <summary>
    /// Gets whether the request has been built.
    /// </summary>
    public bool IsBuilt => this._isBuilt;
    
    /// <summary>
    /// Gets the list of request interceptors.
    /// </summary>
    public IReadOnlyList<IRequestInterceptor> RequestInterceptors => this._requestInterceptors;
    
    /// <summary>
    /// Gets the list of response interceptors.
    /// </summary>
    public IReadOnlyList<IResponseInterceptor> ResponseInterceptors => this._responseInterceptors;

    /// <summary>
    /// Gets the request URI.
    /// </summary>
    public abstract ReadOnlySpan<char> Uri { get; }
    
    /// <summary>
    /// Gets the request body as bytes.
    /// </summary>
    public abstract ReadOnlySpan<byte> Body { get; }

    /// <summary>
    /// Builds the request, finalizing all configuration.
    /// </summary>
    public abstract BaseRequest Build();
    
    /// <summary>
    /// Sends the request asynchronously.
    /// </summary>
    public abstract Task<Response> SendAsync(CancellationToken cancellationToken = default);

    public virtual void Dispose()
    {
        if (this._disposed) return;
        this._disposed = true;
        
        this._requestInterceptors.Clear();
        this._responseInterceptors.Clear();
        GC.SuppressFinalize(this);
    }

    public virtual ValueTask DisposeAsync()
    {
        this.Dispose();
        return ValueTask.CompletedTask;
    }
}