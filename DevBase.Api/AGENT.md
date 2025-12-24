# DevBase.Api - AI Agent Guide

This guide helps AI agents effectively use the DevBase.Api library for integrating with music streaming services, lyrics providers, and AI platforms.

## Overview

DevBase.Api provides ready-to-use API clients for:
- **Music Services**: Deezer, Tidal, Apple Music, NetEase
- **Lyrics Providers**: BeautifulLyrics, Musixmatch, OpenLyricsClient
- **AI Services**: OpenAI, Replicate

All clients extend `ApiClient` and use DevBase.Net for networking.

**Target Framework:** .NET 9.0  
**Current Version:** 1.0.0

---

## Project Structure

```
DevBase.Api/
├── Apis/
│   ├── ApiClient.cs              # Base class for all API clients
│   ├── AppleMusic/               # Apple Music API (20 files)
│   │   └── AppleMusic.cs
│   ├── BeautifulLyrics/          # BeautifulLyrics API (7 files)
│   │   └── BeautifulLyrics.cs
│   ├── Deezer/                   # Deezer API (83 files)
│   │   └── Deezer.cs
│   ├── Musixmatch/               # Musixmatch API (13 files)
│   │   └── MusixMatch.cs
│   ├── NetEase/                  # NetEase Music API (28 files)
│   │   └── NetEase.cs
│   ├── OpenAi/                   # OpenAI API (3 files)
│   │   └── OpenAi.cs
│   ├── OpenLyricsClient/         # OpenLyricsClient API (8 files)
│   │   └── OpenLyricsClient.cs
│   ├── Replicate/                # Replicate API (11 files)
│   │   └── Replicate.cs
│   └── Tidal/                    # Tidal API (16 files)
│       └── Tidal.cs
├── Enums/                        # API-specific enums
├── Exceptions/                   # API-specific exceptions
└── Serializer/                   # JSON serialization
```

---

## Class Reference

### ApiClient Class (Base)

**Namespace:** `DevBase.Api.Apis`

Base class for all API clients with error handling support.

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `StrictErrorHandling` | `bool` | `false` | Throw exceptions or return null |

#### Protected Methods

```csharp
dynamic Throw<T>(Exception exception)     // Returns null or throws
(string, bool) ThrowTuple(Exception ex)   // Returns ("", false) or throws
```

---

### Deezer Class

**Namespace:** `DevBase.Api.Apis.Deezer`

Deezer music service API client.

#### Constructor

```csharp
Deezer(string arlToken = "")
```

#### Authentication Methods

```csharp
Task<JsonDeezerJwtToken> GetJwtToken()
Task<JsonDeezerAuthTokenResponse> GetAccessToken(string appID = "457142")
Task<JsonDeezerAuthTokenResponse> GetAccessToken(string sessionID, string appID = "457142")
Task<string> GetArlTokenFromSession(string sessionID)
Task<string> GetCsrfToken()
```

#### Search & Retrieve Methods

```csharp
Task<JsonDeezerSearchResponse> Search(string query)
Task<JsonDeezerSearchResponse> Search(string query, int limit, int index = 0)
Task<JsonDeezerTrack> GetSong(string trackID)
Task<JsonDeezerAlbum> GetAlbum(string albumID)
Task<JsonDeezerArtist> GetArtist(string artistID)
```

#### Lyrics Methods

```csharp
Task<(string RawLyrics, AList<TimeStampedLyric> TimeStampedLyrics)> GetLyrics(string trackID)
Task<JsonDeezerRawLyricsResponse> GetLyricsAjax(string trackID)
Task<JsonDeezerRawLyricsGraphResponse> GetLyricsGraph(string trackID)
```

---

### Tidal Class

**Namespace:** `DevBase.Api.Apis.Tidal`

Tidal music service API client with OAuth2 device flow.

#### Constructor

```csharp
Tidal()
```

#### Authentication Methods

```csharp
Task<JsonTidalDeviceAuth> RegisterDevice()
Task<JsonTidalAccountAccess> GetTokenFrom(string deviceCode)
Task<JsonTidalSession> Login(string accessToken)
Task<JsonTidalSession> RefreshSession(string refreshToken)
```

#### Search & Retrieve Methods

```csharp
Task<JsonTidalSearchResponse> Search(string query, string sessionId)
Task<JsonTidalTrack> GetTrack(string trackId, string sessionId)
Task<JsonTidalAlbum> GetAlbum(string albumId, string sessionId)
Task<JsonTidalLyrics> GetLyrics(string trackId, string sessionId)
```

---

### AppleMusic Class

**Namespace:** `DevBase.Api.Apis.AppleMusic`

Apple Music API client.

#### Constructor

```csharp
AppleMusic(string developerToken)
```

