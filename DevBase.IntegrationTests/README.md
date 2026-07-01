# DevBase Integration Tests

This project contains the integration test package for DevBase. It covers the
deterministic local library checks and the opt-in live API checks in one test
assembly.

The default run avoids external providers and stays deterministic against local
files, in-memory cryptography, and a loopback HTTP test server. Live API tests
perform real web requests and are skipped unless explicitly enabled.

Run:

```bash
dotnet test DevBase.IntegrationTests/DevBase.IntegrationTests.csproj
```

Run live API checks:

```bash
DEVBASE_API_INTEGRATION_LIVE=1 dotnet test DevBase.IntegrationTests/DevBase.IntegrationTests.csproj --filter Category=LiveApi
```

Optional credentials unlock credentialed API checks:

```bash
DEVBASE_DEEZER_ARL=...
DEVBASE_MUSIXMATCH_USER_TOKEN=...
DEVBASE_APPLE_MUSIC_MEDIA_USER_TOKEN=...
DEVBASE_TIDAL_ACCESS_TOKEN=...
```

Current coverage:

- Core typography and `AList` line/chunk flow.
- File IO plus LRC/SRT parsing and lyric conversion.
- `DevBase.Net` request/response parsing against a local HTTP server.
- MD5, Blowfish, BouncyCastle AES, and ECDH round trips.
- Apple Music: web access-token discovery, catalog search, optional lyrics.
- BeautifulLyrics: raw timed lyrics by known ISRC.
- Deezer: public search, unlogged access token, optional ARL-backed JWT.
- Musixmatch: token issue, track search, details, lyrics, snippet, subtitles, richsync, catalog.
- NetEase: search, details, raw lyrics endpoint.
- Tidal: public search, optional access-token lyrics.
