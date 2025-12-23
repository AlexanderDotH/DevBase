# DevBase.Api

DevBase.Api is a comprehensive client library for integrating with various Music, AI, and Lyrics services. It provides a unified and strongly-typed way to interact with these platforms.

## Table of Contents
- [Music Services](#music-services)
  - [Deezer](#deezer)
  - [Tidal](#tidal)
  - [Apple Music](#apple-music)
  - [NetEase Cloud Music](#netease-cloud-music)
  - [Musixmatch](#musixmatch)
- [AI Services](#ai-services)
  - [OpenAI](#openai)
  - [Replicate](#replicate)
- [Lyrics Services](#lyrics-services)
  - [BeautifulLyrics](#beautifullyrics)
  - [OpenLyricsClient](#openlyricsclient)

---

## Music Services

### Deezer
Interact with the Deezer API, including authentication, search, and track details.

**Key Features:**
- **Search**: Tracks, Albums, Artists.
- **Details**: detailed song metadata.
- **Lyrics**: Fetch synchronized lyrics (LRC) via internal APIs (Ajax/Pipe).
- **Download**: (Experimental) Decryption of tracks (Blowfish).

**Initialization:**
```csharp
using DevBase.Api.Apis.Deezer;

// Initialize (ARL token optional, needed for some user-specific features)
var deezer = new Deezer("your_arl_token");
```

**Examples:**
```csharp
// Search for a track
var searchResult = await deezer.Search("Eminem Lose Yourself");

// Get Track Details
var track = await deezer.GetSong("123456789");
Console.WriteLine($"{track.Title} by {track.Artists[0]}");

// Get Lyrics (Synced)
var lyrics = await deezer.GetLyrics("123456789");
foreach(var line in lyrics.TimeStampedLyrics)
{
    Console.WriteLine($"[{line.StartTime}] {line.Text}");
}
```

### Tidal
Interact with the Tidal API. Requires valid Client ID/Secret or Access Tokens.

**Key Features:**
- **Authentication**: OAuth2 Device Flow, Login with credentials (if supported).
- **Search**: High-fidelity track search.
- **Download**: Get stream URLs.

**Initialization:**
```csharp
using DevBase.Api.Apis.Tidal;

var tidal = new Tidal();
```

**Examples:**
```csharp
// Login (using a known access token for session)
var session = await tidal.Login("your_access_token");

// Search
var results = await tidal.Search("The Weeknd", countryCode: "US");

// Get Lyrics
var lyrics = await tidal.GetLyrics("your_access_token", "track_id");
```

### Apple Music
Wrapper for the Apple Music API (Kit).

**Key Features:**
- **Search**: Catalog search using `amp-api`.
- **Lyrics**: Fetch syllable-synced lyrics (requires User Media Token).

**Initialization:**
```csharp
using DevBase.Api.Apis.AppleMusic;

// Initialize with Developer Token
var appleMusic = new AppleMusic("your_developer_token");

// (Optional) Add User Media Token for personalized/restricted endpoints
appleMusic.WithMediaUserToken(myUserToken);
```

**Examples:**
```csharp
// Search
var songs = await appleMusic.Search("Taylor Swift", limit: 5);

// Get Lyrics (Requires User Media Token)
var lyricsResponse = await appleMusic.GetLyrics("song_id");
```

### NetEase Cloud Music
Integration with NetEase Cloud Music (via `music.xianqiao.wang` or similar proxy).

**Key Features:**
- **Search**: Standard keyword search.
- **Lyrics**: Fetch standard and "Karaoke" (K-Lyrics) format.
- **Download**: Get direct download URLs.

**Initialization:**
```csharp
using DevBase.Api.Apis.NetEase;

var netease = new NetEase();
```

**Examples:**
```csharp
// Search
var searchResult = await netease.Search("Anime OST");

// Get Download URL
var urlResponse = await netease.Url("track_id");

// Get Lyrics
var lyrics = await netease.Lyrics("track_id");
```

### Musixmatch
Client for the Musixmatch API.

**Initialization:**
```csharp
using DevBase.Api.Apis.Musixmatch;

var mxm = new MusixMatch();
```

**Examples:**
```csharp
// Login
var auth = await mxm.Login("email", "password");
```

---

## AI Services

### OpenAI
Simple wrapper for OpenAI's API, currently focused on Audio Transcription (Whisper).

**Initialization:**
```csharp
using DevBase.Api.Apis.OpenAi;

var openai = new OpenAi("your_api_key");
```

**Examples:**
```csharp
// Transcribe Audio
byte[] audioData = File.ReadAllBytes("audio.mp3");
var transcription = await openai.Transcribe(audioData);
Console.WriteLine(transcription.text);
```

### Replicate
Client for Replicate.com to run AI models.

**Initialization:**
```csharp
using DevBase.Api.Apis.Replicate;

// Pass a list of tokens to rotate or just one
var replicate = new Replicate(new AList<string>("token1", "token2"));
```

**Examples:**
```csharp
// Run a Prediction
var response = await replicate.Predict(
    modelID: "version_id",
    linkToAudio: "https://...",
    model: "whisper",
    webhook: "https://callback.url"
);
```

---

## Lyrics Services

### BeautifulLyrics
Fetches lyrics from the BeautifulLyrics service (often provides rich sync).

**Examples:**
```csharp
using DevBase.Api.Apis.BeautifulLyrics;

var client = new BeautifulLyrics();

// Get Lyrics by ISRC
var lyrics = await client.GetLyrics("US123456789");
```

### OpenLyricsClient
Client for OpenLyricsClient API, supporting AI synchronization and subscriptions.

**Initialization:**
```csharp
using DevBase.Api.Apis.OpenLyricsClient;

var client = new OpenLyricsClient("server_public_key");
```

**Examples:**
```csharp
// AI Sync
var result = await client.AiSync(subscription, "Title", "Album", durationMs, "model", "Artist");
```
