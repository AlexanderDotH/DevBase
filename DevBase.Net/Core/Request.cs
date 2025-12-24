using DevBase.Net.Configuration;
using DevBase.Net.Data;
using DevBase.Net.Data.Body;
using DevBase.Net.Data.Body.Mime;
using DevBase.Net.Data.Header;
using DevBase.Net.Interfaces;
using DevBase.Net.Proxy;

namespace DevBase.Net.Core;

public partial class Request : IDisposable, IAsyncDisposable
{
    private static readonly Dictionary<string, HttpClient> ClientPool = new();
    private static readonly object PoolLock = new();
    private static readonly MimeDictionary SharedMimeDictionary = new();
    private static TimeSpan PooledConnectionLifetime = TimeSpan.FromMinutes(5);
    private static TimeSpan PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2);
    private static int MaxConnectionsPerServer = 10;

    private readonly RequestBuilder _requestBuilder;

    private HttpMethod _method = HttpMethod.Get;
    private TimeSpan _timeout = TimeSpan.FromSeconds(30);
    private CancellationToken _cancellationToken = CancellationToken.None;
    private TrackedProxyInfo? _proxy;
    private RetryPolicy _retryPolicy = RetryPolicy.None;
    private ScrapingBypassConfig? _scrapingBypass;
    private JsonPathConfig? _jsonPathConfig;
    private HostCheckConfig? _hostCheckConfig;
    private LoggingConfig? _loggingConfig;
    private bool _validateCertificates = true;
    private bool _validateHeaders = true;
    private bool _followRedirects = true;
    private int _maxRedirects = 50;
    private readonly List<IRequestInterceptor> _requestInterceptors = [];
    private readonly List<IResponseInterceptor> _responseInterceptors = [];
    private RequestKeyValueListBodyBuilder? _formBuilder;
    private bool _isBuilt;
    private bool _disposed;

    public ReadOnlySpan<char> Uri => this._requestBuilder.Uri;
    public ReadOnlySpan<byte> Body => this._requestBuilder.Body;
    public Uri? GetUri() => this._requestBuilder.Uri.IsEmpty ? null : new Uri(this._requestBuilder.Uri.ToString());
    public HttpMethod Method => this._method;
    public TimeSpan Timeout => this._timeout;
    public CancellationToken CancellationToken => this._cancellationToken;
    public TrackedProxyInfo? Proxy => this._proxy;
    public RetryPolicy RetryPolicy => this._retryPolicy;
    public ScrapingBypassConfig? ScrapingBypass => this._scrapingBypass;
    public JsonPathConfig? JsonPathConfig => this._jsonPathConfig;
    public HostCheckConfig? HostCheckConfig => this._hostCheckConfig;
    public LoggingConfig? LoggingConfig => this._loggingConfig;
    public bool ValidateCertificates => this._validateCertificates;
    public bool HeaderValidationEnabled => this._validateHeaders;
    public bool FollowRedirects => this._followRedirects;
    public int MaxRedirects => this._maxRedirects;
    public IReadOnlyList<IRequestInterceptor> RequestInterceptors => this._requestInterceptors;
    public IReadOnlyList<IResponseInterceptor> ResponseInterceptors => this._responseInterceptors;
    public RequestHeaderBuilder? HeaderBuilder => this._requestBuilder.RequestHeaderBuilder;

    public Request()
    {
        this._requestBuilder = new RequestBuilder();
    }

    public Request(ReadOnlyMemory<char> url) : this()
    {
        this._requestBuilder = new RequestBuilder(url);
    }

    public Request(string url) : this(url.AsMemory()) { }

    public Request(Uri uri) : this()
    {
        this._requestBuilder = new RequestBuilder(uri);
    }

    public Request(string url, HttpMethod method) : this(url)
    {
        this._method = method;
    }

    public Request(Uri uri, HttpMethod method) : this(uri)
    {
        this._method = method;
    }

    public void Dispose()
    {
        if (this._disposed) return;
        this._disposed = true;
        
        this._requestInterceptors.Clear();
        this._responseInterceptors.Clear();
    }

    public ValueTask DisposeAsync()
    {
        this.Dispose();
        return ValueTask.CompletedTask;
    }
}