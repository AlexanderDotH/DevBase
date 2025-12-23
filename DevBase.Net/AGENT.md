# DevBase.Net - AI Agent Guide

This guide helps AI agents effectively use the DevBase.Net HTTP client library.

## Overview

DevBase.Net is the networking backbone of the DevBase solution. It provides a high-performance `Request` class that wraps `HttpClient` with advanced features like SOCKS5 proxying, retry policies, interceptors, and detailed metrics.

**Target Framework:** .NET 9.0

## Core Architecture

### Request Lifecycle

1. **Build** → Create `Request` with fluent API
2. **Configure** → Add headers, body, timeout, retries, proxy
3. **Execute** → Call `SendAsync()`
4. **Intercept** → Request/Response interceptors modify flow
5. **Parse** → Extract data from `Response`
6. **Dispose** → Release resources

### Key Components

| Component | Purpose |
|-----------|---------|
| `Request` | Main entry point, fluent builder, manages execution |
| `Response` | Wraps HttpResponseMessage + MemoryStream, provides parsing |
| `RequestBuilder` | Internal builder for URI, headers, body |
| `HttpToSocks5Proxy` | Local HTTP proxy tunneling to SOCKS5 |
| `RetryPolicy` | Handles transient failures with backoff |
| `RequestMetrics` | Captures precise timing data |
| `IRequestInterceptor` | Middleware for request modification |
| `IResponseInterceptor` | Middleware for response transformation |

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

### Pattern 3: Handling Proxies

**When user mentions proxies, use `HttpToSocks5Proxy`:**

```csharp
using DevBase.Net.Proxy;
using DevBase.Net.Proxy.HttpToSocks5;

// Create proxy
var proxyInfo = new Socks5ProxyInfo("proxy.example.com", 1080, "user", "pass");
var proxy = new HttpToSocks5Proxy(new[] { proxyInfo });

// Use with request
var response = await new Request("https://api.example.com")
    .WithProxy(new TrackedProxyInfo(proxyInfo))
    .SendAsync();

// Cleanup
proxy.Dispose();
```

**Important:** The proxy starts a local HTTP listener on a random port. Always dispose when done.

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
List<string> names = await response.ParseJsonPathAsync<List<string>>("$.users[*].name");

// Extract nested value
decimal price = await response.ParseJsonPathAsync<decimal>("$.product.pricing.amount");
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

### Pattern 10: Concurrent Requests with Rate Limiting

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
| Use proxy | `.WithProxy(TrackedProxyInfo)` |
| Retry policy | `.WithRetryPolicy(RetryPolicy.Exponential(3))` |
| Parse JSON | `await response.ParseJsonAsync<T>()` |
| JSON Path | `await response.ParseJsonPathAsync<T>(path)` |
| Parse HTML | `await response.ParseHtmlAsync()` |
| Stream lines | `await foreach (var line in response.StreamLinesAsync())` |
| Get string | `await response.GetStringAsync()` |
| Get bytes | `await response.GetBytesAsync()` |
| Check status | `response.IsSuccessStatusCode` |

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
