# DevBase.Request

A modern, high-performance HTTP client library for .NET with fluent API, proxy support, retry policies, and advanced parsing features.

## Features

- **Fluent API** - Intuitive, chainable method calls
- **Async/Await** - Fully asynchronous implementation
- **Connection Pooling** - Efficient HTTP client reuse
- **Proxy Support** - HTTP, HTTPS, SOCKS4 and SOCKS5 (including proxy chaining)
- **Retry Policies** - Configurable retry strategies with backoff
- **Response Caching** - Built-in caching with SHA256 keys
- **JsonPath Parsing** - Streaming-capable JSON parsing
- **Browser Spoofing** - Realistic user-agent generation
- **Header Validation** - Automatic header validation
- **Request/Response Interceptors** - Middleware pattern
- **Metrics** - Detailed request performance metrics

## Installation

```xml
<PackageReference Include="DevBase.Request" Version="x.x.x" />
```

Or via NuGet CLI:

```bash
dotnet add package DevBase.Request
```

## Quick Start

```csharp
using DevBase.Net.Core;

// Simple GET request
Request request = new Request("https://api.example.com/data");
Response response = await request.SendAsync();
string content = await response.GetStringAsync();

// With Fluent API
Response response = await new Request("https://api.example.com/users")
    .AsGet()
    .WithAcceptJson()
    .WithTimeout(TimeSpan.FromSeconds(10))
    .SendAsync();

MyUser user = await response.ParseJsonAsync<MyUser>();
```

## Basic Usage

### GET Requests

```csharp
Request request = new Request("https://api.example.com/data");
Response response = await request.SendAsync();
```

### POST Requests with JSON

```csharp
MyData data = new MyData { Name = "Test", Value = 42 };

Response response = await new Request("https://api.example.com/create")
    .AsPost()
    .WithJsonBody(data)
    .SendAsync();
```

### Headers and Parameters

```csharp
Response response = await new Request("https://api.example.com/search")
    .WithHeader("X-Custom-Header", "CustomValue")
    .WithParameter("query", "test")
    .WithParameters(("page", "1"), ("limit", "50"))
    .SendAsync();
```

## Response Processing

```csharp
Response response = await request.SendAsync();

// As string
string content = await response.GetStringAsync();

// As bytes
byte[] bytes = await response.GetBytesAsync();

// JSON deserialization
MyClass result = await response.ParseJsonAsync<MyClass>();

// HTML parsing with AngleSharp
IDocument htmlDoc = await response.ParseHtmlAsync();

// JsonPath queries
string name = await response.ParseJsonPathAsync<string>("$.user.name");
```

## Retry Policies

```csharp
Response response = await new Request("https://api.example.com/data")
    .WithRetryPolicy(RetryPolicy.Default)     // 3 retries, Linear Backoff
    .SendAsync();

// Custom policy
RetryPolicy customPolicy = new RetryPolicy
{
    MaxRetries = 5,
    BaseDelay = TimeSpan.FromSeconds(1),
    BackoffStrategy = EnumBackoffStrategy.Exponential
};
```

## Proxy Support

```csharp
// HTTP Proxy
ProxyInfo proxyInfo = new ProxyInfo("proxy.example.com", 8080);

// SOCKS5 Proxy
ProxyInfo socks5Proxy = new ProxyInfo("socks.example.com", 1080, EnumProxyType.Socks5);

Response response = await new Request("https://api.example.com/data")
    .WithProxy(proxyInfo)
    .SendAsync();
```

## Authentication

```csharp
// Basic Authentication
Response response = await new Request("https://api.example.com/protected")
    .UseBasicAuthentication("username", "password")
    .SendAsync();

// Bearer Token
Response response = await new Request("https://api.example.com/protected")
    .UseBearerAuthentication("your-jwt-token-here")
    .SendAsync();
```

## Batch Requests

```csharp
Requests batchRequests = new Requests()
    .WithRateLimit(10, TimeSpan.FromSeconds(1))
    .WithParallelism(5)
    .Add("https://api.example.com/item/1")
    .Add("https://api.example.com/item/2");

List<Response> responses = await batchRequests.SendAllAsync();
```

## Metrics

```csharp
Response response = await request.SendAsync();
RequestMetrics metrics = response.Metrics;

Console.WriteLine($"Total Duration: {metrics.TotalDuration.TotalMilliseconds}ms");
Console.WriteLine($"Time to First Byte: {metrics.TimeToFirstByte.TotalMilliseconds}ms");
```

## License

MIT License - see LICENSE file for details.
