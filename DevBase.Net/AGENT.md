# DevBase.Net Agent Guide

## Overview
DevBase.Net is a high-performance HTTP client library with fluent API, proxy support, and advanced features.

## Key Classes

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `Request` | `DevBase.Net.Core` | Main HTTP request builder |
| `Response` | `DevBase.Net.Core` | HTTP response wrapper with parsing |
| `ProxyInfo` | `DevBase.Net.Proxy` | Proxy configuration |
| `RetryPolicy` | `DevBase.Net.Core` | Retry strategy configuration |

## Quick Reference

### Basic Request
```csharp
using DevBase.Net.Core;

var response = await new Request("https://api.example.com")
    .SendAsync();
```

### With Headers & Body
```csharp
var response = await new Request("https://api.example.com")
    .AsPost()
    .WithHeader("Authorization", "Bearer token")
    .WithJsonBody(new { key = "value" })
    .WithTimeout(TimeSpan.FromSeconds(30))
    .SendAsync();
```

### Response Parsing
```csharp
// JSON to type
var data = await response.ParseJsonAsync<MyType>();

// String content
string text = await response.GetStringAsync();

// Bytes
byte[] bytes = await response.GetBytesAsync();
```

### Proxy Support
```csharp
using DevBase.Net.Proxy;

var response = await new Request(url)
    .WithProxy(new ProxyInfo("host", 1080, EnumProxyType.Socks5))
    .SendAsync();
```

## File Structure
```
DevBase.Net/
├── Core/
│   ├── Request.cs           # Main request class (partial)
│   ├── RequestConfiguration.cs  # With* methods
│   ├── RequestHttp.cs       # HTTP execution
│   └── Response.cs          # Response wrapper
├── Data/
│   ├── Body/                # Request body builders
│   └── Header/              # Header builders
├── Proxy/                   # Proxy implementations
├── Security/                # JWT/Token handling
└── Utils/                   # Helper utilities
```

## Important Notes

1. **Request is a partial class** split across multiple files
2. **Use `SendAsync()`** to execute requests
3. **Response has `IsSuccessStatusCode`** property for status checks
4. **Use `WithTimeout()`** to set request timeouts
5. **Cookies are handled via `Response.GetCookies()`**

## Common Patterns

### Check Response Status
```csharp
if (!response.IsSuccessStatusCode)
{
    // Handle error
}
```

### With Retry Policy
```csharp
var response = await new Request(url)
    .WithRetryPolicy(new RetryPolicy(maxRetries: 3))
    .SendAsync();
```

### Form Data
```csharp
var response = await new Request(url)
    .AsPost()
    .WithFormBody(new Dictionary<string, string> {
        { "key", "value" }
    })
    .SendAsync();
```
