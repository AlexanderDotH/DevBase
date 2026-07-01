# DevBase.Api

**DevBase.Api** provides API clients for music streaming services and lyrics providers. Version 2.0.0 uses per-method request handling: each API method builds its own DevBase.Net `Request`, sends it, validates the response, and parses the result locally.

## Supported APIs

### Music Streaming Services
- **Deezer** - Search, track info, lyrics, song downloads with Blowfish decryption
- **Tidal** - Device authentication, search, lyrics, high-quality downloads
- **Apple Music** - Catalog search, track details, lyrics
- **NetEase Cloud Music** - Chinese music service with LRC/karaoke lyrics

### Lyrics Providers
- **BeautifulLyrics** - Rich-synced lyrics via ISRC lookup
- **Musixmatch** - MusixmatchClientLib-equivalent token, auth, search, lyrics, subtitles, richsync, translations, catalog, user/crowd, contribution, and mission APIs

## Installation

```bash
dotnet add package DevBase.Api
```

## Features

- **Per-Method Request Ownership** - Every API method owns request construction, sending, response checks, and parsing.
- **Failure Isolation** - A base-client request bug cannot break every API client because transport logic stays near the endpoint that needs it.
- **Strict/Lenient Error Handling** - Choose between typed exceptions or default/null returns.
- **Cancellation Token Support** - Network methods can accept cancellation tokens and pass them to DevBase.Net.
- **Built on DevBase.Net** - HTTP requests are still performed with the DevBase.Net `Request` API.
- **Type-Safe Responses** - JSON responses are deserialized into provider-specific models.
- **Cookie Management** - Authenticated APIs keep provider-specific cookie behavior in the owning client.
- **Expanded Musixmatch Support** - Musixmatch mirrors the upstream MusixmatchClientLib feature groups while keeping DevBase per-method request ownership.

## Request Handling Rule

Do not add a central send/parse abstraction to `ApiClient`. Each API method should keep the full request lifecycle in the provider client:

1. Build a DevBase.Net `Request` for the endpoint.
2. Call `SendAsync` from the method or a provider-local helper.
3. Check HTTP status and provider-specific response text.
4. Parse JSON, string, or bytes in the same provider-owned flow.
5. Use `Throw<T>()` or `ThrowTuple()` only for the documented strict/lenient failure result.

This keeps behavior easy to debug and prevents one base-class request change from breaking Deezer, Tidal, Apple Music, NetEase, and lyrics providers at the same time.

Cancellation should remain explicit. If a public method accepts `CancellationToken cancellationToken = default`, it should pass that token to request send and response parsing operations. Caller cancellation should surface as cancellation, not as a lenient default value.

## Quick Start

### Deezer API

```csharp
using DevBase.Api.Apis.Deezer;

var deezer = new Deezer();

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
var searchResults = await deezer.Search("Never Gonna Give You Up", cts.Token);
var firstTrack = searchResults.data[0];

var track = await deezer.GetSong(firstTrack.id.ToString(), cts.Token);
Console.WriteLine($"Title: {track.title}");
Console.WriteLine($"Artist: {track.artist.name}");

var lyrics = await deezer.GetLyrics(track.id.ToString(), cts.Token);
```

### Tidal API

```csharp
using DevBase.Api.Apis.Tidal;

var tidal = new Tidal();

var deviceAuth = await tidal.RegisterDevice();
Console.WriteLine($"Visit: {deviceAuth.verificationUriComplete}");

JsonTidalAccountAccess access = null;
while (access == null)
{
    await Task.Delay(deviceAuth.interval * 1000);
    access = await tidal.GetTokenFrom(deviceAuth.deviceCode);
}

var session = await tidal.Login(access.access_token);
var searchResults = await tidal.Search("Rick Astley", session.sessionId);
```

### Apple Music API

```csharp
using DevBase.Api.Apis.AppleMusic;

var appleMusic = new AppleMusic("your-developer-token");

var results = await appleMusic.SearchCatalog("us", "Never Gonna Give You Up");
var song = await appleMusic.GetSong("us", songId);
var lyrics = await appleMusic.GetLyrics("us", songId);
```

### NetEase Cloud Music API

```csharp
using DevBase.Api.Apis.NetEase;

var netease = new NetEase();

var searchResults = await netease.Search("周杰伦");
var firstSong = searchResults.result.songs[0];
var lyrics = await netease.Lyrics(firstSong.id.ToString());
Console.WriteLine(lyrics.lrc.lyric);
Console.WriteLine(lyrics.klyric.lyric);
```

