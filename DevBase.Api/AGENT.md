# DevBase.Api Agent Guide

## Overview
DevBase.Api provides ready-to-use API clients for music streaming services, AI platforms, and lyrics providers.

## Key Classes

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `ApiClient` | `DevBase.Api.Apis` | Base class for all API clients |
| `Deezer` | `DevBase.Api.Apis.Deezer` | Deezer music API |
| `Tidal` | `DevBase.Api.Apis.Tidal` | Tidal music API |
| `AppleMusic` | `DevBase.Api.Apis.AppleMusic` | Apple Music API |
| `NetEase` | `DevBase.Api.Apis.NetEase` | NetEase music API |
| `BeautifulLyrics` | `DevBase.Api.Apis.BeautifulLyrics` | Lyrics provider |

## Error Handling Pattern

All API clients extend `ApiClient` which provides error handling:

```csharp
public class MyApiClient : ApiClient
{
    public async Task<MyResult> GetData()
    {
        if (errorCondition)
            return Throw<object>(new MyException(ExceptionType.Reason));
        
        // Normal return
        return result;
    }
    
    // For tuple return types
    public async Task<(string Data, bool Flag)> GetTuple()
    {
        if (errorCondition)
            return ThrowTuple(new MyException(ExceptionType.Reason));
        
        return (data, true);
    }
}
```

### Error Handling Modes
- `StrictErrorHandling = true` → Exceptions are thrown
- `StrictErrorHandling = false` → Default values returned (null, empty, false)

## Quick Reference

### Deezer
```csharp
var deezer = new Deezer(arlToken: "optional");
var results = await deezer.Search("artist name");
var track = await deezer.GetSong("trackId");
```

### NetEase
```csharp
var netease = new NetEase();
var results = await netease.Search("keyword");
var lyrics = await netease.Lyrics("trackId");
```

### BeautifulLyrics
```csharp
var lyrics = new BeautifulLyrics();
var (rawLyrics, isRichSync) = await lyrics.GetRawLyrics("isrc");
var parsedLyrics = await lyrics.GetLyrics("isrc");
```

### AppleMusic
```csharp
var apple = await AppleMusic.WithAccessToken();
var results = await apple.Search("query");
```

## File Structure
```
DevBase.Api/
├── Apis/
│   ├── ApiClient.cs         # Base class with Throw methods
│   ├── Deezer/
│   │   ├── Deezer.cs
│   │   └── Structure/       # JSON response types
│   ├── Tidal/
│   ├── AppleMusic/
│   ├── NetEase/
│   ├── BeautifulLyrics/
│   └── ...
├── Enums/                   # Exception type enums
├── Exceptions/              # Custom exceptions
└── Serializer/              # JSON deserializer
```

## Important Notes

1. **Always extend `ApiClient`** for new API clients
2. **Use `Throw<object>()`** for reference type returns
3. **Use `ThrowTuple()`** for `ValueTuple` returns
4. **JSON types are in `Structure/Json/` folders**
5. **Use `JsonDeserializer<T>`** for JSON parsing
6. **External APIs may be unavailable** - handle gracefully in tests

## Dependencies
- **DevBase.Net** for HTTP requests
- **DevBase.Format** for lyrics parsing
- **Newtonsoft.Json** for JSON serialization
