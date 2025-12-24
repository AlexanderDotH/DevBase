# DevBase.Net

A modern, high-performance HTTP client library for .NET 9.0 with fluent API, SOCKS5 proxy support, retry policies, and advanced response parsing.

## Features

- Fluent request builder API
- **Browser spoofing and anti-detection** (Chrome, Firefox, Edge, Safari)
- SOCKS5 proxy support with HttpToSocks5Proxy
- Configurable retry policies (linear/exponential backoff)
- JSON, HTML, XML parsing
- JSON Path queries
- Response streaming
- Request/Response interceptors
- Detailed request metrics
- Connection pooling

## Installation

```bash
dotnet add package DevBase.Net
```

## Quick Start

```csharp
using DevBase.Net.Core;

// Simple GET request
var response = await new Request("https://api.example.com/data").SendAsync();
string content = await response.GetStringAsync();

// POST with JSON
var response = await new Request("https://api.example.com/users")
    .AsPost()
    .WithJsonBody(new { name = "John" })
    .SendAsync();
```

## Usage Examples

### HTTP Methods

```csharp
// GET (default)
var response = await new Request(url).SendAsync();

// POST
var response = await new Request(url).AsPost().WithJsonBody(data).SendAsync();

// PUT
var response = await new Request(url).AsPut().WithJsonBody(data).SendAsync();

// DELETE
var response = await new Request(url).AsDelete().SendAsync();
```

### Headers and Authentication

```csharp
var response = await new Request(url)
    .WithHeader("Authorization", "Bearer token")
    .WithHeader("X-Custom", "value")
    .SendAsync();
```

### Response Parsing

```csharp
// JSON
var data = await response.ParseJsonAsync<MyType>();

// JSON Path
var value = await response.ParseJsonPathAsync<string>("$.user.name");

// HTML
var doc = await response.ParseHtmlAsync();

// String
var text = await response.GetStringAsync();
```

### Proxy Support (HTTP/HTTPS/SOCKS4/SOCKS5)

```csharp
using DevBase.Net.Proxy;
using DevBase.Net.Proxy.Enums;

// HTTP Proxy
var httpProxy = new ProxyInfo("proxy.example.com", 8080, EnumProxyType.Http);

// SOCKS5 Proxy with authentication
var socks5Proxy = new ProxyInfo("proxy.example.com", 1080, "user", "pass", EnumProxyType.Socks5);

// SOCKS5h Proxy (remote DNS - more private)
var socks5hProxy = new ProxyInfo("proxy.example.com", 1080, EnumProxyType.Socks5h);

// Parse from string
var proxy = ProxyInfo.Parse("socks5://user:pass@proxy.example.com:1080");

// Use with request
var response = await new Request(url)
    .WithProxy(proxy)
    .SendAsync();
```

### Proxied Batch Requests with Rotation

```csharp
using DevBase.Net.Core;
using DevBase.Net.Proxy;

var proxiedBatch = new ProxiedBatchRequests()
    .WithRateLimit(5)
    .WithProxy("socks5://proxy1.example.com:1080")
    .WithProxy("socks5://proxy2.example.com:1080")
    .WithRoundRobinRotation()  // Or: WithRandomRotation(), WithLeastFailuresRotation()
    .OnProxyFailure((proxy, ctx) => Console.WriteLine($"Proxy {proxy.Key} failed"));

var batch = proxiedBatch.CreateBatch("scraping");
for (int i = 0; i < 100; i++)
    batch.Enqueue($"https://target.com/page/{i}");

var responses = await proxiedBatch.ExecuteAllAsync();
```

### Retry Policies

```csharp
var response = await new Request(url)
    .WithRetryPolicy(RetryPolicy.Exponential(maxRetries: 3))
    .SendAsync();
```

### Browser Spoofing and Anti-Detection

```csharp
using DevBase.Net.Configuration;
using DevBase.Net.Configuration.Enums;

// Simple Chrome emulation with default settings
var response = await new Request("https://protected-site.com")
    .WithScrapingBypass(ScrapingBypassConfig.Default)
    .SendAsync();

// Custom configuration
var config = new ScrapingBypassConfig
{
    Enabled = true,
    BrowserProfile = EnumBrowserProfile.Chrome,
    RefererStrategy = EnumRefererStrategy.SearchEngine
};

var response = await new Request("https://target-site.com")
    .WithScrapingBypass(config)
    .SendAsync();

// User headers always take priority
var response = await new Request("https://api.example.com")
    .WithScrapingBypass(ScrapingBypassConfig.Default)
    .WithUserAgent("MyCustomBot/1.0")  // Overrides Chrome user agent
    .SendAsync();
```

**Available Browser Profiles:**
- `Chrome` - Emulates Google Chrome with client hints
- `Firefox` - Emulates Mozilla Firefox
- `Edge` - Emulates Microsoft Edge
- `Safari` - Emulates Apple Safari

**Referer Strategies:**
- `None` - No referer header
- `PreviousUrl` - Use previous URL (for sequential scraping)
- `BaseHost` - Use base host URL
- `SearchEngine` - Random search engine URL

### Batch Requests with Rate Limiting

```csharp
using DevBase.Net.Core;

// Process many requests with controlled concurrency
var batchRequests = new BatchRequests()
    .WithRateLimit(3)  // 3 concurrent requests per second
    .WithCookiePersistence()
    .OnResponse(r => Console.WriteLine($"Status: {r.StatusCode}"))
    .OnProgress(p => Console.WriteLine($"{p.PercentComplete:F1}% complete"));

// Create batches
var batch = batchRequests.CreateBatch("api-calls");
for (int i = 0; i < 100; i++)
    batch.Enqueue($"https://api.example.com/item/{i}");

// Execute - sends 3 requests concurrently per second
var responses = await batchRequests.ExecuteAllAsync();

// Or stream results
await foreach (var response in batchRequests.ExecuteAllAsyncEnumerable())
{
    // Process as they complete
}
```

## Credits

HttpToSocks5Proxy based on [MihaZupan/HttpToSocks5Proxy](https://github.com/MihaZupan/HttpToSocks5Proxy) (MIT License).

## License

MIT License