### BeautifulLyrics API

```csharp
using DevBase.Api.Apis.BeautifulLyrics;

var beautifulLyrics = new BeautifulLyrics();
var (rawLyrics, isRichSync) = await beautifulLyrics.GetRawLyrics("GBAYE0601330");
```

### Musixmatch API

The Musixmatch client ports the upstream MusixmatchClientLib surface into DevBase.Api. It supports token issuing, account auth, track search, lyrics, subtitles, richsync, translations, catalog/chart APIs, user/crowd APIs, account-changing contribution methods, and missions.

Musixmatch uses private/undocumented endpoints. These endpoints can change without notice, may be rate-limited, and some methods can modify account/community state. Prefer async methods, pass cancellation tokens, and use account-changing methods only when the caller explicitly requested the action.

```csharp
using DevBase.Api.Apis.Musixmatch;
using DevBase.Api.Apis.Musixmatch.Enums;
using DevBase.Api.Apis.Musixmatch.Structure.Types;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

// Auto-issues a Desktop token. Convenient, but repeated construction can trigger rate limits.
var autoTokenClient = new MusixMatch();

// Preferred for normal apps: provide a known token and context.
var musixmatch = new MusixMatch("your-user-token", MusixMatchApiContext.Desktop);
var fromToken = new MusixMatch(new MusixMatchToken("your-user-token", MusixMatchApiContext.Web));
```

Search and lyrics:

```csharp
var tracks = await musixmatch.SongSearchAsync(
    "Never Gonna Give You Up",
    new PaginationParameters { Page = 1, PageSize = 5 },
    cts.Token);

var firstTrack = tracks[0];
var track = await musixmatch.GetTrackByIdAsync(firstTrack.TrackId, cts.Token);
var lyrics = await musixmatch.GetTrackLyricsAsync(track.TrackId, cts.Token);
var snippet = await musixmatch.GetTrackSnippetAsync(track.TrackId, cts.Token);
var translated = await musixmatch.GetLyricsTranslationAsync(track.TrackId, "de", cts.Token);
```

Advanced search:

```csharp
var filtered = await musixmatch.SongSearchAsync(
    new TrackSearchParameters
    {
        Artist = "Rick Astley",
        Title = "Never Gonna Give You Up",
        Language = "en",
        HasLyrics = true,
        HasSubtitles = true,
        HasRichSync = true
    },
    new PaginationParameters { Page = 1, PageSize = 10 },
    cts.Token);

var lyricMatches = await musixmatch.SongSearchByLyricsAsync("never gonna give you up", cts.Token);
```

Subtitles and richsync:

```csharp
var rawSubtitles = await musixmatch.GetTrackSubtitlesRawAsync(
    track.TrackId,
    MusixMatchSubtitleFormat.Lrc,
    cts.Token);

var subtitles = await musixmatch.GetTrackSubtitlesAsync(track.TrackId, cts.Token);
var rawRichsync = await musixmatch.GetTrackRichsyncRawAsync(track.TrackId, cts.Token);
var richsync = await musixmatch.GetTrackRichsyncAsync(track.TrackId, cts.Token);
```

Catalog, user, crowd, and Spotify token APIs:

```csharp
var chartTrackIds = await musixmatch.ChartTracksGetAsync(cancellationToken: cts.Token);
var artists = await musixmatch.ArtistSearchAsync("Rick Astley", cancellationToken: cts.Token);
var albums = await musixmatch.GetArtistAlbumsAsync(artists[0].ArtistId, cancellationToken: cts.Token);
var genres = await musixmatch.GetMusicGenresAsync(cts.Token);

var user = await musixmatch.GetUserInfoAsync(cts.Token);
var score = await musixmatch.GetUserScoreAsync(cts.Token);
var feedback = await musixmatch.GetCrowdUserFeedbackAsync(cts.Token);
var spotifyToken = await musixmatch.GetSpotifyOauthTokenAsync(cts.Token);
```

Contribution write methods:

```csharp
// These calls may change Musixmatch account/community state.
await musixmatch.AuthenticateMusixMatchAsync("email@example.com", "password", cts.Token);
await musixmatch.SubmitTrackLyricsAsync(track.TrackId, "corrected lyrics", cts.Token);
await musixmatch.SubmitTrackSubtitlesAsync(track.TrackId, subtitles, cts.Token);
await musixmatch.SubmitTrackRichsyncAsync(track.TrackId, richsync, cts.Token);
await musixmatch.SubmitTrackTranslationsRawAsync(
    track.TrackId,
    new List<TranslationPost>
    {
        new()
        {
            OriginalIndex = "0",
            Position = 0,
            Language = "de",
            SourceLine = "Never gonna give you up",
            Translation = "Ich werde dich nie aufgeben"
        }
    },
    cancellationToken: cts.Token);
```