#### Methods

```csharp
Task<JsonAppleMusicSearchResponse> SearchCatalog(string storefront, string query)
Task<JsonAppleMusicSong> GetSong(string storefront, string id)
Task<JsonAppleMusicLyrics> GetLyrics(string storefront, string id)
```

---

### NetEase Class

**Namespace:** `DevBase.Api.Apis.NetEase`

NetEase Music API client (Chinese music).

#### Constructor

```csharp
NetEase()
```

#### Methods

```csharp
Task<JsonNetEaseSearchResponse> Search(string query)
Task<JsonNetEaseSongDetail> SongDetail(string id)
Task<JsonNetEaseLyrics> Lyrics(string id)
```

---

### BeautifulLyrics Class

**Namespace:** `DevBase.Api.Apis.BeautifulLyrics`

BeautifulLyrics API for rich-synced lyrics.

#### Constructor

```csharp
BeautifulLyrics()
```

#### Methods

```csharp
Task<(string Lyrics, bool IsRichSync)> GetRawLyrics(string isrc)
```

---

### MusixMatch Class

**Namespace:** `DevBase.Api.Apis.Musixmatch`

Musixmatch lyrics API client.

#### Constructor

```csharp
MusixMatch()
```

#### Methods

```csharp
Task<string> SearchTrack(string title, string artist)
Task<JsonMusixmatchLyrics> GetLyrics(string trackId)
```

---

### OpenAi Class

**Namespace:** `DevBase.Api.Apis.OpenAi`

OpenAI API client.

#### Constructor

```csharp
OpenAi(string apiKey)
```

#### Methods

```csharp
Task<JsonOpenAiChatResponse> ChatCompletion(string model, object[] messages)
```

---

### Replicate Class

**Namespace:** `DevBase.Api.Apis.Replicate`

Replicate AI inference API client.

#### Constructor

```csharp
Replicate(string apiToken)
```

#### Methods

```csharp
Task<JsonReplicatePrediction> CreatePrediction(string model, object input)
Task<JsonReplicatePrediction> GetPrediction(string predictionId)
Task<JsonReplicatePrediction> WaitForPrediction(string predictionId)
```

---

## Core Concept: ApiClient Base Class

All API clients inherit from `ApiClient`:

```csharp
public class ApiClient
{
    public bool StrictErrorHandling { get; set; }
    
    protected dynamic Throw<T>(Exception exception);
    protected (string, bool) ThrowTuple(Exception exception);
}
```

### Error Handling Pattern

**Key Rule:** Always use `Throw<T>()` or `ThrowTuple()` for error handling in API client methods.

```csharp
// For single return values
if (response.StatusCode != HttpStatusCode.OK)
    return Throw<MyType>(new MyException(ExceptionType.NotFound));

// For tuple return values
if (condition_failed)
    return ThrowTuple(new MyException(ExceptionType.Error));
```

**Behavior:**
- `StrictErrorHandling = false` (default): Returns `null` or default value
- `StrictErrorHandling = true`: Throws the exception

## Usage Patterns for AI Agents

### Pattern 1: Basic API Call with Error Handling

```csharp
using DevBase.Api.Apis.Deezer;

var deezer = new Deezer();

// Lenient mode (default)
var track = await deezer.GetSong("123456");
if (track == null)
{
    Console.WriteLine("Track not found or error occurred");
    return;
}

// Strict mode
deezer.StrictErrorHandling = true;
try
{
    var track = await deezer.GetSong("123456");
}
catch (DeezerException ex)
{
    Console.WriteLine($"Error: {ex.ExceptionType}");
}
```

### Pattern 2: Search and Retrieve

```csharp
// Search for tracks
var results = await deezer.Search("Rick Astley Never Gonna Give You Up");

if (results?.data == null || results.data.Length == 0)
{
    Console.WriteLine("No results found");
    return;
}

// Get first result
var firstTrack = results.data[0];
var fullTrack = await deezer.GetSong(firstTrack.id.ToString());
```

### Pattern 3: Authenticated Access (Deezer)

```csharp
// Initialize with ARL token
var deezer = new Deezer("your-arl-token-here");

// Get JWT token
var jwt = await deezer.GetJwtToken();
if (jwt == null)
{
    Console.WriteLine("Invalid ARL token");
    return;
}

// Use authenticated endpoints
// Cookies are automatically managed
```

### Pattern 4: Device Authentication Flow (Tidal)

