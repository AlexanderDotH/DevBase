# DevBase.Net - AI Agent Guide

This guide helps AI agents effectively use the DevBase.Net HTTP client library.

## Overview

DevBase.Net is the networking backbone of the DevBase solution. It provides a high-performance HTTP client with advanced features like HTTP/HTTPS/SOCKS4/SOCKS5/SSH proxy support, retry policies, interceptors, batch processing, proxy rotation, and detailed metrics.

**Target Framework:** .NET 9.0  
**Current Version:** 1.1.0

---

## Project Structure

```
DevBase.Net/
├── Core/                    # Main HTTP client classes
│   ├── Request.cs           # Core request class (properties, constructors)
│   ├── RequestConfiguration.cs  # Fluent configuration API
│   ├── RequestHttp.cs       # HTTP execution logic (SendAsync)
│   ├── Response.cs          # Response parsing and streaming
│   ├── Requests.cs          # Simple queue-based processor
│   ├── BatchRequests.cs     # Named batch processor with progress
│   └── ProxiedBatchRequests.cs  # Batch processor with proxy rotation
├── Proxy/                   # Proxy support
│   ├── ProxyInfo.cs         # Proxy configuration and parsing
│   ├── ProxyConfiguration.cs # Fluent proxy builders
│   ├── TrackedProxyInfo.cs  # Failure tracking wrapper
│   ├── HttpToSocks5/        # SOCKS proxy tunneling
│   └── Socks/               # SOCKS protocol implementation
├── Configuration/           # Configuration classes
│   ├── RetryPolicy.cs       # Retry with backoff strategies
│   ├── LoggingConfig.cs     # Request/response logging
│   └── ...                  # Other config classes
├── Data/                    # Request building
│   ├── Body/                # Request body builders
│   ├── Header/              # Header builders
│   └── Parameters/          # URL parameter builders
├── Exceptions/              # Custom exception types
├── Metrics/                 # Performance metrics
└── Utils/                   # Utility classes
```

---

## Core Architecture

### Request Lifecycle

```
1. CREATE     → new Request(url)
2. CONFIGURE  → .WithHeaders() / .WithBody() / .WithProxy()
3. BUILD      → .Build() (automatic on SendAsync)
4. INTERCEPT  → IRequestInterceptor.OnRequestAsync()
5. EXECUTE    → HttpClient.SendAsync()
6. RETRY      → RetryPolicy handles failures
7. INTERCEPT  → IResponseInterceptor.OnResponseAsync()
8. PARSE      → response.ParseJsonAsync<T>()
9. DISPOSE    → response.Dispose()
```

### Key Components

| Component | Namespace | Purpose |
|-----------|-----------|---------|
| `Request` | `DevBase.Net.Core` | Main entry point, fluent builder, manages HTTP execution |
| `Response` | `DevBase.Net.Core` | Wraps HttpResponseMessage, provides parsing methods |
| `Requests` | `DevBase.Net.Core` | Simple queue-based request processor |
| `BatchRequests` | `DevBase.Net.Core` | Named batch processor with progress callbacks |
| `ProxiedBatchRequests` | `DevBase.Net.Core` | Batch processor with proxy rotation and failure tracking |
| `ProxyInfo` | `DevBase.Net.Proxy` | Proxy configuration with HTTP/HTTPS/SOCKS4/SOCKS5/SSH support |
| `ProxyConfiguration` | `DevBase.Net.Proxy` | Fluent builder for provider-specific proxy settings |
| `TrackedProxyInfo` | `DevBase.Net.Proxy` | Proxy wrapper with failure tracking |
| `RetryPolicy` | `DevBase.Net.Configuration` | Retry configuration with backoff strategies |
| `RequestMetrics` | `DevBase.Net.Metrics` | Captures timing data |

---

## Class Reference

### Request Class

**Namespace:** `DevBase.Net.Core`

The main HTTP client class with fluent API for building and sending requests.

#### Constructors

```csharp
Request()
Request(string url)
Request(Uri uri)
Request(string url, HttpMethod method)
Request(Uri uri, HttpMethod method)
Request(ReadOnlyMemory<char> url)
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Uri` | `ReadOnlySpan<char>` | The request URI |
| `Body` | `ReadOnlySpan<byte>` | The request body |
| `Method` | `HttpMethod` | HTTP method (GET, POST, etc.) |
| `Timeout` | `TimeSpan` | Request timeout (default: 30s) |
| `CancellationToken` | `CancellationToken` | Cancellation token |
| `Proxy` | `TrackedProxyInfo?` | Proxy configuration |
| `RetryPolicy` | `RetryPolicy` | Retry configuration |
| `ValidateCertificates` | `bool` | SSL certificate validation |
| `FollowRedirects` | `bool` | Follow HTTP redirects |
| `MaxRedirects` | `int` | Maximum redirects (default: 50) |

#### Configuration Methods

```csharp
// URL Configuration
Request WithUrl(string url)
Request WithUrl(Uri uri)
Request WithParameters(ParameterBuilder parameterBuilder)
Request WithParameter(string key, string value)
Request WithParameters(params (string key, string value)[] parameters)

// HTTP Method
Request WithMethod(HttpMethod method)
Request AsGet()
Request AsPost()
Request AsPut()
Request AsPatch()
Request AsDelete()
Request AsHead()
Request AsOptions()
Request AsTrace()

// Headers
Request WithHeaders(RequestHeaderBuilder headerBuilder)
Request WithHeader(string name, string value)
Request WithAccept(params string[] acceptTypes)
Request WithAcceptJson()
Request WithAcceptXml()
Request WithAcceptHtml()
Request WithUserAgent(string userAgent)
Request WithBogusUserAgent()
Request WithBogusUserAgent<T>() where T : IBogusUserAgentGenerator
Request WithReferer(string referer)
Request WithCookie(string cookie)

// Authentication
Request UseBasicAuthentication(string username, string password)
Request UseBearerAuthentication(string token)
Request UseJwtAuthentication(AuthenticationToken token)
Request UseJwtAuthentication(string rawToken)

// Request Body
Request WithRawBody(RequestRawBodyBuilder bodyBuilder)
Request WithRawBody(string content, Encoding? encoding = null)
Request WithJsonBody(string jsonContent, Encoding? encoding = null)  // Supports both objects {} and arrays []
Request WithJsonBody<T>(T obj)  // Serializes any object including List<T>, arrays, etc.
Request WithBufferBody(byte[] buffer)
Request WithBufferBody(Memory<byte> buffer)
Request WithEncodedForm(RequestEncodedKeyValueListBodyBuilder formBuilder)
Request WithEncodedForm(params (string key, string value)[] formData)
Request WithForm(RequestKeyValueListBodyBuilder formBuilder)

// Execution Configuration
Request WithTimeout(TimeSpan timeout)
Request WithCancellationToken(CancellationToken cancellationToken)
Request WithProxy(TrackedProxyInfo? proxy)
Request WithProxy(ProxyInfo proxy)
Request WithProxy(string proxyString)  // Parse from string: [protocol://][user:pass@]host:port
Request WithRetryPolicy(RetryPolicy policy)
Request WithCertificateValidation(bool validate)
Request WithHeaderValidation(bool validate)
Request WithFollowRedirects(bool follow, int maxRedirects = 50)

// Advanced Configuration
Request WithScrapingBypass(ScrapingBypassConfig config)  // Browser spoofing and anti-detection
Request WithJsonPathParsing(JsonPathConfig config)
Request WithHostCheck(HostCheckConfig config)
Request WithLogging(LoggingConfig config)
Request WithRequestInterceptor(IRequestInterceptor interceptor)
Request WithResponseInterceptor(IResponseInterceptor interceptor)
```