Missions:

```csharp
var missionClient = musixmatch.CreateMissionClient();
var missions = await missionClient.GetMissionsAsync(cts.Token);
var missionTracks = await missionClient.GetMissionTracksAsync(
    missions[0].Id,
    languages: "en",
    destinationLanguage: "de",
    cancellationToken: cts.Token);
```

All Musixmatch methods have async and sync variants. Async is preferred for UI apps, services, and any workflow that needs cancellation or avoids blocking threads.

Existing compatibility:

```csharp
var authResponse = await musixmatch.Login("email@example.com", "password", cts.Token);
var accountToken = await musixmatch.LoginWithMusixMatchAccountAsync("email@example.com", "password", cts.Token);

var syncTracks = musixmatch.SongSearch("Never Gonna Give You Up");
var syncLyrics = musixmatch.GetTrackLyrics(syncTracks[0].TrackId);
```

## Error Handling

All API clients extend `ApiClient`, which should remain a narrow base for strict/lenient failure defaults.

### Lenient Mode

Lenient mode is the default and returns `null` or documented default values on handled API failures:

```csharp
var deezer = new Deezer();
var track = await deezer.GetSong("invalid-id");

if (track == null)
{
    Console.WriteLine("Track not found");
}
```

### Strict Mode

Strict mode throws provider-specific exceptions:

```csharp
var deezer = new Deezer
{
    StrictErrorHandling = true
};

try
{
    var track = await deezer.GetSong("invalid-id");
}
catch (DeezerException ex)
{
    Console.WriteLine($"Deezer error: {ex.ExceptionType}");
}
```

`StrictErrorHandling` controls handled API, transport, response, and parsing failures. It should not swallow caller cancellation; cancelled operations should surface as `OperationCanceledException` or `TaskCanceledException` in both modes.

## Exception Types

| API | Exception Type | Enum |
|-----|---------------|------|
| Deezer | `DeezerException` | `EnumDeezerExceptionType` |
| Tidal | `TidalException` | `EnumTidalExceptionType` |
| Apple Music | `AppleMusicException` | `EnumAppleMusicExceptionType` |
| NetEase | `NetEaseException` | `EnumNetEaseExceptionType` |
| BeautifulLyrics | `BeautifulLyricsException` | `EnumBeautifulLyricsExceptionType` |
| Musixmatch | `MusixMatchRequestException` | `MusixMatchStatusCode` |

## Creating Custom API Clients

```csharp
using DevBase.Api.Apis;
using DevBase.Net.Core;
using Newtonsoft.Json;

public class MyApiClient : ApiClient
{
    public async Task<MyData> GetData(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            Request request = new($"https://api.example.com/data/{id}");
            Response response = await request.SendAsync(cancellationToken);
            string content = await response.GetStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(content))
                return Throw<MyData>(new MyApiException("Invalid response"));

            MyData? data = JsonConvert.DeserializeObject<MyData>(content);
            return data ?? Throw<MyData>(new MyApiException("Invalid JSON"));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Throw<MyData>(new MyApiException("Request failed", ex));
        }
    }
}
```

## Performance Tips

1. **Reuse client instances** across multiple requests.
2. **Keep endpoint logic local** so request behavior can be tuned for each provider.
3. **Pass cancellation tokens** through fan-out and UI-triggered workflows.
4. **Cache stable metadata** such as search results and track details.
5. **Batch requests** when a provider supports it.
6. **Use concurrent calls carefully** with provider-specific rate limits.

## Testing

Request-handling tests should cover every provider client without calling live services:

- `ApiClient` remains limited to strict/lenient defaults.
- Provider clients do not depend on a central request executor or base send/parse helpers.
- Each provider method owns request construction, `SendAsync`, response validation, and parsing.
- Lenient mode returns documented defaults for handled failures.
- Strict mode throws the provider-specific exception for the same failures.
- Cancellation is never converted into a lenient-mode default value.

Credentialed integration tests should be explicit, rate-limit aware, and safe to skip in normal CI.

## Target Framework

- **.NET 9.0**

## License

MIT License - See LICENSE file for details.

## Repository

https://github.com/AlexanderDotH/DevBase
