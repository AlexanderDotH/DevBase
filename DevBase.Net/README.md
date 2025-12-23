# DevBase.Net

A modern, high-performance HTTP client library for .NET 9.0 with fluent API, SOCKS5 proxy support, retry policies, and advanced response parsing.

## Features

- Fluent request builder API
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

### SOCKS5 Proxy

```csharp
using DevBase.Net.Proxy.HttpToSocks5;

var proxy = new HttpToSocks5Proxy("127.0.0.1", 9050);
var response = await new Request(url)
    .WithProxy(proxy)
    .SendAsync();
```

### Retry Policies

```csharp
var response = await new Request(url)
    .WithRetryPolicy(RetryPolicy.Exponential(maxRetries: 3))
    .SendAsync();
```

## Credits

HttpToSocks5Proxy based on [MihaZupan/HttpToSocks5Proxy](https://github.com/MihaZupan/HttpToSocks5Proxy) (MIT License).

## License

MIT License
