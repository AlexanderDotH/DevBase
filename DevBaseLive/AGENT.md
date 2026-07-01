# DevBaseLive Agent Guide

## Purpose

`DevBaseLive` is a console playground for manual DevBase experiments, mainly current `DevBase.Net.Request` behavior and sample music/Tidal workflows. It is not a package and should not be treated as a production command-line tool.

## Important Files

- `DevBaseLive.csproj`: executable project referencing the main DevBase libraries plus experiment-only packages.
- `Program.cs`: current manual request-building demo with logging, retry, host checks, auth, multipart files, scraping bypass, custom headers, and a webhook target.
- `Tracks/TrackMiner.cs`: sample Tidal search/mining workflow using generated search terms.
- `Objects/Track.cs`: simple track metadata object used by `TrackMiner`.

## Commands

```bash
dotnet restore DevBaseLive/DevBaseLive.csproj
dotnet build DevBaseLive/DevBaseLive.csproj --no-restore -v:minimal
dotnet run --project DevBaseLive/DevBaseLive.csproj
```

## Contributor Notes

- Keep this project out of library API design decisions; it is a scratch/demo harness.
- Do not commit real local file paths, webhook URLs, API tokens, cookies, or account identifiers.
- Prefer environment variables or obvious placeholder values for experiments that need credentials.
- Keep examples compilable even when the demonstrated endpoint is optional or external.
- If an experiment becomes reusable, move it into a library project and cover it with tests.
- Avoid adding dependencies here unless they support a concrete demo or manual verification workflow.
