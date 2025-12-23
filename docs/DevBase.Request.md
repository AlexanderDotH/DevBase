# DevBase.Request

DevBase.Request is a modern, high-performance HTTP client library for .NET. It offers a fluent API, robust proxy support (including SOCKS5 tunneling), retry policies, and advanced response processing capabilities.

## Table of Contents
- [Core Features](#core-features)
- [Basic Usage](#basic-usage)
- [Advanced Usage](#advanced-usage)
  - [Proxy Support](#proxy-support)
  - [Authentication](#authentication)
  - [Batch Requests](#batch-requests)
  - [Metrics](#metrics)

## Core Features
- **Fluent API**: Chainable methods for building requests intuitively.
- **Async/Await**: Built from the ground up for asynchronous operations.
- **Connection Pooling**: Efficient reuse of `HttpClient` instances.
- **Proxy Support**: Native support for HTTP, HTTPS, SOCKS4, and SOCKS5 proxies, including chaining.
- **Retry Policies**: configurable retry strategies with exponential or linear backoff.
- **Response Caching**: Built-in caching mechanism to reduce redundant network calls.

## Basic Usage

### Simple GET
```csharp
using DevBase.Requests.Core;

// Simplest form
Request request = new Request("https://api.example.com/data");
Response response = await request.SendAsync();
string content = await response.GetStringAsync();
```

### Fluent POST with JSON
```csharp
var payload = new { Name = "Test", Value = 42 };

Response response = await new Request("https://api.example.com/create")
    .AsPost()
    .WithJsonBody(payload)
    .WithHeader("X-API-Key", "secret")
    .SendAsync();

if (response.IsSuccessStatusCode)
{
    Console.WriteLine("Success!");
}
```

### Response Parsing
The `Response` object provides several helpers to parse content:
```csharp
// Parse JSON directly to type
MyModel model = await response.ParseJsonAsync<MyModel>();

// Parse specific field via JsonPath
string token = await response.ParseJsonPathAsync<string>("$.auth.token");

// Get raw bytes
byte[] data = await response.GetBytesAsync();
```

## Advanced Usage

### Proxy Support
DevBase.Request includes `HttpToSocks5Proxy`, allowing you to tunnel HTTP traffic through SOCKS5 proxies.

```csharp
using DevBase.Requests.Proxy;

// Standard HTTP Proxy
var httpProxy = new ProxyInfo("1.2.3.4", 8080);

// SOCKS5 Proxy
var socksProxy = new ProxyInfo("5.6.7.8", 1080, EnumProxyType.Socks5);

// Apply to request
var response = await new Request("https://api.ipify.org")
    .WithProxy(socksProxy)
    .SendAsync();
```

### Authentication
Helper methods make adding standard authentication headers easy.

```csharp
// Basic Auth
.UseBasicAuthentication("user", "pass")

// Bearer Token
.UseBearerAuthentication("eyJh...")
```

### Batch Requests
Send multiple requests in parallel with rate limiting.

```csharp
var batch = new Requests()
    .WithRateLimit(requestsPerSecond: 5)
    .WithParallelism(degreeOfParallelism: 3);

batch.Add("https://site.com/1");
batch.Add("https://site.com/2");

List<Response> results = await batch.SendAllAsync();
```

### Metrics
Inspect request performance details.

```csharp
RequestMetrics metrics = response.Metrics;
Console.WriteLine($"Total: {metrics.TotalDuration.TotalMilliseconds}ms");
Console.WriteLine($"TTFB:  {metrics.TimeToFirstByte.TotalMilliseconds}ms");
```
