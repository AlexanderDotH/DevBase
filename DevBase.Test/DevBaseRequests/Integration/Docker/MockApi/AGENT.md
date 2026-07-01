# MockApi Agent Guide

## Purpose

`MockApi` is a standalone ASP.NET Core minimal API used by `DevBase.Test` Docker integration tests. It simulates HTTP behavior needed by `DevBase.Net`: methods, headers, file uploads, auth, retries, rate limits, delays, errors, cookies, streaming, proxy headers, and batch routes.

## Important Files

- `MockApi.csproj`: standalone ASP.NET Core project targeting `net9.0`.
- `Program.cs`: all test endpoints and in-memory state for retry/rate-limit simulation.
- `Dockerfile`: SDK publish stage plus ASP.NET runtime image exposing port `8080`.

## Endpoint Areas

- `/health`: readiness check.
- `/api/get`, `/api/post`, `/api/put`, `/api/delete/{id}`, `/api/patch/{id}`: basic methods.
- `/api/query`, `/api/headers`, `/api/echo-header/{headerName}`: query/header echo behavior.
- `/api/upload`, `/api/upload-single`: multipart upload coverage.
- `/api/rate-limited`, `/api/rate-limit-strict`, `/api/rate-limit/reset`: rate-limit simulation.
- `/api/retry-eventually`, `/api/fail-once`: retry simulation.
- Additional delay, timeout, error, basic/bearer auth, cookie, large-response, streamed-response, proxy-detection, and batch endpoints live in `Program.cs`.

## Commands

```bash
dotnet restore DevBase.Test/DevBaseRequests/Integration/Docker/MockApi/MockApi.csproj
dotnet build DevBase.Test/DevBaseRequests/Integration/Docker/MockApi/MockApi.csproj --no-restore -v:minimal
dotnet run --project DevBase.Test/DevBaseRequests/Integration/Docker/MockApi/MockApi.csproj
```

Docker build from this directory:

```bash
docker build -t devbase-mock-api .
docker run --rm -p 8080:8080 devbase-mock-api
```

## Contributor Notes

- Keep endpoints deterministic and fast; tests depend on predictable status codes and response shapes.
- Reset shared dictionaries or add reset endpoints when adding stateful behavior.
- Avoid external calls, real credentials, or machine-specific paths.
- Preserve existing route names and JSON field names unless tests are updated together.
- This project is excluded from `DevBase.Test` compilation; build it separately after changes.
- Keep the Dockerfile simple and compatible with the test fixture startup path.
- Cap artificial delays and large payloads so failure cases do not hang test runs.