#### Execution Methods

```csharp
Request Build()
Task<Response> SendAsync(CancellationToken cancellationToken = default)
HttpRequestMessage ToHttpRequestMessage()
```

#### Static Methods

```csharp
static Request Create()
static Request Create(string url)
static Request Create(Uri uri)
static Request Create(string url, HttpMethod method)
static void ConfigureConnectionPool(TimeSpan? connectionLifetime, TimeSpan? connectionIdleTimeout, int? maxConnections)
static void ClearClientPool()
```

---

### Response Class

**Namespace:** `DevBase.Net.Core`

Wraps the HTTP response with parsing and streaming capabilities.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `StatusCode` | `HttpStatusCode` | HTTP status code |
| `IsSuccessStatusCode` | `bool` | True if 2xx status |
| `Headers` | `HttpResponseHeaders` | Response headers |
| `ContentHeaders` | `HttpContentHeaders?` | Content headers |
| `ContentType` | `string?` | Content-Type header |
| `ContentLength` | `long?` | Content-Length header |
| `HttpVersion` | `Version` | HTTP protocol version |
| `ReasonPhrase` | `string?` | Status reason phrase |
| `Metrics` | `RequestMetrics` | Performance metrics |
| `FromCache` | `bool` | True if from cache |
| `RequestUri` | `Uri?` | Original request URI |
| `IsRedirect` | `bool` | True if redirect response |
| `IsClientError` | `bool` | True if 4xx status |
| `IsServerError` | `bool` | True if 5xx status |
| `IsRateLimited` | `bool` | True if 429 status |

#### Content Methods

```csharp
Task<byte[]> GetBytesAsync(CancellationToken cancellationToken = default)
Task<string> GetStringAsync(Encoding? encoding = null, CancellationToken cancellationToken = default)
Stream GetStream()
```

#### Parsing Methods

```csharp
Task<T> GetAsync<T>(CancellationToken cancellationToken = default)
Task<T> ParseJsonAsync<T>(bool useSystemTextJson = true, CancellationToken cancellationToken = default)
Task<JsonDocument> ParseJsonDocumentAsync(CancellationToken cancellationToken = default)
Task<XDocument> ParseXmlAsync(CancellationToken cancellationToken = default)
Task<IDocument> ParseHtmlAsync(CancellationToken cancellationToken = default)
Task<T> ParseJsonPathAsync<T>(string path, CancellationToken cancellationToken = default)
Task<List<T>> ParseJsonPathListAsync<T>(string path, CancellationToken cancellationToken = default)
Task<MultiSelectorResult> ParseMultipleJsonPathsAsync(MultiSelectorConfig config, CancellationToken cancellationToken = default)
Task<MultiSelectorResult> ParseMultipleJsonPathsAsync(CancellationToken cancellationToken = default, params (string name, string path)[] selectors)
Task<MultiSelectorResult> ParseMultipleJsonPathsOptimizedAsync(CancellationToken cancellationToken = default, params (string name, string path)[] selectors)
```

#### Streaming Methods

```csharp
IAsyncEnumerable<string> StreamLinesAsync(CancellationToken cancellationToken = default)
IAsyncEnumerable<byte[]> StreamChunksAsync(int chunkSize = 4096, CancellationToken cancellationToken = default)
```

#### Header Methods

```csharp
string? GetHeader(string name)
IEnumerable<string> GetHeaderValues(string name)
CookieCollection GetCookies()
void EnsureSuccessStatusCode()
```

---

### BatchRequests Class

**Namespace:** `DevBase.Net.Core`

Processes requests in named batches with rate limiting, progress tracking, and re-queue support.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `BatchCount` | `int` | Number of batches |
| `TotalQueueCount` | `int` | Total queued requests |
| `ResponseQueueCount` | `int` | Completed responses |
| `RateLimit` | `int` | Requests per window |
| `PersistCookies` | `bool` | Cookie persistence enabled |
| `PersistReferer` | `bool` | Referer persistence enabled |
| `IsProcessing` | `bool` | Background processing active |
| `ProcessedCount` | `int` | Total processed requests |
| `ErrorCount` | `int` | Total errors |
| `BatchNames` | `IReadOnlyList<string>` | List of batch names |

#### Configuration Methods

```csharp
BatchRequests WithRateLimit(int requestsPerWindow, TimeSpan? window = null)
BatchRequests WithCookiePersistence(bool persist = true)
BatchRequests WithRefererPersistence(bool persist = true)
```

#### Batch Management

```csharp
Batch CreateBatch(string name)
Batch GetOrCreateBatch(string name)
Batch? GetBatch(string name)
bool RemoveBatch(string name)
BatchRequests ClearAllBatches()
```

#### Callback Registration

```csharp
BatchRequests OnResponse(Func<Response, Task> callback)
BatchRequests OnResponse(Action<Response> callback)
BatchRequests OnError(Func<Request, Exception, Task> callback)
BatchRequests OnError(Action<Request, Exception> callback)
BatchRequests OnProgress(Func<BatchProgressInfo, Task> callback)
BatchRequests OnProgress(Action<BatchProgressInfo> callback)
BatchRequests OnResponseRequeue(Func<Response, Request, RequeueDecision> callback)
BatchRequests OnErrorRequeue(Func<Request, Exception, RequeueDecision> callback)
```

#### Execution Methods

```csharp
Task<List<Response>> ExecuteAllAsync(CancellationToken cancellationToken = default)
IAsyncEnumerable<Response> ExecuteAllAsyncEnumerable(CancellationToken cancellationToken = default)
Task<List<Response>> ExecuteBatchAsync(string batchName, CancellationToken cancellationToken = default)
void StartProcessing()
Task StopProcessingAsync()
```

#### Response Management

```csharp
bool TryDequeueResponse(out Response? response)
List<Response> DequeueAllResponses()
```

#### Statistics

```csharp
BatchStatistics GetStatistics()
void ResetCounters()
```

---

### Batch Class (Inner class of BatchRequests)

**Namespace:** `DevBase.Net.Core`

Represents a named group of requests within BatchRequests.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Batch name |
| `QueueCount` | `int` | Queued request count |

#### Methods

```csharp
Batch Add(Request request)
Batch Add(IEnumerable<Request> requests)
Batch Add(string url)
Batch Add(IEnumerable<string> urls)
Batch Enqueue(Request request)
Batch Enqueue(string url)
Batch Enqueue(string url, Action<Request> configure)
Batch Enqueue(Func<Request> requestFactory)
bool TryDequeue(out Request? request)
void Clear()
BatchRequests EndBatch()
```