```csharp
using DevBase.Api.Apis.Tidal;

var tidal = new Tidal();

// Step 1: Register device
var deviceAuth = await tidal.RegisterDevice();
Console.WriteLine($"Visit: {deviceAuth.verificationUriComplete}");
Console.WriteLine($"Or enter code: {deviceAuth.userCode}");

// Step 2: Poll for authorization
JsonTidalAccountAccess access = null;
int maxAttempts = 60;
int attempts = 0;

while (access == null && attempts < maxAttempts)
{
    await Task.Delay(deviceAuth.interval * 1000);
    
    tidal.StrictErrorHandling = false;
    access = await tidal.GetTokenFrom(deviceAuth.deviceCode);
    attempts++;
}

if (access == null)
{
    Console.WriteLine("Authorization timeout");
    return;
}

// Step 3: Login with access token
var session = await tidal.Login(access.access_token);
```

### Pattern 5: Multi-Provider Lyrics Search

```csharp
async Task<string> FindBestLyrics(string isrc, string title, string artist, string trackId)
{
    // Priority 1: BeautifulLyrics (best quality, rich-synced)
    var bl = new BeautifulLyrics();
    var (blLyrics, isRichSync) = await bl.GetRawLyrics(isrc);
    if (blLyrics != null)
    {
        Console.WriteLine($"Found lyrics on BeautifulLyrics (Rich: {isRichSync})");
        return blLyrics;
    }
    
    // Priority 2: Musixmatch
    var mm = new MusixMatch();
    var mmResults = await mm.SearchTrack(title, artist);
    if (mmResults != null)
    {
        Console.WriteLine("Found lyrics on Musixmatch");
        return mmResults;
    }
    
    // Priority 3: Deezer
    var deezer = new Deezer();
    var deezerLyrics = await deezer.GetLyrics(trackId);
    if (deezerLyrics != null)
    {
        Console.WriteLine("Found lyrics on Deezer");
        return deezerLyrics;
    }
    
    // Priority 4: NetEase (for Chinese songs)
    var netease = new NetEase();
    var neSearch = await netease.Search($"{artist} {title}");
    if (neSearch?.result?.songs != null && neSearch.result.songs.Length > 0)
    {
        var neLyrics = await netease.Lyrics(neSearch.result.songs[0].id.ToString());
        if (neLyrics?.lrc?.lyric != null)
        {
            Console.WriteLine("Found lyrics on NetEase");
            return neLyrics.lrc.lyric;
        }
    }
    
    return null;
}
```

### Pattern 6: OpenAI Integration

```csharp
using DevBase.Api.Apis.OpenAi;

var openai = new OpenAi("sk-your-api-key");

var messages = new[]
{
    new { role = "system", content = "You are a music expert." },
    new { role = "user", content = "What genre is this song?" }
};

var response = await openai.ChatCompletion("gpt-4", messages);
string answer = response.choices[0].message.content;
```

### Pattern 7: Batch Processing with Rate Limiting

```csharp
using DevBase.Async.Task;

var deezer = new Deezer();
var trackIds = new[] { "123", "456", "789", /* ... */ };

// Limit to 5 concurrent requests
Multitasking tasks = new Multitasking(capacity: 5);

foreach (var id in trackIds)
{
    tasks.Register(async () =>
    {
        var track = await deezer.GetSong(id);
        if (track != null)
        {
            await ProcessTrackAsync(track);
        }
    });
}

await tasks.WaitAll();
```

## API-Specific Guidelines

### Deezer

**Key Methods:**
- `Search(query)` - Search tracks, albums, artists
- `GetSong(id)` - Get track details
- `GetLyrics(id)` - Get synchronized lyrics
- `GetJwtToken()` - Requires ARL token
- `GetAccessToken()` - Get unlogged access token

**Important:**
- ARL token required for authenticated endpoints
- Cookies are automatically managed
- Supports Blowfish decryption for downloads

### Tidal

**Key Methods:**
- `RegisterDevice()` - Start device auth flow
- `GetTokenFrom(deviceCode)` - Poll for authorization
- `Login(accessToken)` - Get session
- `Search(query, sessionId)` - Search catalog
- `GetLyrics(trackId, sessionId)` - Get lyrics

**Important:**
- Device authentication flow required
- Session ID needed for most operations
- Poll interval specified in device auth response

### Apple Music

**Key Methods:**
- `SearchCatalog(storefront, query)` - Search catalog
- `GetSong(storefront, id)` - Get song details
- `GetLyrics(storefront, id)` - Get lyrics

**Important:**
- Requires developer token
- Storefront code required (e.g., "us", "gb")
- Token must be valid JWT

### NetEase

**Key Methods:**
- `Search(query)` - Search Chinese music
- `SongDetail(id)` - Get song details
- `Lyrics(id)` - Get LRC and karaoke lyrics

**Important:**
- Best for Chinese music
- Returns both standard LRC and karaoke lyrics
- No authentication required

### BeautifulLyrics

