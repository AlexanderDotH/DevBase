# DevBase Agent Guide

This document helps AI agents work effectively with the DevBase solution.

## Solution Overview

DevBase is a multi-project .NET solution providing utilities, API clients, and helpers. The solution targets **.NET 9.0**.

## Project Structure

```
DevBase/
├── DevBase/                    # Core utilities (AList, IO, async tasks)
├── DevBase.Api/                # API clients (Deezer, Tidal, AppleMusic, etc.)
├── DevBase.Avalonia/           # Avalonia UI utilities (color analysis)
├── DevBase.Cryptography/       # Basic cryptography (Blowfish, MD5)
├── DevBase.Cryptography.BouncyCastle/  # BouncyCastle crypto wrappers
├── DevBase.Extensions/         # .NET type extensions
├── DevBase.Format/             # File format parsers (LRC, SRT, ENV)
├── DevBase.Logging/            # Lightweight logging
├── DevBase.Net/                # HTTP client library (main networking)
├── DevBase.Test/               # Unit tests (NUnit)
├── DevBaseLive/                # Console app for testing
└── docs/                       # Documentation
```

## Key Dependencies Between Projects

```
DevBase.Api → DevBase.Net → DevBase → DevBase.Cryptography
DevBase.Format → DevBase
DevBase.Test → All projects
```

## Common Patterns

### 1. HTTP Requests (DevBase.Net)
```csharp
using DevBase.Net.Core;

Response response = await new Request("https://api.example.com")
    .WithHeader("Authorization", "Bearer token")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .SendAsync();

var data = await response.ParseJsonAsync<MyType>();
```

### 2. API Client Error Handling (DevBase.Api)
All API clients extend `ApiClient` and use the `Throw<T>()` method:
```csharp
if (response.StatusCode != HttpStatusCode.OK)
    return Throw<object>(new MyException(ExceptionType.NotFound));
```

For tuple return types, use `ThrowTuple()`:
```csharp
return ThrowTuple(new MyException(ExceptionType.NotFound));
```

### 3. Generic Collections (DevBase)
Use `AList<T>` instead of `List<T>` for enhanced functionality:
```csharp
AList<string> items = new AList<string>();
items.Add("item");
string first = items.Get(0);
bool isEmpty = items.IsEmpty();
```

## Important Classes

| Project | Key Classes |
|---------|-------------|
| DevBase.Net | `Request`, `Response`, `ProxyInfo`, `RetryPolicy` |
| DevBase.Api | `ApiClient`, `Deezer`, `Tidal`, `AppleMusic`, `NetEase` |
| DevBase | `AList<T>`, `AFile`, `AFileObject` |
| DevBase.Format | `LrcParser`, `SrtParser`, `TimeStampedLyric` |

## Namespaces

- **DevBase.Net.Core** - Request/Response classes
- **DevBase.Net.Proxy** - Proxy support
- **DevBase.Api.Apis.[Service]** - API clients
- **DevBase.Generics** - Generic collections
- **DevBase.Format.Formats** - File parsers

## Testing

Run all tests:
```bash
dotnet test
```

Run specific test class:
```bash
dotnet test --filter "FullyQualifiedName~ClassName"
```

## Building NuGet Packages

Packages are generated on build with `GeneratePackageOnBuild=true`:
```bash
dotnet build -c Release
```

Packages output to: `bin/Release/*.nupkg`

## Tips for AI Agents

1. **Use `DevBase.Net.Core.Request`** for HTTP operations, not `HttpClient` directly
2. **Extend `ApiClient`** when creating new API clients
3. **Use `Throw<T>()`** for error handling in API clients
4. **Use `ThrowTuple()`** for methods returning `ValueTuple` types
5. **Check `StrictErrorHandling`** property for exception behavior
6. **Use `AList<T>`** from DevBase.Generics for collections
7. **External API tests** should handle unavailable services gracefully
8. **README.md files** are included in NuGet packages

## Error Handling Pattern

```csharp
// In API client methods
if (condition_failed)
    return Throw<object>(new MyException(ExceptionType.Reason));

// For tuple returns
if (condition_failed)
    return ThrowTuple(new MyException(ExceptionType.Reason));
```

When `StrictErrorHandling = true`, exceptions are thrown.
When `StrictErrorHandling = false`, default values are returned.