---

### ProxiedBatchRequests Class

**Namespace:** `DevBase.Net.Core`

Extends BatchRequests with proxy rotation, failure tracking, and proxy-specific callbacks.

#### Additional Properties

| Property | Type | Description |
|----------|------|-------------|
| `ProxyCount` | `int` | Total proxies |
| `AvailableProxyCount` | `int` | Available proxies |
| `ProxyFailureCount` | `int` | Total proxy failures |

#### Proxy Configuration (Fluent - returns ProxiedBatchRequests)

```csharp
ProxiedBatchRequests WithProxy(ProxyInfo proxy)
ProxiedBatchRequests WithProxy(string proxyString)
ProxiedBatchRequests WithProxies(IEnumerable<ProxyInfo> proxies)
ProxiedBatchRequests WithProxies(IEnumerable<string> proxyStrings)
ProxiedBatchRequests WithMaxProxyRetries(int maxRetries)  // Default: 3
ProxiedBatchRequests ConfigureProxyTracking(int maxFailures = 3, TimeSpan? timeoutDuration = null)
```

#### Dynamic Proxy Addition (Thread-Safe - can be called during processing)

```csharp
void AddProxy(ProxyInfo proxy)
void AddProxy(string proxyString)
void AddProxies(IEnumerable<ProxyInfo> proxies)
void AddProxies(IEnumerable<string> proxyStrings)
void ClearProxies()
void ResetAllProxies()
```

#### Rotation Strategy

```csharp
ProxiedBatchRequests WithRotationStrategy(IProxyRotationStrategy strategy)
ProxiedBatchRequests WithRoundRobinRotation()
ProxiedBatchRequests WithRandomRotation()
ProxiedBatchRequests WithLeastFailuresRotation()
ProxiedBatchRequests WithStickyRotation()
```

#### Proxy Callbacks

```csharp
ProxiedBatchRequests OnProxyFailure(Func<TrackedProxyInfo, ProxyFailureContext, Task> callback)
ProxiedBatchRequests OnProxyFailure(Action<TrackedProxyInfo, ProxyFailureContext> callback)
```

#### Proxy Statistics

```csharp
IReadOnlyList<TrackedProxyInfo> GetProxyStatistics()
ProxiedBatchStatistics GetStatistics()
void ResetProxies()
```

---

### ProxyInfo Class

**Namespace:** `DevBase.Net.Proxy`

Immutable proxy configuration with support for HTTP, HTTPS, SOCKS4, SOCKS5, SOCKS5h, and SSH tunnels.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Host` | `string` | Proxy hostname |
| `Port` | `int` | Proxy port |
| `Type` | `EnumProxyType` | Proxy type |
| `Credentials` | `NetworkCredential?` | Authentication |
| `Key` | `string` | Unique identifier |
| `HasAuthentication` | `bool` | Has credentials |
| `BypassLocalAddresses` | `bool` | Bypass local |
| `BypassList` | `string[]?` | Bypass patterns |
| `ResolveHostnamesLocally` | `bool` | DNS resolution location |
| `ConnectionTimeout` | `TimeSpan` | Connection timeout |
| `ReadWriteTimeout` | `TimeSpan` | Read/write timeout |
| `InternalServerPort` | `int` | Internal HTTP listener port |

#### Constructors

```csharp
ProxyInfo(string host, int port, EnumProxyType type = EnumProxyType.Http, NetworkCredential? credentials = null)
ProxyInfo(string host, int port, string username, string password, EnumProxyType type = EnumProxyType.Http)
```

#### Static Methods

```csharp
static ProxyInfo Parse(string proxyString)
static bool TryParse(string proxyString, out ProxyInfo? proxyInfo)
static ProxyInfo FromConfiguration(ProxyConfiguration config)
static void ClearProxyCache()
```

#### Instance Methods

```csharp
Uri ToUri()
IWebProxy ToWebProxy()
```

---

### ProxyConfiguration Class

**Namespace:** `DevBase.Net.Proxy`

Fluent builder for creating ProxyInfo with provider-specific options.

#### Static Factory Methods

```csharp
static HttpProxyBuilder Http(string host, int port)
static HttpProxyBuilder Https(string host, int port)
static Socks4ProxyBuilder Socks4(string host, int port)
static Socks5ProxyBuilder Socks5(string host, int port)
static Socks5ProxyBuilder Socks5h(string host, int port)
```

#### HttpProxyBuilder Methods

```csharp
HttpProxyBuilder WithCredentials(string username, string password)
HttpProxyBuilder WithCredentials(NetworkCredential credentials)
HttpProxyBuilder BypassLocal(bool bypass = true)
HttpProxyBuilder WithBypassList(params string[] addresses)
ProxyConfiguration Build()
ProxyInfo ToProxyInfo()
```

#### Socks4ProxyBuilder Methods

```csharp
Socks4ProxyBuilder WithUserId(string userId)
Socks4ProxyBuilder WithConnectionTimeout(TimeSpan timeout)
Socks4ProxyBuilder WithReadWriteTimeout(TimeSpan timeout)
Socks4ProxyBuilder WithInternalServerPort(int port)
ProxyConfiguration Build()
ProxyInfo ToProxyInfo()
```

#### Socks5ProxyBuilder Methods

```csharp
Socks5ProxyBuilder WithCredentials(string username, string password)
Socks5ProxyBuilder WithCredentials(NetworkCredential credentials)
Socks5ProxyBuilder ResolveHostnamesLocally(bool resolveLocally = true)
Socks5ProxyBuilder ResolveHostnamesRemotely()
Socks5ProxyBuilder WithConnectionTimeout(TimeSpan timeout)
Socks5ProxyBuilder WithReadWriteTimeout(TimeSpan timeout)
Socks5ProxyBuilder WithInternalServerPort(int port)
ProxyConfiguration Build()
ProxyInfo ToProxyInfo()
```

---

### TrackedProxyInfo Class

**Namespace:** `DevBase.Net.Proxy`

Wraps ProxyInfo with failure tracking and automatic timeout.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Proxy` | `ProxyInfo` | Underlying proxy |
| `FailureCount` | `int` | Current failures |
| `LastFailure` | `DateTime?` | Last failure time |
| `IsTimedOut` | `bool` | Currently timed out |
| `TimeoutUntil` | `DateTime?` | Timeout end time |
| `TotalTimeouts` | `int` | Total timeouts |
| `MaxFailures` | `int` | Max failures before timeout |
| `TimeoutDuration` | `TimeSpan` | Timeout duration |
| `RemainingTimeout` | `TimeSpan?` | Remaining timeout |
| `Key` | `string` | Proxy key |

#### Constructor

```csharp
TrackedProxyInfo(ProxyInfo proxy, int maxFailures = 3, TimeSpan? timeoutDuration = null)
```

#### Methods

```csharp
bool ReportFailure()
void ReportSuccess()
bool IsAvailable()
void ResetTimeout()
IWebProxy ToWebProxy()
```

---

### RetryPolicy Class

**Namespace:** `DevBase.Net.Configuration`

