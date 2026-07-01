# DevBase.IntegrationTests Agent Guide

## Purpose

`DevBase.IntegrationTests` is the consolidated NUnit integration test project. It contains deterministic local integration coverage plus opt-in live API checks for music and lyrics providers.

## Important Areas

- `Apis/`: live API tests for Apple Music, BeautifulLyrics, Deezer, Musixmatch, NetEase, and Tidal.
- `Core/`: integration checks for core collections and typography behavior.
- `Cryptography/`: cryptography round-trip coverage across local and BouncyCastle projects.
- `Format/`: file parser integration checks against real fixture data.
- `Net/`: local HTTP integration tests for `DevBase.Net`.
- `Infrastructure/`: `LocalHttpTestServer`, captured request/response helpers, and `LiveApiTestSettings`.
- `GlobalUsings.cs`: NUnit global using and single-test-thread level of parallelism.

## Commands

```bash
dotnet restore DevBase.IntegrationTests/DevBase.IntegrationTests.csproj
dotnet test DevBase.IntegrationTests/DevBase.IntegrationTests.csproj --no-restore -v:minimal
dotnet test DevBase.IntegrationTests/DevBase.IntegrationTests.csproj --filter "Category=LiveApi"
dotnet test DevBase.IntegrationTests/DevBase.IntegrationTests.csproj --logger:"junit;LogFilePath=TestResults/junit/DevBase.IntegrationTests.junit.xml;MethodFormat=Class;FailureBodyFormat=Verbose"
```

Enable live API tests explicitly:

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

## Contributor Notes

- Default runs must avoid real provider calls unless `DEVBASE_API_INTEGRATION_LIVE` or the legacy live switch is enabled.
- Use `LiveApiTestSettings.RequireLiveApiTests()` for every live-provider test.
- Use `LiveApiTestSettings.RequireEnvironmentValue(...)` for credentialed flows; do not invent local config files for secrets.
- Convert provider downtime, empty transient responses, or unsupported regions into `Assert.Ignore`, not false positives.
- Keep local integration tests loopback-only and deterministic.
- Keep network timeouts short and explicit through `LiveApiTestSettings.CreateTimeout`.
- Write JUnit output under `TestResults/` so generated reports stay ignored.
