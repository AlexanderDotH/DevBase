# DevBase.Test Agent Guide

## Purpose

`DevBase.Test` is the main NUnit test project for the DevBase solution. It covers unit and local integration behavior for the core library, API clients, format parsers, cryptography, Avalonia color helpers, and `DevBase.Net` request infrastructure.

## Important Areas

- `DevBase/`: tests for core collections, IO, async helpers, typography, and string utilities.
- `DevBaseApi/`: provider and Musixmatch fixture/contract tests.
- `DevBaseFormat/` and `DevBaseFormatData/`: parser tests and fixture files for LRC, SRT, Apple XML, Musixmatch JSON, and related lyric formats.
- `DevBaseCryptographyBouncyCastle/`: AES, ECDH, and token verifier tests.
- `DevBaseRequests/`: request builder, proxy, batching, parsing, security, and local integration tests.
- `DevBaseRequests/Integration/Docker/`: Docker-backed integration fixtures; the nested `MockApi` project is built separately.
- `Test/`: shared test helpers such as `PenetrationTest`.

## Commands

```bash
dotnet restore DevBase.Test/DevBase.Test.csproj
dotnet test DevBase.Test/DevBase.Test.csproj --no-restore -v:minimal
dotnet test DevBase.Test/DevBase.Test.csproj --filter "FullyQualifiedName~MusixMatch"
dotnet test DevBase.Test/DevBase.Test.csproj --logger:"junit;LogFilePath=TestResults/junit/DevBase.Test.junit.xml;MethodFormat=Class;FailureBodyFormat=Verbose"
```

Build the Docker mock API separately when changing it:

```bash
dotnet build DevBase.Test/DevBaseRequests/Integration/Docker/MockApi/MockApi.csproj -v:minimal
```

## Contributor Notes

- Keep default test runs deterministic. External provider outages should become `Assert.Ignore` or fixture-based tests, not random failures.
- Keep live credentials in environment variables only; never commit tokens, cookies, ARLs, or generated credentials.
- Use focused filters for large local loops, then run the full project before finishing broad changes.
- Keep fixture JSON under the matching provider folder and avoid rewriting fixtures without a schema reason.
- The Docker `MockApi` subtree is excluded from this project compilation; treat it as its own project.
- When adding JUnit output, write it under `TestResults/` so existing gitignore rules keep generated XML out of commits.
- Do not relax security regression tests for TLS validation, XML parsing, JWT validation, or empty Musixmatch data.
