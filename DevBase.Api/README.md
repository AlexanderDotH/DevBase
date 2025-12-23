# DevBase.Api

A comprehensive API client library for .NET providing ready-to-use integrations with popular music and AI services.

## Features

- **Music Services**
  - Apple Music API
  - Deezer API
  - Tidal API
  - Musixmatch API
  - NetEase Music API
- **AI Services**
  - OpenAI API
  - Replicate API
- **Lyrics Services**
  - BeautifulLyrics
  - OpenLyricsClient

## Installation

```xml
<PackageReference Include="DevBase.Api" Version="x.x.x" />
```

Or via NuGet CLI:

```bash
dotnet add package DevBase.Api
```

## Supported APIs

| Service | Description |
|---------|-------------|
| Apple Music | Apple Music streaming service API |
| Deezer | Deezer music streaming API |
| Tidal | Tidal HiFi streaming API |
| Musixmatch | Lyrics and music metadata API |
| NetEase | NetEase Cloud Music API |
| OpenAI | ChatGPT and AI models API |
| Replicate | AI model hosting and inference |
| BeautifulLyrics | Lyrics service |
| OpenLyricsClient | Open lyrics database |

## Usage Examples

### Deezer API

```csharp
using DevBase.Api.Apis.Deezer;

DeezerApi deezer = new DeezerApi();

// Search for tracks
var results = await deezer.SearchAsync("artist name song title");

// Get track details
var track = await deezer.GetTrackAsync(trackId);
```

### Tidal API

```csharp
using DevBase.Api.Apis.Tidal;

TidalApi tidal = new TidalApi(accessToken);

// Search for albums
var albums = await tidal.SearchAlbumsAsync("album name");

// Get track stream URL
var streamUrl = await tidal.GetStreamUrlAsync(trackId);
```

### Musixmatch API

```csharp
using DevBase.Api.Apis.Musixmatch;

MusixmatchApi musixmatch = new MusixmatchApi(apiKey);

// Search for lyrics
var lyrics = await musixmatch.GetLyricsAsync("artist", "song title");

// Get synced lyrics
var syncedLyrics = await musixmatch.GetSyncedLyricsAsync(trackId);
```

### OpenAI API

```csharp
using DevBase.Api.Apis.OpenAi;

OpenAiApi openai = new OpenAiApi(apiKey);

// Chat completion
var response = await openai.ChatAsync("Hello, how are you?");

// With system prompt
var response = await openai.ChatAsync(
    "Translate to German",
    "Hello World"
);
```

### Replicate API

```csharp
using DevBase.Api.Apis.Replicate;

ReplicateApi replicate = new ReplicateApi(apiToken);

// Run a model
var result = await replicate.RunAsync(
    "owner/model:version",
    new { prompt = "input text" }
);

// Get prediction status
var status = await replicate.GetPredictionAsync(predictionId);
```

## Architecture

```
DevBase.Api/
├── Apis/
│   ├── AppleMusic/       # Apple Music integration
│   ├── Deezer/           # Deezer API client
│   ├── Tidal/            # Tidal API client
│   ├── Musixmatch/       # Musixmatch lyrics API
│   ├── NetEase/          # NetEase Cloud Music
│   ├── OpenAi/           # OpenAI ChatGPT
│   ├── Replicate/        # Replicate AI models
│   ├── BeautifulLyrics/  # Lyrics service
│   └── OpenLyricsClient/ # Open lyrics database
├── Enums/                # API enumerations
├── Exceptions/           # API-specific exceptions
└── Serializer/           # JSON serialization helpers
```

## Dependencies

- DevBase.Net (HTTP client)
- DevBase.Format (lyrics parsing)
- DevBase.Cryptography.BouncyCastle (authentication)
- Newtonsoft.Json
- HtmlAgilityPack

## License

MIT License - see LICENSE file for details.
