# DevBase.Api

**DevBase.Api** provides a comprehensive collection of API clients for popular music streaming services, AI platforms, and lyrics providers. All clients are built on top of DevBase.Net for high-performance networking.

## Supported APIs

### Music Streaming Services
- **Deezer** - Search, track info, lyrics, song downloads with Blowfish decryption
- **Tidal** - Device authentication, search, lyrics, high-quality downloads
- **Apple Music** - Catalog search, track details, lyrics (requires auth token)
- **NetEase Cloud Music** - Chinese music service with LRC/karaoke lyrics

### Lyrics Providers
- **BeautifulLyrics** - Rich-synced lyrics via ISRC lookup
- **Musixmatch** - Lyrics search and retrieval
- **OpenLyricsClient** - AI-powered lyrics synchronization

### AI Services
- **OpenAI** - Chat completions and GPT models
- **Replicate** - Model inference and predictions

## Installation

```bash
dotnet add package DevBase.Api
```

## Features

- **Unified Error Handling** - Consistent error handling across all clients
- **Strict/Lenient Modes** - Choose between exceptions or null returns
- **Built on DevBase.Net** - High-performance HTTP with retry policies
- **Type-Safe Responses** - Strongly-typed JSON deserialization
- **Cookie Management** - Automatic cookie handling for authenticated APIs

## Quick Start

### Deezer API

```csharp
using DevBase.Api.Apis.Deezer;

// Initialize client
var deezer = new Deezer();

// Search for tracks
var searchResults = await deezer.Search("Never Gonna Give You Up");
var firstTrack = searchResults.data[0];

// Get track details
var track = await deezer.GetSong(firstTrack.id.ToString());
Console.WriteLine($"Title: {track.title}");
Console.WriteLine($"Artist: {track.artist.name}");

// Get lyrics
var lyrics = await deezer.GetLyrics(track.id.ToString());

// With ARL token for authenticated access
var authenticatedDeezer = new Deezer("your-arl-token");
var jwtToken = await authenticatedDeezer.GetJwtToken();
```

### Tidal API

```csharp
using DevBase.Api.Apis.Tidal;

var tidal = new Tidal();

// Device authentication flow
var deviceAuth = await tidal.RegisterDevice();
Console.WriteLine($"Visit: {deviceAuth.verificationUriComplete}");

// Poll for token
JsonTidalAccountAccess access = null;
while (access == null)
{
    await Task.Delay(deviceAuth.interval * 1000);
    try
    {
        access = await tidal.GetTokenFrom(deviceAuth.deviceCode);
    }
    catch { /* Still pending */ }
}

// Login with access token
var session = await tidal.Login(access.access_token);

// Search
var searchResults = await tidal.Search("Rick Astley", session.sessionId);

// Get lyrics
var lyrics = await tidal.GetLyrics(trackId, session.sessionId);
```

### Apple Music API

```csharp
using DevBase.Api.Apis.AppleMusic;

var appleMusic = new AppleMusic("your-developer-token");

// Search catalog
var results = await appleMusic.SearchCatalog("us", "Never Gonna Give You Up");

// Get song details
var song = await appleMusic.GetSong("us", songId);

// Get lyrics
var lyrics = await appleMusic.GetLyrics("us", songId);
```

### NetEase Cloud Music API

```csharp
using DevBase.Api.Apis.NetEase;

var netease = new NetEase();

// Search
var searchResults = await netease.Search("周杰伦");
var firstSong = searchResults.result.songs[0];

// Get lyrics (supports LRC and karaoke formats)
var lyrics = await netease.Lyrics(firstSong.id.ToString());
Console.WriteLine(lyrics.lrc.lyric);  // Standard LRC
Console.WriteLine(lyrics.klyric.lyric);  // Karaoke lyrics

// Get song details
var songDetail = await netease.SongDetail(firstSong.id.ToString());
```

### BeautifulLyrics API

```csharp
using DevBase.Api.Apis.BeautifulLyrics;

var beautifulLyrics = new BeautifulLyrics();

// Get lyrics by ISRC
var (rawLyrics, isRichSync) = await beautifulLyrics.GetRawLyrics("GBAYE0601330");

if (isRichSync)
{
    Console.WriteLine("Rich-synced lyrics available!");
}
```

### Musixmatch API

```csharp
using DevBase.Api.Apis.Musixmatch;

var musixmatch = new MusixMatch();

// Search for lyrics
var results = await musixmatch.SearchTrack("Never Gonna Give You Up", "Rick Astley");

// Get lyrics by track ID
var lyrics = await musixmatch.GetLyrics(trackId);
```

### OpenAI API

```csharp
using DevBase.Api.Apis.OpenAi;

var openai = new OpenAi("your-api-key");

// Chat completion
var messages = new[]
{
    new { role = "system", content = "You are a helpful assistant." },
    new { role = "user", content = "What is the capital of France?" }
};

var response = await openai.ChatCompletion("gpt-4", messages);
Console.WriteLine(response.choices[0].message.content);
```

### Replicate API

```csharp
using DevBase.Api.Apis.Replicate;

var replicate = new Replicate("your-api-token");

// Run a model
var prediction = await replicate.CreatePrediction(
    "stability-ai/sdxl",
    new { prompt = "A beautiful sunset over mountains" }
);

// Check prediction status
var result = await replicate.GetPrediction(prediction.id);
```

## Error Handling

All API clients extend `ApiClient`, providing two error handling modes:

