using System.Buffers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using DevBase.Requests.Configuration;
using DevBase.Requests.Data;
using DevBase.Requests.Data.Body;
using DevBase.Requests.Data.Body.Mime;
using DevBase.Requests.Data.Header;
using DevBase.Requests.Data.Header.Authentication;
using DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Requests.Data.Parameters;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Interfaces;
using DevBase.Requests.Metrics;
using DevBase.Requests.Proxy;
using DevBase.Requests.Render;

namespace DevBase.Requests;

public sealed class Request : IDisposable, IAsyncDisposable
{
    #region Static HttpClient Pool
    
    private static readonly Dictionary<string, HttpClient> ClientPool = new();
    private static readonly object PoolLock = new();
    private static TimeSpan PooledConnectionLifetime = TimeSpan.FromMinutes(5);
    private static TimeSpan PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2);
    private static int MaxConnectionsPerServer = 10;
    
    #endregion
    
    #region Fields
    
    private readonly RequestBuilder _requestBuilder;
    private readonly MimeDictionary _mimeDictionary;
    
    private HttpMethod _method = HttpMethod.Get;
    private TimeSpan _timeout = TimeSpan.FromSeconds(30);
    private CancellationToken _cancellationToken = CancellationToken.None;
    private TrackedProxyInfo? _proxy;
    private RetryPolicy _retryPolicy = RetryPolicy.Default;
    private ScrapingBypassConfig? _scrapingBypass;
    private JsonPathConfig? _jsonPathConfig;
    private HostCheckConfig? _hostCheckConfig;
    private LoggingConfig? _loggingConfig;
    private bool _validateCertificates = true;
    private bool _followRedirects = true;
    private int _maxRedirects = 50;
    private readonly List<IRequestInterceptor> _requestInterceptors = [];
    private readonly List<IResponseInterceptor> _responseInterceptors = [];
    private bool _isBuilt;
    private bool _disposed;
    
    #endregion

    #region Properties
    
    public ReadOnlySpan<char> Uri => _requestBuilder.Uri;
    public ReadOnlySpan<byte> Body => _requestBuilder.Body;
    public Uri? GetUri() => _requestBuilder.Uri.IsEmpty ? null : new Uri(_requestBuilder.Uri.ToString());
    public HttpMethod Method => _method;
    public TimeSpan Timeout => _timeout;
    public CancellationToken CancellationToken => _cancellationToken;
    public TrackedProxyInfo? Proxy => _proxy;
    public RetryPolicy RetryPolicy => _retryPolicy;
    public ScrapingBypassConfig? ScrapingBypass => _scrapingBypass;
    public JsonPathConfig? JsonPathConfig => _jsonPathConfig;
    public HostCheckConfig? HostCheckConfig => _hostCheckConfig;
    public LoggingConfig? LoggingConfig => _loggingConfig;
    public bool ValidateCertificates => _validateCertificates;
    public bool FollowRedirects => _followRedirects;
    public int MaxRedirects => _maxRedirects;
    public IReadOnlyList<IRequestInterceptor> RequestInterceptors => _requestInterceptors;
    public IReadOnlyList<IResponseInterceptor> ResponseInterceptors => _responseInterceptors;
    public RequestHeaderBuilder? HeaderBuilder => _requestBuilder.RequestHeaderBuilder;
    
    #endregion

    #region Constructors
    
    public Request()
    {
        _requestBuilder = new RequestBuilder();
        _mimeDictionary = new MimeDictionary();
    }

    public Request(ReadOnlyMemory<char> url) : this()
    {
        _requestBuilder = new RequestBuilder(url);
    }

    public Request(string url) : this(url.AsMemory()) { }

    public Request(Uri uri) : this()
    {
        _requestBuilder = new RequestBuilder(uri);
    }

    public Request(string url, HttpMethod method) : this(url)
    {
        _method = method;
    }

    public Request(Uri uri, HttpMethod method) : this(uri)
    {
        _method = method;
    }

    #endregion

    #region URL Builder

    public Request WithUrl(string url)
    {
        _requestBuilder.WithUrl(url);
        return this;
    }

    public Request WithUrl(Uri uri)
    {
        _requestBuilder.WithUrl(uri);
        return this;
    }

    public Request WithParameters(ParameterBuilder parameterBuilder)
    {
        _requestBuilder.WithParameters(parameterBuilder);
        return this;
    }

    public Request WithParameter(string key, string value)
    {
        var builder = new ParameterBuilder();
        builder.AddParameter(key, value);
        return WithParameters(builder);
    }

    public Request WithParameters(params (string key, string value)[] parameters)
    {
        var builder = new ParameterBuilder();
        builder.AddParameters(parameters);
        return WithParameters(builder);
    }

    #endregion

    #region Method Builder

    public Request WithMethod(HttpMethod method)
    {
        ArgumentNullException.ThrowIfNull(method);
        _method = method;
        return this;
    }

    public Request AsGet() => WithMethod(HttpMethod.Get);
    public Request AsPost() => WithMethod(HttpMethod.Post);
    public Request AsPut() => WithMethod(HttpMethod.Put);
    public Request AsPatch() => WithMethod(HttpMethod.Patch);
    public Request AsDelete() => WithMethod(HttpMethod.Delete);
    public Request AsHead() => WithMethod(HttpMethod.Head);
    public Request AsOptions() => WithMethod(HttpMethod.Options);
    public Request AsTrace() => WithMethod(HttpMethod.Trace);

    #endregion

    #region Headers Builder

    public Request WithHeaders(RequestHeaderBuilder headerBuilder)
    {
        _requestBuilder.WithHeaders(headerBuilder);
        return this;
    }

    public Request WithHeader(string name, string value)
    {
        EnsureHeaderBuilder();
        _requestBuilder.RequestHeaderBuilder!.SetHeader(name, value);
        return this;
    }

    public Request WithAccept(params string[] acceptTypes)
    {
        EnsureHeaderBuilder();
        _requestBuilder.RequestHeaderBuilder!.WithAccept(acceptTypes);
        return this;
    }

    public Request WithAcceptJson() => WithAccept("json");
    public Request WithAcceptXml() => WithAccept("xml");
    public Request WithAcceptHtml() => WithAccept("html");

    public Request WithUserAgent(string userAgent)
    {
        EnsureHeaderBuilder();
        _requestBuilder.RequestHeaderBuilder!.WithUserAgent(userAgent);
        return this;
    }

    public Request WithBogusUserAgent()
    {
        EnsureHeaderBuilder();
        _requestBuilder.RequestHeaderBuilder!.WithBogusUserAgent();
        return this;
    }

    public Request WithBogusUserAgent<T>() where T : IBogusUserAgentGenerator
    {
        EnsureHeaderBuilder();
        _requestBuilder.RequestHeaderBuilder!.WithBogusUserAgent<T>();
        return this;
    }

    public Request WithBogusUserAgent<T1, T2>() 
        where T1 : IBogusUserAgentGenerator 
        where T2 : IBogusUserAgentGenerator
    {
        EnsureHeaderBuilder();
        _requestBuilder.RequestHeaderBuilder!.WithBogusUserAgent<T1, T2>();
        return this;
    }

    public Request WithReferer(string referer)
    {
        return WithHeader("Referer", referer);
    }

    public Request WithCookie(string cookie)
    {
        return WithHeader("Cookie", cookie);
    }

    private void EnsureHeaderBuilder()
    {
        if (_requestBuilder.RequestHeaderBuilder == null)
            _requestBuilder.WithHeaders(new RequestHeaderBuilder());
    }

    #endregion

    #region Authentication Builder

    public Request UseBasicAuthentication(string username, string password)
    {
        EnsureHeaderBuilder();
        _requestBuilder.RequestHeaderBuilder!.UseBasicAuthentication(username, password);
        return this;
    }

    public Request UseBearerAuthentication(string token)
    {
        EnsureHeaderBuilder();
        _requestBuilder.RequestHeaderBuilder!.UseBearerAuthentication(token);
        return this;
    }

    #endregion

    #region Body Builder

    public Request WithRawBody(RequestRawBodyBuilder bodyBuilder)
    {
        _requestBuilder.WithRaw(bodyBuilder);
        return this;
    }

    public Request WithRawBody(string content, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var builder = new RequestRawBodyBuilder();
        builder.WithText(content, encoding);
        return WithRawBody(builder);
    }

    public Request WithJsonBody(string jsonContent, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var builder = new RequestRawBodyBuilder();
        builder.WithJson(jsonContent, encoding);
        return WithRawBody(builder);
    }

    public Request WithJsonBody<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return WithJsonBody(json, Encoding.UTF8);
    }

    public Request WithBufferBody(byte[] buffer)
    {
        var builder = new RequestRawBodyBuilder();
        builder.WithBuffer(buffer);
        return WithRawBody(builder);
    }

    public Request WithBufferBody(Memory<byte> buffer)
    {
        return WithBufferBody(buffer.ToArray());
    }

    public Request WithEncodedForm(RequestEncodedKeyValueListBodyBuilder formBuilder)
    {
        _requestBuilder.WithEncodedForm(formBuilder);
        return this;
    }

    public Request WithEncodedForm(params (string key, string value)[] formData)
    {
        var builder = new RequestEncodedKeyValueListBodyBuilder();
        foreach (var (key, value) in formData)
            builder.AddText(key, value);
        return WithEncodedForm(builder);
    }

    public Request WithForm(RequestKeyValueListBodyBuilder formBuilder)
    {
        _requestBuilder.WithForm(formBuilder);
        return this;
    }

    #endregion

    #region Configuration Builder

    public Request WithTimeout(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeout, TimeSpan.Zero);
        _timeout = timeout;
        return this;
    }

    public Request WithCancellationToken(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        return this;
    }

    public Request WithProxy(TrackedProxyInfo? proxy)
    {
        _proxy = proxy;
        return this;
    }

    public Request WithProxy(ProxyInfo proxy)
    {
        _proxy = new TrackedProxyInfo(proxy);
        return this;
    }

    public Request WithRetryPolicy(RetryPolicy policy)
    {
        _retryPolicy = policy ?? RetryPolicy.Default;
        return this;
    }

    public Request WithScrapingBypass(ScrapingBypassConfig config)
    {
        _scrapingBypass = config;
        return this;
    }

    public Request WithJsonPathParsing(JsonPathConfig config)
    {
        _jsonPathConfig = config;
        return this;
    }

    public Request WithHostCheck(HostCheckConfig config)
    {
        _hostCheckConfig = config;
        return this;
    }

    public Request WithLogging(LoggingConfig config)
    {
        _loggingConfig = config;
        return this;
    }

    public Request WithCertificateValidation(bool validate)
    {
        _validateCertificates = validate;
        return this;
    }

    public Request WithFollowRedirects(bool follow, int maxRedirects = 50)
    {
        _followRedirects = follow;
        _maxRedirects = maxRedirects;
        return this;
    }

    public static void ConfigureConnectionPool(
        TimeSpan? connectionLifetime = null,
        TimeSpan? connectionIdleTimeout = null,
        int? maxConnections = null)
    {
        if (connectionLifetime.HasValue)
            PooledConnectionLifetime = connectionLifetime.Value;
        if (connectionIdleTimeout.HasValue)
            PooledConnectionIdleTimeout = connectionIdleTimeout.Value;
        if (maxConnections.HasValue)
            MaxConnectionsPerServer = maxConnections.Value;
    }

    #endregion

    #region Interceptors Builder

    public Request WithRequestInterceptor(IRequestInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);
        _requestInterceptors.Add(interceptor);
        return this;
    }

    public Request WithResponseInterceptor(IResponseInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);
        _responseInterceptors.Add(interceptor);
        return this;
    }

    #endregion

    #region Build & Send

    public Request Build()
    {
        if (_isBuilt)
            return this;
        
        _requestBuilder.Build();
        _isBuilt = true;
        return this;
    }

    public async Task<Response> SendAsync(CancellationToken cancellationToken = default)
    {
        Build();
        
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);
        var token = linkedCts.Token;
        
        var metricsBuilder = new RequestMetricsBuilder();
        var attemptNumber = 0;
        System.Exception? lastException = null;

        // Run request interceptors
        foreach (var interceptor in _requestInterceptors.OrderBy(i => i.Order))
        {
            await interceptor.OnRequestAsync(this, token);
        }

        // Host reachability check
        if (_hostCheckConfig?.Enabled == true)
        {
            await CheckHostReachabilityAsync(token);
        }

        while (attemptNumber <= _retryPolicy.MaxRetries)
        {
            attemptNumber++;
            
            if (attemptNumber > 1)
            {
                metricsBuilder.IncrementRetryCount();
                var delay = _retryPolicy.GetDelay(attemptNumber - 1);
                await Task.Delay(delay, token);
            }

            try
            {
                var client = GetOrCreateClient();
                
                using var timeoutCts = new CancellationTokenSource(_timeout);
                using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);

                metricsBuilder.SetProxy(_proxy != null, _proxy?.Key);

                using var httpRequest = ToHttpRequestMessage();
                httpRequest.Version = new Version(3, 0);
                httpRequest.VersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

                metricsBuilder.MarkConnectStart();
                var httpResponse = await client.SendAsync(httpRequest, 
                    HttpCompletionOption.ResponseHeadersRead, combinedCts.Token);
                metricsBuilder.MarkConnectEnd();
                metricsBuilder.MarkFirstByte();

                // Check for rate limiting
                if (httpResponse.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    var rateLimitException = HandleRateLimitResponse(httpResponse);
                    
                    if (attemptNumber <= _retryPolicy.MaxRetries)
                    {
                        lastException = rateLimitException;
                        
                        if (rateLimitException.RetryAfter.HasValue)
                            await Task.Delay(rateLimitException.RetryAfter.Value, token);
                        
                        continue;
                    }
                    
                    throw rateLimitException;
                }

                // Read response content into Memory<byte>
                var contentStream = new MemoryStream();
                await httpResponse.Content.CopyToAsync(contentStream, combinedCts.Token);
                metricsBuilder.MarkDownloadEnd();
                metricsBuilder.SetBytesReceived(contentStream.Length);
                metricsBuilder.SetProtocol($"HTTP/{httpResponse.Version}");

                _proxy?.ReportSuccess();

                var response = new Response(httpResponse, contentStream, metricsBuilder.Build())
                {
                    RequestUri = new Uri(Uri.ToString()),
                    FromCache = false
                };

                // Run response interceptors
                foreach (var interceptor in _responseInterceptors.OrderBy(i => i.Order))
                {
                    await interceptor.OnResponseAsync(response, token);
                }

                return response;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                lastException = new RequestTimeoutException(_timeout, new Uri(Uri.ToString()), attemptNumber);
                
                if (!_retryPolicy.RetryOnTimeout || attemptNumber > _retryPolicy.MaxRetries)
                    throw lastException;
            }
            catch (HttpRequestException ex) when (IsProxyError(ex))
            {
                _proxy?.ReportFailure();
                lastException = new ProxyException(ex.Message, ex, _proxy?.Proxy, attemptNumber);
                
                if (!_retryPolicy.RetryOnProxyError || attemptNumber > _retryPolicy.MaxRetries)
                    throw lastException;
            }
            catch (HttpRequestException ex)
            {
                var uri = Uri.ToString();
                var host = new Uri(uri).Host;
                lastException = new NetworkException(ex.Message, ex, host, attemptNumber);
                
                if (!_retryPolicy.RetryOnNetworkError || attemptNumber > _retryPolicy.MaxRetries)
                    throw lastException;
            }
            catch (System.Exception ex)
            {
                lastException = ex;
                throw;
            }
        }

        throw lastException ?? (System.Exception)new NetworkException("Request failed after all retries");
    }

    public HttpRequestMessage ToHttpRequestMessage()
    {
        var uri = new Uri(Uri.ToString());
        var message = new HttpRequestMessage(_method, uri);

        // Add headers from RequestHeaderBuilder
        if (_requestBuilder.RequestHeaderBuilder != null)
        {
            foreach (var entry in _requestBuilder.RequestHeaderBuilder.GetEntries())
            {
                message.Headers.TryAddWithoutValidation(entry.Key, entry.Value);
            }
        }

        // Add body if present
        if (!Body.IsEmpty)
        {
            var bodyArray = Body.ToArray();
            message.Content = new ByteArrayContent(bodyArray);
            
            // Set content type from MimeDictionary
            if (_mimeDictionary.TryGetMimeTypeAsString("json", out var jsonMime) && 
                _requestBuilder.RequestHeaderBuilder?.GetHeader("Content-Type") == null)
            {
                message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(jsonMime);
            }
        }

        return message;
    }

    #endregion

    #region HttpClient Management

    private HttpClient GetOrCreateClient()
    {
        var key = BuildClientKey();

        lock (PoolLock)
        {
            if (ClientPool.TryGetValue(key, out var existingClient))
                return existingClient;

            var handler = CreateHandler();
            var client = new HttpClient(handler, disposeHandler: true);
            
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));

            ClientPool[key] = client;
            return client;
        }
    }

    private string BuildClientKey()
    {
        var proxyKey = _proxy?.Key ?? "direct";
        return $"{proxyKey}|validate:{_validateCertificates}|redirect:{_followRedirects}";
    }

    private SocketsHttpHandler CreateHandler()
    {
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = PooledConnectionLifetime,
            PooledConnectionIdleTimeout = PooledConnectionIdleTimeout,
            MaxConnectionsPerServer = MaxConnectionsPerServer,
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = _followRedirects,
            MaxAutomaticRedirections = _maxRedirects,
            EnableMultipleHttp2Connections = true,
            ConnectTimeout = TimeSpan.FromSeconds(30)
        };

        if (!_validateCertificates)
        {
            handler.SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true
            };
        }

        if (_proxy != null)
        {
            var proxyType = _proxy.Proxy.Type;
            
            if (proxyType == DevBase.Requests.Proxy.Enums.EnumProxyType.Http || proxyType == DevBase.Requests.Proxy.Enums.EnumProxyType.Https)
            {
                handler.Proxy = _proxy.ToWebProxy();
                handler.UseProxy = true;
            }
        }

        return handler;
    }

    #endregion

    #region Host Reachability

    private async Task CheckHostReachabilityAsync(CancellationToken cancellationToken)
    {
        var uri = new Uri(Uri.ToString());
        var host = uri.Host;

        if (_hostCheckConfig!.Method == HostCheckMethod.Ping)
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(host, (int)_hostCheckConfig.Timeout.TotalMilliseconds);
            
            if (reply.Status != IPStatus.Success)
                throw new NetworkException($"Host {host} is not reachable (ping failed: {reply.Status})");
        }
        else
        {
            using var client = new TcpClient();
            var port = _hostCheckConfig.Port > 0 ? _hostCheckConfig.Port : uri.Port;
            var connectTask = client.ConnectAsync(host, port);
            
            if (await Task.WhenAny(connectTask, Task.Delay(_hostCheckConfig.Timeout, cancellationToken)) != connectTask)
                throw new NetworkException($"Host {host}:{port} is not reachable (connection timeout)");

            await connectTask;
        }
    }

    #endregion

    #region Error Handling

    private static bool IsProxyError(HttpRequestException ex)
    {
        var message = ex.Message.ToLowerInvariant();
        return message.Contains("proxy") || 
               message.Contains("407") ||
               ex.StatusCode == HttpStatusCode.ProxyAuthenticationRequired;
    }

    private RateLimitException HandleRateLimitResponse(HttpResponseMessage response)
    {
        var requestUri = new Uri(Uri.ToString());
        
        if (response.Headers.TryGetValues("Retry-After", out var retryAfterValues))
        {
            var retryAfter = retryAfterValues.FirstOrDefault();
            if (int.TryParse(retryAfter, out var seconds))
                return RateLimitException.FromRetryAfter(seconds, requestUri);
            
            if (DateTime.TryParse(retryAfter, out var dateTime))
                return RateLimitException.FromResetTime(dateTime.ToUniversalTime(), requestUri);
        }

        if (response.Headers.TryGetValues("X-RateLimit-Reset", out var resetValues))
        {
            var resetStr = resetValues.FirstOrDefault();
            if (long.TryParse(resetStr, out var unixTime))
            {
                var resetTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
                return RateLimitException.FromResetTime(resetTime, requestUri);
            }
        }

        return new RateLimitException("Rate limited", TimeSpan.FromSeconds(60), null, requestUri);
    }

    #endregion

    #region Static Factory

    public static Request Create() => new();
    public static Request Create(string url) => new(url);
    public static Request Create(Uri uri) => new(uri);
    public static Request Create(string url, HttpMethod method) => new(url, method);

    public static void ClearClientPool()
    {
        lock (PoolLock)
        {
            foreach (var client in ClientPool.Values)
                client.Dispose();
            ClientPool.Clear();
        }
    }

    #endregion

    #region Disposal

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        _requestInterceptors.Clear();
        _responseInterceptors.Clear();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    #endregion
}