**Key Methods:**
- `GetRawLyrics(isrc)` - Get lyrics by ISRC

**Important:**
- Returns tuple: `(lyrics, isRichSync)`
- Rich-synced lyrics include word-level timing
- ISRC is International Standard Recording Code

## Common Mistakes to Avoid

### ❌ Mistake 1: Not Checking for Null

```csharp
// Wrong
var track = await deezer.GetSong("123");
Console.WriteLine(track.title);  // NullReferenceException!

// Correct
var track = await deezer.GetSong("123");
if (track != null)
{
    Console.WriteLine(track.title);
}
```

### ❌ Mistake 2: Using Wrong Error Handling Method

```csharp
// Wrong - in API client implementation
if (error)
    throw new MyException();  // Don't throw directly!

// Correct - in API client implementation
if (error)
    return Throw<MyType>(new MyException(ExceptionType.Error));
```

### ❌ Mistake 3: Not Using ThrowTuple for Tuple Returns

```csharp
// Wrong - in API client returning tuple
if (error)
    return Throw<(string, bool)>(new MyException());  // Won't work!

// Correct
if (error)
    return ThrowTuple(new MyException());
```

### ❌ Mistake 4: Creating New Client for Each Request

```csharp
// Wrong - inefficient
foreach (var id in ids)
{
    var deezer = new Deezer();  // Don't recreate!
    var track = await deezer.GetSong(id);
}

// Correct
var deezer = new Deezer();
foreach (var id in ids)
{
    var track = await deezer.GetSong(id);
}
```

### ❌ Mistake 5: Not Handling Tidal Authorization Pending

```csharp
// Wrong
var access = await tidal.GetTokenFrom(deviceCode);  // Might throw!

// Correct
tidal.StrictErrorHandling = false;
var access = await tidal.GetTokenFrom(deviceCode);
if (access == null)
{
    // Still pending, try again later
}
```

## Creating Custom API Clients

When extending `ApiClient`:

```csharp
using DevBase.Api.Apis;
using DevBase.Api.Exceptions;
using DevBase.Net.Core;

public class MyApiClient : ApiClient
{
    public async Task<MyData> GetData(string id)
    {
        var response = await new Request($"https://api.example.com/data/{id}")
            .WithTimeout(TimeSpan.FromSeconds(30))
            .SendAsync();
        
        // Always check status
        if (!response.IsSuccessStatusCode)
            return Throw<MyData>(new MyApiException("Request failed"));
        
        // Parse response
        var data = await response.ParseJsonAsync<MyData>();
        
        // Validate
        if (data == null)
            return Throw<MyData>(new MyApiException("Invalid response"));
        
        return data;
    }
    
    // For tuple returns
    public async Task<(string, bool)> GetStatus(string id)
    {
        var response = await new Request($"https://api.example.com/status/{id}")
            .SendAsync();
        
        if (!response.IsSuccessStatusCode)
            return ThrowTuple(new MyApiException("Request failed"));
        
        var status = await response.GetStringAsync();
        return (status, true);
    }
}
```

## Integration with DevBase.Format

Many API responses include lyrics that can be parsed:

```csharp
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;

var deezer = new Deezer();
var lyrics = await deezer.GetLyrics("123456");

// Parse LRC format
var parser = new LrcParser();
var parsed = parser.Parse(lyrics);

foreach (var line in parsed)
{
    Console.WriteLine($"[{line.Timestamp}] {line.Text}");
}
```

## Performance Tips

1. **Reuse client instances** across multiple requests
2. **Use lenient mode** for better performance (no exception overhead)
3. **Implement caching** for frequently accessed data
4. **Batch requests** when possible
5. **Use Multitasking** for concurrent API calls with rate limiting

## Quick Reference

| API | Primary Use Case | Authentication |
|-----|-----------------|----------------|
| Deezer | Track search, lyrics, downloads | ARL token (optional) |
| Tidal | High-quality audio, lyrics | Device auth flow |
| Apple Music | Catalog browsing, lyrics | Developer token |
| NetEase | Chinese music, karaoke lyrics | None |
| BeautifulLyrics | Rich-synced lyrics | None |
| Musixmatch | Lyrics search | None |
| OpenAI | Chat completions | API key |
| Replicate | Model inference | API token |

## Testing Considerations

- Use lenient mode (`StrictErrorHandling = false`) for tests
- Mock API responses by extending client classes
- Test both success and failure scenarios
- Consider rate limits in integration tests
- Use test credentials for authenticated APIs

## Version

Current version: **1.5.0**  
Target framework: **.NET 9.0**

## Dependencies

- DevBase.Net
- DevBase
- DevBase.Format
- DevBase.Cryptography
- Newtonsoft.Json