### Lenient Mode (Default)

Returns `null` or default values on errors:

```csharp
var deezer = new Deezer();
var track = await deezer.GetSong("invalid-id");  // Returns null

if (track == null)
{
    Console.WriteLine("Track not found");
}
```

### Strict Mode

Throws typed exceptions on errors:

```csharp
var deezer = new Deezer { StrictErrorHandling = true };

try
{
    var track = await deezer.GetSong("invalid-id");
}
catch (DeezerException ex)
{
    Console.WriteLine($"Deezer error: {ex.ExceptionType}");
}
catch (Exception ex)
{
    Console.WriteLine($"General error: {ex.Message}");
}
```

## Exception Types

Each API has its own exception type:

| API | Exception Type | Enum |
|-----|---------------|------|
| Deezer | `DeezerException` | `EnumDeezerExceptionType` |
| Tidal | `TidalException` | `EnumTidalExceptionType` |
| Apple Music | `AppleMusicException` | `EnumAppleMusicExceptionType` |
| NetEase | `NetEaseException` | `EnumNetEaseExceptionType` |
| BeautifulLyrics | `BeautifulLyricsException` | `EnumBeautifulLyricsExceptionType` |
| OpenLyricsClient | `OpenLyricsClientException` | `EnumOpenLyricsClientExceptionType` |
| Replicate | `ReplicateException` | `EnumReplicateExceptionType` |

## Advanced Usage

### Custom Timeout

```csharp
// Most API methods use Request internally
// You can extend the client to customize behavior
```

### Cookie Persistence (Deezer)

```csharp
var deezer = new Deezer("arl-token");

// Cookies are automatically managed
var token = await deezer.GetJwtToken();
// Subsequent requests use updated cookies
```

### Retry Policies

Since all clients use DevBase.Net, you can implement retry logic:

```csharp
// Clients internally use Request which supports retry policies
// For custom implementations, extend the client class
```

## API Client Base Class

All clients extend `ApiClient`:

```csharp
public class ApiClient
{
    public bool StrictErrorHandling { get; set; }
    
    protected dynamic Throw<T>(Exception exception);
    protected (string, bool) ThrowTuple(Exception exception);
}
```

### Creating Custom API Clients

```csharp
using DevBase.Api.Apis;
using DevBase.Net.Core;

public class MyApiClient : ApiClient
{
    public async Task<MyData> GetData(string id)
    {
        var response = await new Request($"https://api.example.com/data/{id}")
            .SendAsync();
        
        if (!response.IsSuccessStatusCode)
            return Throw<MyData>(new MyApiException("Failed to fetch data"));
        
        return await response.ParseJsonAsync<MyData>();
    }
}
```

## Response Types

All API responses are strongly typed using JSON structure classes:

- `JsonDeezer*` - Deezer response types
- `JsonTidal*` - Tidal response types
- `JsonAppleMusic*` - Apple Music response types
- `JsonNetEase*` - NetEase response types

## Dependencies

- **DevBase.Net** - HTTP client library
- **DevBase** - Core utilities
- **DevBase.Format** - Lyrics format parsing
- **DevBase.Cryptography** - Deezer Blowfish decryption
- **Newtonsoft.Json** - JSON serialization

## Common Patterns

### Pattern 1: Search and Download

```csharp
var deezer = new Deezer("arl-token");

// Search
var results = await deezer.Search("artist - song");
var track = results.data.FirstOrDefault();

// Get full details
var song = await deezer.GetSong(track.id.ToString());

// Download (requires authentication)
var downloadUrl = await deezer.GetDownloadUrl(song);
```

### Pattern 2: Multi-Provider Lyrics Search

```csharp
async Task<string> FindLyrics(string isrc, string title, string artist)
{
    // Try BeautifulLyrics first (best quality)
    var bl = new BeautifulLyrics();
    var (lyrics, isRich) = await bl.GetRawLyrics(isrc);
    if (lyrics != null) return lyrics;
    
    // Try Musixmatch
    var mm = new MusixMatch();
    var mmResults = await mm.SearchTrack(title, artist);
    if (mmResults != null) return mmResults;
    
    // Try Deezer
    var deezer = new Deezer();
    var deezerLyrics = await deezer.GetLyrics(trackId);
    
    return deezerLyrics;
}
```

### Pattern 3: Authenticated API Access

```csharp
// Store tokens securely
string arlToken = LoadFromSecureStorage("deezer_arl");
var deezer = new Deezer(arlToken);

// Use authenticated endpoints
var jwt = await deezer.GetJwtToken();
var userPlaylists = await deezer.GetUserPlaylists(jwt.token);
```

## Performance Tips

1. **Reuse client instances** - Don't create new clients for each request
2. **Enable caching** - Cache search results and track metadata
3. **Use batch operations** - Some APIs support batch requests
4. **Handle rate limits** - Implement exponential backoff for rate-limited APIs
5. **Dispose resources** - Response objects should be disposed

## Testing

```csharp
// Use lenient mode for tests
var client = new Deezer { StrictErrorHandling = false };

// Mock responses by extending the client
public class MockDeezer : Deezer
{
    public override async Task<JsonDeezerSearch> Search(string query)
    {
        return new JsonDeezerSearch { /* mock data */ };
    }
}
```

## Target Framework

- **.NET 9.0**

## License

MIT License - See LICENSE file for details

## Author

AlexanderDotH

## Repository

https://github.com/AlexanderDotH/DevBase