Configures retry behavior with backoff strategies.

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MaxRetries` | `int` | 3 | Maximum retry attempts (all errors count) |
| `BackoffStrategy` | `EnumBackoffStrategy` | Exponential | Delay strategy |
| `InitialDelay` | `TimeSpan` | 500ms | First retry delay |
| `MaxDelay` | `TimeSpan` | 30s | Maximum delay |
| `BackoffMultiplier` | `double` | 2.0 | Delay multiplier |

#### Static Presets

```csharp
static RetryPolicy Default    // 3 retries, exponential backoff
static RetryPolicy None       // No retries
static RetryPolicy Aggressive // 5 retries, fast backoff
```

#### Methods

```csharp
TimeSpan GetDelay(int attemptNumber)
```

---

### ScrapingBypassConfig Class

**Namespace:** `DevBase.Net.Configuration`

Configures browser spoofing and anti-detection features to bypass scraping protections.

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BrowserProfile` | `EnumBrowserProfile` | None | Browser to emulate (Chrome, Firefox, Edge, Safari) |
| `RefererStrategy` | `EnumRefererStrategy` | None | Referer header strategy |

> **Note:** Providing a `ScrapingBypassConfig` implies it is enabled. To disable browser spoofing, simply don't call `WithScrapingBypass()`.

#### Static Presets

```csharp
static ScrapingBypassConfig Default  // Chrome profile with PreviousUrl referer
```

#### Browser Profiles

- **Chrome**: Emulates Google Chrome with Chromium client hints (sec-ch-ua headers)
- **Firefox**: Emulates Mozilla Firefox with appropriate headers
- **Edge**: Emulates Microsoft Edge with Chromium client hints
- **Safari**: Emulates Apple Safari with appropriate headers
- **None**: No browser spoofing applied

#### Referer Strategies

