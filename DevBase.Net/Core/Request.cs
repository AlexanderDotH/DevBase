using DevBase.Net.Configuration;
using DevBase.Net.Data;
using DevBase.Net.Data.Body;
using DevBase.Net.Data.Body.Mime;
using DevBase.Net.Data.Header;
using DevBase.Net.Interfaces;
using DevBase.Net.Proxy;

namespace DevBase.Net.Core;

/// <summary>
/// HTTP request class that extends BaseRequest with full request building and execution capabilities.
/// Split across partial classes: Request.cs (core), RequestConfiguration.cs (fluent API), 
/// RequestHttp.cs (HTTP execution), RequestContent.cs (content handling), RequestBuilder.cs (file uploads).
/// </summary>
public partial class Request : BaseRequest
{
    private static readonly Dictionary<string, HttpClient> ClientPool = new();
    private static readonly object PoolLock = new();
    private static readonly MimeDictionary SharedMimeDictionary = new();
    private static TimeSpan PooledConnectionLifetime = TimeSpan.FromMinutes(5);
    private static TimeSpan PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2);
    private static int MaxConnectionsPerServer = 10;

    private readonly RequestBuilder _requestBuilder;
    
    // Request-specific configuration not in BaseRequest
    private ScrapingBypassConfig? _scrapingBypass;
    private JsonPathConfig? _jsonPathConfig;
    private HostCheckConfig? _hostCheckConfig;
    private LoggingConfig? _loggingConfig;
    private bool _validateHeaders = true;
    private RequestKeyValueListBodyBuilder? _formBuilder;
    private readonly List<IRequestInterceptor> _requestInterceptors = [];
    private readonly List<IResponseInterceptor> _responseInterceptors = [];
    private bool _disposed;

    /// <summary>
    /// Gets the request URI.
    /// </summary>
    public override ReadOnlySpan<char> Uri => this._requestBuilder.Uri;
    
    /// <summary>
    /// Gets the request body as bytes.
    /// </summary>
    public override ReadOnlySpan<byte> Body => this._requestBuilder.Body;
    
    /// <summary>
    /// Gets the request URI as a Uri object.
    /// </summary>
    public Uri? GetUri() => this._requestBuilder.Uri.IsEmpty ? null : new Uri(this._requestBuilder.Uri.ToString());
    
    /// <summary>
    /// Gets the scraping bypass configuration.
    /// </summary>
    public ScrapingBypassConfig? ScrapingBypass => this._scrapingBypass;
    
    /// <summary>
    /// Gets the JSON path configuration.
    /// </summary>
    public JsonPathConfig? JsonPathConfig => this._jsonPathConfig;
    
    /// <summary>
    /// Gets the host check configuration.
    /// </summary>
    public HostCheckConfig? HostCheckConfig => this._hostCheckConfig;
    
    /// <summary>
    /// Gets the logging configuration.
    /// </summary>
    public LoggingConfig? LoggingConfig => this._loggingConfig;
    
    /// <summary>
    /// Gets whether header validation is enabled.
    /// </summary>
    public bool HeaderValidationEnabled => this._validateHeaders;
    
    /// <summary>
    /// Gets the header builder for this request.
    /// </summary>
    public RequestHeaderBuilder? HeaderBuilder => this._requestBuilder.RequestHeaderBuilder;

    /// <summary>
    /// Gets the request interceptors (new modifier to hide base implementation).
    /// </summary>
    public new IReadOnlyList<IRequestInterceptor> RequestInterceptors => this._requestInterceptors;
    
    /// <summary>
    /// Gets the response interceptors (new modifier to hide base implementation).
    /// </summary>
    public new IReadOnlyList<IResponseInterceptor> ResponseInterceptors => this._responseInterceptors;

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

    public override void Dispose()
    {
        if (this._disposed) return;
        base.Dispose();
    }

    public override ValueTask DisposeAsync()
    {
        this.Dispose();
        return ValueTask.CompletedTask;
    }
}