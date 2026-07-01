# DevBase

DevBase is a multi-project .NET 9 solution with reusable utilities, HTTP request infrastructure, music and lyrics API clients, parser packages, cryptography helpers, Avalonia color tools, and test harnesses.

## Project Map

| Project | Purpose |
| --- | --- |
| `DevBase/` | Dependency-free core utilities: `AList`, tuple lists, file/directory wrappers, typography helpers, cache helpers, async/thread helpers, enums, and shared utilities. |
| `DevBase.Net/` | Fluent HTTP client library with `Request`/`Response`, retries, parsing, metrics, browser spoofing, batching, header/JWT validation, and HTTP/SOCKS proxy support. |
| `DevBase.Api/` | Music and lyrics API clients for Deezer, Tidal, Apple Music, NetEase, BeautifulLyrics, and Musixmatch. Provider methods own request construction and parsing. |
| `DevBase.Format/` | Parsers for lyric, subtitle, Apple XML/TTML, Musixmatch JSON-style, MML/RMML, KLyrics, and `.env` formats. |
| `DevBase.Cryptography/` | Local cryptography helpers for Blowfish CBC and MD5 checksum/legacy hashing. |
| `DevBase.Cryptography.BouncyCastle/` | BouncyCastle-backed AES-GCM, ECDH, sealing, random generation, key conversion, and token signature verification helpers. |
| `DevBase.Avalonia/` | Avalonia bitmap color utilities, color math, framebuffer pixel access, and RGB color data. |
| `DevBase.Avalonia.Extension/` | Advanced Avalonia color extraction with LAB/RGB K-means clustering, ImageSharp blur preprocessing, and color filtering/post-processing. |
| `DevBase.Extensions/` | Stopwatch-focused extension package for formatting stopped elapsed-time values as Markdown tables. |
| `DevBase.Logging/` | Minimal Debug-only generic logger with timestamped, type-scoped severity output. |
| `DevBase.Test/` | Main NUnit test project for unit and local integration coverage across the libraries. |
| `DevBase.IntegrationTests/` | Consolidated deterministic integration tests plus opt-in live API checks. |
| `DevBase.Test.Avalonia/` | Manual Avalonia desktop harness for visual color-extraction checks. |
| `DevBaseLive/` | Console playground for manual DevBase experiments, currently focused on `DevBase.Net` request flows and a sample Tidal track miner. |
| `DevBase.Test/DevBaseRequests/Integration/Docker/MockApi/` | Standalone Docker-ready ASP.NET Core mock API for HTTP methods, uploads, auth, retries, rate limits, delays, errors, cookies, streaming, proxy headers, and batch behavior; it has its own `.csproj` and is not included in `DevBase.sln`. |

## Requirements

- .NET SDK 9 or newer. `global.json` allows rolling forward to the latest major SDK.
- Docker only for Docker-backed request integration tests.
- Live API credentials only when explicitly running credentialed provider checks.

## Build

Restore and build the whole solution:

```bash
dotnet restore DevBase.sln
dotnet build DevBase.sln --no-restore -v:minimal
```

Build a single project:

```bash
dotnet build DevBase.Net/DevBase.Net.csproj --no-restore -v:minimal
```

Build the nested mock API separately. It is a standalone support project and is not included in `DevBase.sln`:

```bash
dotnet build DevBase.Test/DevBaseRequests/Integration/Docker/MockApi/MockApi.csproj -v:minimal
```

## Tests

Run the main test project:

```bash
dotnet test DevBase.Test/DevBase.Test.csproj --no-restore -v:minimal
```

Run deterministic integration tests:

```bash
dotnet test DevBase.IntegrationTests/DevBase.IntegrationTests.csproj --no-restore -v:minimal
```

Write JUnit-compatible reports:

```bash
mkdir -p TestResults/junit
dotnet test DevBase.Test/DevBase.Test.csproj --logger:"junit;LogFilePath=TestResults/junit/DevBase.Test.junit.xml;MethodFormat=Class;FailureBodyFormat=Verbose"
dotnet test DevBase.IntegrationTests/DevBase.IntegrationTests.csproj --logger:"junit;LogFilePath=TestResults/junit/DevBase.IntegrationTests.junit.xml;MethodFormat=Class;FailureBodyFormat=Verbose"
```

`TestResults/` is ignored by git.

## Live API Tests

Live provider checks are opt-in:

```bash
DEVBASE_API_INTEGRATION_LIVE=1 dotnet test DevBase.IntegrationTests/DevBase.IntegrationTests.csproj --filter "Category=LiveApi"
```

Optional credential variables:

```bash
DEVBASE_DEEZER_ARL=...
DEVBASE_MUSIXMATCH_USER_TOKEN=...
DEVBASE_APPLE_MUSIC_MEDIA_USER_TOKEN=...
DEVBASE_TIDAL_ACCESS_TOKEN=...
```

Do not commit real tokens, cookies, ARLs, generated credentials, or account-specific data.

## Package Projects

Most library projects generate NuGet packages on build through `GeneratePackageOnBuild=true`. Project README files are packed as package readmes where configured.

Build packages with:

```bash
dotnet build DevBase.sln -c Release -v:minimal
```

## Development Notes

- Prefer project-local `AGENT.md` files for implementation guidance before editing a project.
- Keep `DevBase.Api` request handling per method; `ApiClient` is not a unified send/parse abstraction.
- Keep `DevBase.Net` security guarantees intact: no TLS validation bypasses, hardened XML parsing, and explicit JWT validation behavior.
- Keep default test runs deterministic. External provider outages should skip live tests, not fail unrelated CI.
- Keep generated `bin/`, `obj/`, package files, and test reports out of commits.

## Credits

The `DevBase.Net.Proxy.HttpToSocks5` implementation is based on Miha Zupan's `HttpToSocks5Proxy` library.

- Original author: Miha Zupan
- License: MIT
- Repository: https://github.com/MihaZupan/HttpToSocks5Proxy

<!-- GitAds-Verify: VBE4GZBOLA67HUH4HIX2KZCK1C5NKGAL -->