- **None**: No Referer header added
- **PreviousUrl**: Use previous request URL as referer (for sequential requests)
- **BaseHost**: Use base host URL as referer (e.g., https://example.com/)
- **SearchEngine**: Use random search engine URL as referer (Google, Bing, DuckDuckGo)

---

### Enums

#### EnumProxyType

```csharp
namespace DevBase.Net.Proxy.Enums;

public enum EnumProxyType
{
    Http,     // Standard HTTP proxy
    Https,    // HTTPS/SSL proxy
    Socks4,   // SOCKS4 (no auth, local DNS)
    Socks5,   // SOCKS5 (auth support, configurable DNS)
    Socks5h,  // SOCKS5 with remote DNS resolution
    Ssh       // SSH tunnel (dynamic port forwarding)
}
```

#### EnumBackoffStrategy

```csharp
namespace DevBase.Net.Configuration.Enums;

public enum EnumBackoffStrategy
{
    Fixed,       // Same delay each retry
    Linear,      // delay * attemptNumber
    Exponential  // delay * multiplier^(attempt-1)
}
```

---

### Record Types

#### RequeueDecision

```csharp
public sealed record RequeueDecision(bool ShouldRequeue, Request? ModifiedRequest = null)
{
    public static RequeueDecision NoRequeue => new(false);
    public static RequeueDecision Requeue() => new(true);
    public static RequeueDecision RequeueWith(Request modifiedRequest) => new(true, modifiedRequest);
}
```

#### BatchProgressInfo

```csharp
public sealed record BatchProgressInfo(
    string BatchName,
    int Completed,
    int Total,
    int Errors
)
{
    public double PercentComplete => Total > 0 ? (double)Completed / Total * 100 : 0;
    public int Remaining => Total - Completed;
}
```

#### BatchStatistics

```csharp
public sealed record BatchStatistics(
    int BatchCount,
    int TotalQueuedRequests,
    int ProcessedRequests,
    int ErrorCount,
    Dictionary<string, int> RequestsPerBatch
)
{
    public double SuccessRate => ProcessedRequests > 0
        ? (double)(ProcessedRequests - ErrorCount) / ProcessedRequests * 100 : 0;
}
```

#### ProxiedBatchStatistics

```csharp
public sealed record ProxiedBatchStatistics(
    int BatchCount,
    int TotalQueuedRequests,
    int ProcessedRequests,
    int ErrorCount,
    int ProxyFailureCount,
    int TotalProxies,
    int AvailableProxies,
    Dictionary<string, int> RequestsPerBatch
)
{
    public double SuccessRate => ...;
    public double ProxyAvailabilityRate => ...;
}
```

#### ProxyFailureContext

```csharp
public sealed record ProxyFailureContext(
    TrackedProxyInfo Proxy,
    Exception Exception,
    Request FailedRequest,
    bool ProxyTimedOut,
    int CurrentFailureCount,
    int RemainingAvailableProxies
);
```

## Usage Patterns for AI Agents

### Pattern 1: Standard API Call

**Always use this pattern for external API interactions:**

```csharp
using DevBase.Net.Core;

// 1. Create request
var request = new Request("https://api.example.com/resource")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithHeader("Authorization", "Bearer token");

// 2. Send
using Response response = await request.SendAsync();

// 3. Validate
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine($"Error: {response.StatusCode}");
    return null;
}

// 4. Parse
var data = await response.ParseJsonAsync<MyType>();
```

### Pattern 2: POST with JSON Body

```csharp
var payload = new { name = "John", age = 30 };

var response = await new Request("https://api.example.com/users")
    .AsPost()
    .WithJsonBody(payload)
    .WithHeader("Content-Type", "application/json")
    .SendAsync();
```

### Pattern 3: Proxy Configuration by Type

**ProxyInfo supports all proxy types with automatic handling:**

```csharp
using DevBase.Net.Proxy;
using DevBase.Net.Proxy.Enums;

// HTTP Proxy (standard web proxy)
var httpProxy = new ProxyInfo("proxy.example.com", 8080, EnumProxyType.Http);

// HTTPS Proxy (SSL/TLS proxy)
var httpsProxy = new ProxyInfo("proxy.example.com", 8443, EnumProxyType.Https);

// SOCKS4 Proxy (no authentication support)
var socks4Proxy = new ProxyInfo("proxy.example.com", 1080, EnumProxyType.Socks4);

// SOCKS5 Proxy (with optional authentication)
var socks5Proxy = new ProxyInfo("proxy.example.com", 1080, EnumProxyType.Socks5);

// SOCKS5h Proxy (remote DNS resolution - more private)
var socks5hProxy = new ProxyInfo("proxy.example.com", 1080, EnumProxyType.Socks5h);

// With authentication
var authProxy = new ProxyInfo("proxy.example.com", 1080, "username", "password", EnumProxyType.Socks5);

// Use with request
var response = await new Request("https://api.example.com")
    .WithProxy(httpProxy)
    .SendAsync();
```

**Proxy Type Comparison:**

| Type | Auth | DNS Resolution | Use Case |
|------|------|----------------|----------|
| `Http` | Optional | Local | Standard web browsing, caching |
| `Https` | Optional | Local | Secure proxy connections |
| `Socks4` | No | Local | Legacy systems, simple tunneling |
| `Socks5` | Optional | Local | General purpose, UDP support |
| `Socks5h` | Optional | Remote | Maximum privacy, bypass DNS leaks |
| `Ssh` | Optional | Remote | SSH tunnel with dynamic port forwarding |

**Parse proxy from string:**

```csharp
// Supported formats (all protocols: http, https, socks4, socks5, socks5h, ssh):
var proxy1 = ProxyInfo.Parse("http://proxy.example.com:8080");
var proxy2 = ProxyInfo.Parse("socks5://user:pass@proxy.example.com:1080");
var proxy3 = ProxyInfo.Parse("socks5h://user:pass@dc.oxylabs.io:8005");
var proxy4 = ProxyInfo.Parse("ssh://admin:pass@ssh.example.com:22");

// Use directly with Request
var response = await new Request("https://api.example.com")
    .WithProxy("socks5://paid1_563X7:rtVVhrth4545++A@dc.oxylabs.io:8005")
    .SendAsync();

// Safe parsing
if (ProxyInfo.TryParse("socks5://...", out var proxy))
{
    request.WithProxy(proxy);
}
```

**Note:** SOCKS proxies use `HttpToSocks5Proxy` internally which starts a local HTTP listener. The proxy cache ensures only one listener per unique proxy configuration.

### Pattern 3b: Advanced Proxy Configuration (Provider-Specific)

**Use `ProxyConfiguration` for fine-grained control over each proxy type:**

```csharp
using DevBase.Net.Proxy;

// HTTP/HTTPS Proxy - with bypass settings
var httpProxy = ProxyConfiguration.Http("proxy.example.com", 8080)
    .WithCredentials("user", "password")
    .BypassLocal(true)                           // Skip proxy for local addresses
    .WithBypassList("*.internal.com", "10.*")    // Skip proxy for these patterns
    .ToProxyInfo();

// HTTPS Proxy - same options as HTTP
var httpsProxy = ProxyConfiguration.Https("proxy.example.com", 8443)
    .WithCredentials("user", "password")
    .BypassLocal(true)
    .ToProxyInfo();

// SOCKS4 Proxy - supports user ID only (no password)
var socks4Proxy = ProxyConfiguration.Socks4("proxy.example.com", 1080)
    .WithUserId("myuserid")                      // SOCKS4 only supports user ID
    .WithConnectionTimeout(TimeSpan.FromSeconds(10))
    .WithReadWriteTimeout(TimeSpan.FromSeconds(30))
    .WithInternalServerPort(0)                   // 0 = auto-assign port
    .ToProxyInfo();

// SOCKS5 Proxy - full authentication and DNS control
var socks5Proxy = ProxyConfiguration.Socks5("proxy.example.com", 1080)
    .WithCredentials("user", "password")
    .ResolveHostnamesLocally(true)               // Local DNS resolution
    .WithConnectionTimeout(TimeSpan.FromSeconds(15))
    .WithInternalServerPort(9999)                // Fixed internal port
    .ToProxyInfo();

// SOCKS5h Proxy - remote DNS resolution (more private)
var socks5hProxy = ProxyConfiguration.Socks5h("proxy.example.com", 1080)
    .WithCredentials("user", "password")
    // DNS always resolved remotely for socks5h
    .ToProxyInfo();

// Use with request
var response = await new Request("https://api.example.com")
    .WithProxy(socks5Proxy)
    .SendAsync();
```

**Provider-Specific Configuration Options:**

| Provider | Option | Description |
|----------|--------|-------------|
| **HTTP/HTTPS** | `WithCredentials()` | Basic authentication |
| | `BypassLocal()` | Skip proxy for localhost/LAN |
| | `WithBypassList()` | Skip proxy for matching patterns |
| **SOCKS4** | `WithUserId()` | User ID (no password support) |
| | `WithConnectionTimeout()` | Connection timeout |
| | `WithReadWriteTimeout()` | Read/write timeout |
| | `WithInternalServerPort()` | Fixed port for internal HTTP listener |
| **SOCKS5** | `WithCredentials()` | Username/password authentication |
| | `ResolveHostnamesLocally()` | DNS resolution location |
| | `WithConnectionTimeout()` | Connection timeout |
| | `WithInternalServerPort()` | Fixed port for internal HTTP listener |
| **SOCKS5h** | Same as SOCKS5 | DNS always resolved remotely |

### Pattern 4: Streaming Large Responses

**For large datasets (NDJSON, logs, CSV), use streaming:**

```csharp
await foreach (string line in response.StreamLinesAsync())
{
    var item = JsonSerializer.Deserialize<LogEntry>(line);
    await ProcessAsync(item);
}
```

### Pattern 5: JSON Path Queries

**When you only need specific fields from large JSON:**

```csharp
// Extract single value
string userId = await response.ParseJsonPathAsync<string>("$.user.id");

// Extract array
List<string> names = await response.ParseJsonPathListAsync<string>("$.users[*].name");

// Extract nested value
decimal price = await response.ParseJsonPathAsync<decimal>("$.product.pricing.amount");
```

### Pattern 5b: Multi-Selector JSON Path Extraction

**Extract multiple values from the same JSON response efficiently with path reuse optimization:**

```csharp
using DevBase.Net.Configuration;
using DevBase.Net.Parsing;

// Standard extraction (no optimization) - disabled by default
var result = await response.ParseMultipleJsonPathsAsync(
    default,
    ("userId", "$.user.id"),
    ("userName", "$.user.name"),
    ("userEmail", "$.user.email"),
    ("city", "$.user.address.city")
);

// Access extracted values
string userId = result.GetString("userId");
string userName = result.GetString("userName");
string userEmail = result.GetString("userEmail");
string city = result.GetString("city");

// Type-safe extraction
int? age = result.GetInt("age");
bool? isActive = result.GetBool("isActive");
double? balance = result.GetDouble("balance");

// Generic extraction
var user = result.Get<UserDto>("user");

// Optimized extraction with path reuse - navigates to $.user once, then extracts multiple fields
var optimizedResult = await response.ParseMultipleJsonPathsOptimizedAsync(
    default,
    ("id", "$.user.id"),
    ("name", "$.user.name"),
    ("email", "$.user.email")  // Shares $.user prefix - only navigates once!
);

// Advanced: Full configuration control
var config = MultiSelectorConfig.CreateOptimized(
    ("productId", "$.data.product.id"),
    ("productName", "$.data.product.name"),
    ("productPrice", "$.data.product.price"),
    ("categoryName", "$.data.category.name")
);
var configResult = await response.ParseMultipleJsonPathsAsync(config);

// Check if value exists
if (configResult.HasValue("productId"))
{
    string id = configResult.GetString("productId");
}

// Iterate over all extracted values
foreach (string name in configResult.Names)
{
    Console.WriteLine($"{name}: {configResult.GetString(name)}");
}
```

**Optimization Behavior:**

- **Disabled by default**: `ParseMultipleJsonPathsAsync()` - No optimization, each path parsed independently
- **Enabled when requested**: `ParseMultipleJsonPathsOptimizedAsync()` - Path reuse optimization enabled
- **Path Reuse**: When multiple selectors share a common prefix (e.g., `$.user.id`, `$.user.name`), the parser navigates to `$.user` once and extracts both values without re-reading the entire path
- **Performance**: Significant improvement for large JSON documents with multiple extractions from the same section

**Configuration Options:**

```csharp
// Create config without optimization (default)
var config = MultiSelectorConfig.Create(
    ("field1", "$.path.to.field1"),
    ("field2", "$.path.to.field2")
);
// OptimizePathReuse = false
// OptimizeProperties = false

// Create config with optimization
var optimizedConfig = MultiSelectorConfig.CreateOptimized(
    ("field1", "$.path.to.field1"),
    ("field2", "$.path.to.field2")
);
// OptimizePathReuse = true
// OptimizeProperties = true
```

### Pattern 6: Retry with Exponential Backoff

**For unreliable APIs or transient errors:**

```csharp
var response = await new Request("https://unreliable-api.com/data")
    .WithRetryPolicy(RetryPolicy.Exponential(
        maxRetries: 3,
        baseDelay: TimeSpan.FromSeconds(1)
    ))
    .SendAsync();
```

### Pattern 7: HTML Scraping

```csharp
var response = await new Request("https://example.com").SendAsync();
IDocument doc = await response.ParseHtmlAsync();

// Query elements
string title = doc.Title;
var links = doc.QuerySelectorAll("a");
var images = doc.QuerySelectorAll("img[src]");

foreach (var link in links)
{
    string href = link.GetAttribute("href");
    string text = link.TextContent;
}
```

### Pattern 8: Form Data Submission

```csharp
var formData = new[]
{
    new FormKeypair("username", "john"),
    new FormKeypair("password", "secret"),
    new FormKeypair("remember", "true")
};

var response = await new Request("https://example.com/login")
    .AsPost()
    .WithFormBody(formData)
    .SendAsync();
```

### Pattern 9: File Upload

```csharp
byte[] fileBytes = await File.ReadAllBytesAsync("document.pdf");

var response = await new Request("https://api.example.com/upload")
    .AsPost()
    .WithRawBody(fileBytes, "application/pdf")
    .WithHeader("X-Filename", "document.pdf")
    .SendAsync();
```

### Pattern 10: Browser Spoofing and Anti-Detection

**Bypass scraping protections by emulating real browsers:**

```csharp
using DevBase.Net.Configuration;
using DevBase.Net.Configuration.Enums;

// Simple Chrome emulation
var response = await new Request("https://protected-site.com")
    .WithScrapingBypass(ScrapingBypassConfig.Default)
    .SendAsync();

// Custom configuration
var config = new ScrapingBypassConfig
{
    BrowserProfile = EnumBrowserProfile.Chrome,
    RefererStrategy = EnumRefererStrategy.SearchEngine
};

var response = await new Request("https://target-site.com")
    .WithScrapingBypass(config)
    .SendAsync();

// Different browser profiles
var firefoxConfig = new ScrapingBypassConfig
{
    BrowserProfile = EnumBrowserProfile.Firefox,
    RefererStrategy = EnumRefererStrategy.BaseHost
};

var edgeConfig = new ScrapingBypassConfig
{
    BrowserProfile = EnumBrowserProfile.Edge,
    RefererStrategy = EnumRefererStrategy.PreviousUrl
};

// User headers always take priority
var response = await new Request("https://api.example.com")
    .WithScrapingBypass(ScrapingBypassConfig.Default)
    .WithUserAgent("MyCustomBot/1.0")  // Overrides Chrome user agent
    .WithHeader("Accept", "application/json")  // Overrides Chrome Accept header
    .SendAsync();
```

**What gets applied:**

- **Chrome Profile**: User-Agent, Accept, Accept-Language, Accept-Encoding, sec-ch-ua headers, sec-fetch-* headers
- **Firefox Profile**: User-Agent, Accept, Accept-Language, Accept-Encoding, DNT, sec-fetch-* headers
- **Edge Profile**: User-Agent, Accept, Accept-Language, Accept-Encoding, sec-ch-ua headers, sec-fetch-* headers
- **Safari Profile**: User-Agent, Accept, Accept-Language, Accept-Encoding

**Referer Strategies:**

```csharp
// No referer
RefererStrategy = EnumRefererStrategy.None

// Use previous URL (for sequential scraping)
RefererStrategy = EnumRefererStrategy.PreviousUrl

// Use base host (e.g., https://example.com/)
RefererStrategy = EnumRefererStrategy.BaseHost

// Random search engine (Google, Bing, DuckDuckGo, Yahoo, Ecosia)
RefererStrategy = EnumRefererStrategy.SearchEngine
```

**Header Priority System:**

User-defined headers **always take priority** over browser spoofing headers. This applies to:

| Method | Priority | Description |
|--------|----------|-------------|
| `WithHeader("User-Agent", ...)` | User > Spoofing | Directly sets header in entries list |
| `WithUserAgent(string)` | User > Spoofing | Uses `UserAgentHeaderBuilder` |
| `WithBogusUserAgent()` | User > Spoofing | Random user agent from built-in generators |
| `WithBogusUserAgent<T>()` | User > Spoofing | Specific bogus user agent generator |
| `WithAccept(...)` | User > Spoofing | Accept header |
| `WithReferer(...)` | User > Spoofing | Referer header |
| All other `WithHeader()` calls | User > Spoofing | Any custom header |

**Example: Custom User-Agent with Browser Spoofing**

```csharp
// Use Chrome browser profile but with custom User-Agent
var response = await new Request("https://api.example.com")
    .WithScrapingBypass(ScrapingBypassConfig.Default)
    .WithBogusUserAgent<BogusFirefoxUserAgentGenerator>()  // Overrides Chrome UA
    .SendAsync();

// Or use a completely custom User-Agent
var response = await new Request("https://api.example.com")
    .WithUserAgent("MyBot/1.0")
    .WithScrapingBypass(new ScrapingBypassConfig 
    { 
        BrowserProfile = EnumBrowserProfile.Chrome,
        RefererStrategy = EnumRefererStrategy.SearchEngine
    })
    .SendAsync();
// Result: User-Agent is "MyBot/1.0", but Chrome's sec-ch-ua and other headers are applied
```

The order of method calls does not matter - user headers are captured before spoofing and re-applied after.

### Pattern 11: Batch Requests with Rate Limiting

**For processing many requests with controlled concurrency:**

```csharp
using DevBase.Net.Core;

// Create batch processor with rate limit of 3 requests per second
// Note: Processing starts automatically on construction
var batchRequests = new BatchRequests()
    .WithRateLimit(3)  // 3 concurrent requests per second
    .WithCookiePersistence()
    .OnResponse(r => Console.WriteLine($"Status: {r.StatusCode}"))
    .OnError((req, ex) => Console.WriteLine($"Error: {ex.Message}"))
    .OnProgress(p => Console.WriteLine($"Progress: {p.PercentComplete:F1}%"));

// Create named batches
var apiBatch = batchRequests.CreateBatch("api-calls");
for (int i = 0; i < 100; i++)
    apiBatch.Enqueue($"https://api.example.com/item/{i}");

var scrapingBatch = batchRequests.CreateBatch("scraping");
for (int i = 0; i < 50; i++)
    scrapingBatch.Enqueue($"https://site.com/page/{i}", r => r.WithBogusUserAgent());

// Execute all batches - sends 3 requests concurrently per second
var responses = await batchRequests.ExecuteAllAsync();

// Or execute specific batch
var apiResponses = await batchRequests.ExecuteBatchAsync("api-calls");

// Stream results as they complete
await foreach (var response in batchRequests.ExecuteAllAsyncEnumerable())
{
    await ProcessAsync(response);
}
```

### Pattern 11: Request Re-queuing with Callbacks

**Automatically retry requests based on response conditions:**

```csharp
using DevBase.Net.Core;

var batchRequests = new BatchRequests()
    .WithRateLimit(5)
    // Re-queue on specific HTTP status codes
    .OnResponseRequeue((response, request) =>
    {
        // Retry on rate limit (429) or server errors (5xx)
        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            return RequeueDecision.Requeue();
        }
        
        if ((int)response.StatusCode >= 500)
        {
            // Modify request before re-queuing
            var modifiedRequest = new Request(request.GetUri()!.ToString())
                .WithHeader("X-Retry", "true");
            return RequeueDecision.RequeueWith(modifiedRequest);
        }
        
        return RequeueDecision.NoRequeue;
    })
    // Re-queue on specific exceptions
    .OnErrorRequeue((request, exception) =>
    {
        // Retry on timeout exceptions
        if (exception is TaskCanceledException || exception is TimeoutException)
        {
            return RequeueDecision.Requeue();
        }
        
        // Retry on network errors
        if (exception is HttpRequestException)
        {
            return RequeueDecision.Requeue();
        }
        
        return RequeueDecision.NoRequeue;
    });

var batch = batchRequests.CreateBatch("resilient");
batch.Enqueue("https://unreliable-api.com/data");

await batchRequests.ExecuteAllAsync();
```

### Pattern 12: Proxied Batch Requests with Rotation

**Process requests through rotating proxies with failure tracking:**

```csharp
using DevBase.Net.Core;
using DevBase.Net.Proxy;

var proxiedBatch = new ProxiedBatchRequests()
    .WithRateLimit(5)
    // Add single proxy
    .WithProxy(new ProxyInfo("proxy1.example.com", 8080))
    // Or add from string
    .WithProxy("socks5://user:pass@proxy2.example.com:1080")
    // Or add multiple
    .WithProxies(new[]
    {
        new ProxyInfo("proxy3.example.com", 8080),
        new ProxyInfo("proxy4.example.com", 8080, "user", "password")
    })
    // Choose rotation strategy
    .WithRoundRobinRotation()  // Default: cycle through proxies
    // OR .WithRandomRotation()        // Random selection
    // OR .WithLeastFailuresRotation() // Prefer healthy proxies
    // OR .WithStickyRotation()        // Stick to one until it fails
    .OnProxyFailure((proxy, context) =>
    {
        Console.WriteLine($"Proxy {proxy.Key} failed: {context.Exception.Message}");
        if (context.ProxyTimedOut)
            Console.WriteLine($"  Proxy timed out after {proxy.MaxFailures} failures");
        Console.WriteLine($"  Remaining available: {context.RemainingAvailableProxies}");
    })
    .OnResponse(r => Console.WriteLine($"Success via proxy"))
    .OnError((req, ex) => Console.WriteLine($"Error: {ex.Message}"));

var batch = proxiedBatch.CreateBatch("scraping");
for (int i = 0; i < 1000; i++)
    batch.Enqueue($"https://target-site.com/page/{i}");

var responses = await proxiedBatch.ExecuteAllAsync();

// Get proxy statistics
var stats = proxiedBatch.GetStatistics();
Console.WriteLine($"Success rate: {stats.SuccessRate:F1}%");
Console.WriteLine($"Proxy availability: {stats.ProxyAvailabilityRate:F1}%");

// Dynamically add more proxies during processing (thread-safe)
proxiedBatch.AddProxy("http://newproxy.example.com:8080");
proxiedBatch.AddProxies(new[] { "socks5://proxy5:1080", "socks5://proxy6:1080" });
```

### Pattern 13: Proxy Rotation Strategies

**Choose the right strategy for your use case:**

```csharp
using DevBase.Net.Core;

// 1. Round Robin (default) - Cycles through proxies in order
var batch = new ProxiedBatchRequests()
    .WithRoundRobinRotation();

// 2. Random - Random proxy selection, good for load balancing
var batch2 = new ProxiedBatchRequests()
    .WithRandomRotation();

// 3. Least Failures - Prefers proxies with fewer failures
// Best for maximizing success rate
var batch3 = new ProxiedBatchRequests()
    .WithLeastFailuresRotation();

// 4. Sticky - Uses same proxy until it fails, then switches
// Good for session-based scraping
var batch4 = new ProxiedBatchRequests()
    .WithStickyRotation();

// 5. Custom Strategy - Implement IProxyRotationStrategy
public class PriorityStrategy : IProxyRotationStrategy
{
    public TrackedProxyInfo? SelectProxy(List<TrackedProxyInfo> proxies, ref int currentIndex)
    {
        // Prefer proxies with specific criteria
        return proxies
            .Where(p => p.IsAvailable())
            .OrderBy(p => p.TotalTimeouts)
            .ThenBy(p => p.FailureCount)
            .FirstOrDefault();
    }
}

var batch5 = new ProxiedBatchRequests()
    .WithRotationStrategy(new PriorityStrategy());
```

### Pattern 14: Simple Concurrent Requests with Multitasking

```csharp
using DevBase.Async.Task;

Multitasking taskManager = new Multitasking(capacity: 5);  // Max 5 concurrent

foreach (var url in urls)
{
    taskManager.Register(async () =>
    {
        var response = await new Request(url).SendAsync();
        await ProcessResponseAsync(response);
    });
}

await taskManager.WaitAll();
```

## Important Concepts

### 1. Response Disposal

**Always dispose Response objects:**

```csharp
// ✅ Correct - using statement
using Response response = await request.SendAsync();

// ✅ Correct - explicit disposal
Response response = await request.SendAsync();
try
{
    // Use response
}
finally
{
    response.Dispose();
}

// ❌ Wrong - memory leak
Response response = await request.SendAsync();
// No disposal
```

### 2. Timeout Configuration

**Default timeout is 30 seconds. Adjust based on use case:**

```csharp
// Quick API calls
request.WithTimeout(TimeSpan.FromSeconds(10));

// File downloads
request.WithTimeout(TimeSpan.FromMinutes(5));

// Long-polling
request.WithTimeout(TimeSpan.FromMinutes(30));

// Streaming (no timeout)
request.WithTimeout(Timeout.InfiniteTimeSpan);
```

### 3. Error Handling

```csharp
try
{
    using Response response = await request.SendAsync();
    
    if (!response.IsSuccessStatusCode)
    {
        string error = await response.GetStringAsync();
        throw new HttpRequestException($"API error: {response.StatusCode} - {error}");
    }
    
    return await response.ParseJsonAsync<T>();
}
catch (TaskCanceledException)
{
    // Timeout occurred
    throw new TimeoutException("Request timed out");
}
catch (HttpRequestException ex)
{
    // Network error
    throw new Exception($"Network error: {ex.Message}", ex);
}
```

### 4. Proxy Chaining

**Chain multiple proxies for enhanced anonymity:**

```csharp
var proxies = new[]
{
    new Socks5ProxyInfo("first.proxy.com", 1080, "user1", "pass1"),
    new Socks5ProxyInfo("second.proxy.com", 1080, "user2", "pass2"),
    new Socks5ProxyInfo("third.proxy.com", 1080)
};

var proxy = new HttpToSocks5Proxy(proxies);
```

### 5. Remote vs Local DNS Resolution

```csharp
var proxy = new HttpToSocks5Proxy(proxyInfo);

// Remote DNS (SOCKS5h) - default, more private
proxy.ResolveHostnamesLocally = false;

// Local DNS - faster but less private
proxy.ResolveHostnamesLocally = true;
```

## Common Mistakes to Avoid

### ❌ Mistake 1: Not Disposing Responses

```csharp
// Wrong
Response response = await request.SendAsync();
string data = await response.GetStringAsync();
// Memory leak!

// Correct
using Response response = await request.SendAsync();
string data = await response.GetStringAsync();
```

### ❌ Mistake 2: Using Wrong Timeout for Operation

```csharp
// Wrong - default 30s timeout for large file download
var response = await new Request("https://cdn.com/large-file.zip").SendAsync();

// Correct
var response = await new Request("https://cdn.com/large-file.zip")
    .WithTimeout(TimeSpan.FromMinutes(10))
    .SendAsync();
```

### ❌ Mistake 3: Not Checking Status Code

```csharp
// Wrong - assumes success
var data = await response.ParseJsonAsync<T>();

// Correct
if (response.IsSuccessStatusCode)
{
    var data = await response.ParseJsonAsync<T>();
}
else
{
    // Handle error
}
```

### ❌ Mistake 4: Creating New HttpClient for Each Request

```csharp
// Wrong - don't do this
using var client = new HttpClient();
var response = await client.GetAsync(url);

// Correct - use Request class
using var response = await new Request(url).SendAsync();
```

### ❌ Mistake 5: Not Using Streaming for Large Data

```csharp
// Wrong - loads entire response into memory
string allData = await response.GetStringAsync();
foreach (string line in allData.Split('\n'))
{
    Process(line);
}

// Correct - streams line by line
await foreach (string line in response.StreamLinesAsync())
{
    Process(line);
}
```

### ❌ Mistake 6: Forgetting to Set Content-Type

```csharp
// Wrong - might fail
request.AsPost().WithRawBody(jsonString);

// Correct
request.AsPost()
    .WithRawBody(jsonString, "application/json")
    .WithHeader("Content-Type", "application/json");

// Better - use WithJsonBody
request.AsPost().WithJsonBody(obj);
```

## Performance Optimization Tips

1. **Reuse Request configuration** for multiple calls to same endpoint
2. **Use connection pooling** (enabled by default)
3. **Stream large responses** instead of loading into memory
4. **Use JSON Path** for extracting specific fields from large JSON
5. **Configure appropriate timeouts** to avoid hanging
6. **Use retry policies** for transient failures
7. **Dispose responses** to release resources quickly
8. **Use ArrayPool** for buffer management (done internally)

## Metrics and Monitoring

```csharp
Response response = await request.SendAsync();
RequestMetrics m = response.Metrics;

// Log performance
Console.WriteLine($"DNS: {m.DnsLookupTime.TotalMilliseconds}ms");
Console.WriteLine($"Connect: {m.ConnectionTime.TotalMilliseconds}ms");
Console.WriteLine($"TLS: {m.TlsHandshakeTime.TotalMilliseconds}ms");
Console.WriteLine($"TTFB: {m.TimeToFirstByte.TotalMilliseconds}ms");
Console.WriteLine($"Total: {m.TotalTime.TotalMilliseconds}ms");
Console.WriteLine($"Downloaded: {m.BytesReceived} bytes");

// Detect slow requests
if (m.TotalTime.TotalSeconds > 5)
{
    Console.WriteLine("Warning: Slow request detected");
}
```

## Integration with Other DevBase Libraries

- **DevBase.Api** - All API clients use `Request` for HTTP calls
- **DevBase** - Uses `AList<T>` for collections
- **DevBase.Format** - Can parse responses with format parsers

## Quick Reference

| Task | Method |
|------|--------|
| GET request | `new Request(url).SendAsync()` |
| POST JSON | `.AsPost().WithJsonBody(obj)` |
| Add header | `.WithHeader(name, value)` |
| Set timeout | `.WithTimeout(TimeSpan)` |
| Use proxy | `.WithProxy(proxyInfo)` or `.WithProxy("socks5://user:pass@host:port")` |
| Retry policy | `.WithRetryPolicy(RetryPolicy.Exponential(3))` |
| Browser spoofing | `.WithScrapingBypass(ScrapingBypassConfig.Default)` |
| Parse JSON | `await response.ParseJsonAsync<T>()` |
| JSON Path | `await response.ParseJsonPathAsync<T>(path)` |
| JSON Path List | `await response.ParseJsonPathListAsync<T>(path)` |
| Multi-selector (no opt) | `await response.ParseMultipleJsonPathsAsync(default, ("name", "$.path")...)` |
| Multi-selector (optimized) | `await response.ParseMultipleJsonPathsOptimizedAsync(default, ("name", "$.path")...)` |
| Parse HTML | `await response.ParseHtmlAsync()` |
| Stream lines | `await foreach (var line in response.StreamLinesAsync())` |
| Get string | `await response.GetStringAsync()` |
| Get bytes | `await response.GetBytesAsync()` |
| Check status | `response.IsSuccessStatusCode` |
| Batch requests | `new BatchRequests().WithRateLimit(3).CreateBatch("name")` |
| Execute batch | `await batchRequests.ExecuteAllAsync()` |
| Re-queue on response | `.OnResponseRequeue((resp, req) => RequeueDecision.Requeue())` |
| Re-queue on error | `.OnErrorRequeue((req, ex) => RequeueDecision.Requeue())` |
| Proxied batch | `new ProxiedBatchRequests().WithProxy(proxyInfo)` |
| Proxy rotation | `.WithRoundRobinRotation()` / `.WithRandomRotation()` |
| Proxy failure callback | `.OnProxyFailure((proxy, ctx) => { })` |

## Testing Considerations

- **Mock responses** using `IResponseInterceptor`
- **Simulate failures** with retry policies
- **Test timeouts** with appropriate delays
- **Verify metrics** are captured correctly
- **Test proxy connections** require actual SOCKS5 server

## Version

Current version: **1.1.0**  
Target framework: **.NET 9.0**

## Dependencies

- Newtonsoft.Json
- AngleSharp
- Serilog
- System.IO.Pipelines
- ZiggyCreatures.FusionCache
