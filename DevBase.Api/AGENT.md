# DevBase.Api - AI Agent Guide

This guide describes how to maintain DevBase.Api clients for music services and lyrics providers.

## Overview

DevBase.Api provides ready-to-use API clients for:

- **Music Services**: Deezer, Tidal, Apple Music, NetEase
- **Lyrics Providers**: BeautifulLyrics, Musixmatch

All clients inherit from `ApiClient`, but `ApiClient` is intentionally narrow. It owns only strict/lenient error defaults and helper returns such as `Throw<T>()` and `ThrowTuple()`.

**Target Framework:** .NET 9.0  
**Current Version:** 2.0.0

## Architecture Rule

Each API method owns its request lifecycle:

1. Construct the DevBase.Net `Request` in the provider client.
2. Call `SendAsync` in the method or provider-local helper.
3. Check HTTP status and provider-specific response text.
4. Parse JSON, strings, or bytes in provider-owned code.
5. Use `Throw<T>()` or `ThrowTuple()` only for handled strict/lenient failure behavior.

Do not centralize transport, response buffering, status policy, or parse behavior in `ApiClient`. The reason is operational: a base-client request bug should not be able to break every API surface at once. Request details belong next to the endpoint rules they serve.

Cancellation should remain explicit. Public network methods may accept `CancellationToken cancellationToken = default`; when they do, pass it through to DevBase.Net send/read/parse work. Never convert caller cancellation into a lenient default value.

## Project Structure

```
DevBase.Api/
├── Apis/
│   ├── ApiClient.cs              # Strict/lenient error default base
│   ├── AppleMusic/               # Apple Music API
│   ├── BeautifulLyrics/          # BeautifulLyrics API
│   ├── Deezer/                   # Deezer API
│   ├── Musixmatch/               # Musixmatch API
│   ├── NetEase/                  # NetEase Music API
│   └── Tidal/                    # Tidal API
├── Enums/                        # API-specific enums
├── Exceptions/                   # API-specific exceptions
└── Serializer/                   # JSON serialization helpers
```

## ApiClient Contract

`ApiClient` should stay small:

```csharp
public class ApiClient
{
    public bool StrictErrorHandling { get; set; }

    protected dynamic Throw<T>(Exception exception);
    protected (string, bool) ThrowTuple(Exception exception);
}
```

Behavior:

- `StrictErrorHandling = false` returns `null`, an empty string, an empty byte array, or the tuple default depending on return shape.
- `StrictErrorHandling = true` throws the original exception.
- Provider clients should wrap provider failures in provider-specific exceptions before calling the helper.

## Provider Implementation Pattern

Use this shape inside provider clients:

```csharp
using DevBase.Api.Apis;
using DevBase.Net.Core;
using Newtonsoft.Json;

public sealed class MyApiClient : ApiClient
{
    public async Task<MyResponse?> GetData(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            Request request = new($"https://api.example.com/items/{id}");
            Response response = await request.SendAsync(cancellationToken);
            string content = await response.GetStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return Throw<MyResponse>(new MyApiException("Request failed"));

            if (string.IsNullOrWhiteSpace(content))
                return Throw<MyResponse>(new MyApiException("Empty response"));

            MyResponse? parsed = JsonConvert.DeserializeObject<MyResponse>(content);
            return parsed ?? Throw<MyResponse>(new MyApiException("Invalid JSON"));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Throw<MyResponse>(new MyApiException("Unhandled provider failure", ex));
        }
    }
}
```

Provider-local helpers are acceptable when they are specific to that provider, such as Deezer cookie handling or Tidal token response interpretation. Those helpers should stay in the provider class and should not become a cross-client transport abstraction.

## API-Specific Notes

### Deezer

- ARL token is required for authenticated endpoints.
- Cookie application and cookie updates belong in Deezer-owned code.
- Download paths should validate track metadata and source URL before reading bytes.

### Tidal

- Device authentication has provider-specific pending/error responses.
- Token polling, session login, refresh, lyrics, and downloads should each validate their own response shape.
- Keep country code and sound quality defaults close to the methods that use them.

### Apple Music

- Developer token lookup and authenticated catalog requests have different response expectations.
- Keep token scraping, storefront handling, and catalog parsing in Apple Music-owned code.

### NetEase

- Search, song detail, URL lookup, downloads, and lyrics each have different empty-result rules.
- Validate empty result arrays before returning provider models.

### Lyrics Providers

- BeautifulLyrics should keep rich-sync parsing local to the lyric method.

### Musixmatch

Musixmatch is intentionally larger than the other lyrics clients. Its implementation should track the upstream MusixmatchClientLib feature groups while staying idiomatic for DevBase.Api:

- token issuing and account auth
- track search, track lookup, snippets, lyrics
- subtitles and richsync, including raw and parsed models
- lyric translations and translation submissions
- chart, artist, album, and genre catalog APIs
- user, crowd, score, feedback, suggestion, and Spotify token APIs
- account-changing contribution/profile APIs
- missions through raw DevBase.Net JSON requests

Keep the public surface grouped in partial files, but do not move Musixmatch transport into `ApiClient`. Provider-local helpers such as endpoint definitions, URL signing, context selection, and response-body extraction are acceptable because they are Musixmatch-specific.

The default `new MusixMatch()` constructor auto-issues a Desktop token. Document and preserve this behavior, but avoid using it in loops or tests because repeated token creation can trigger Musixmatch rate limits. Prefer `new MusixMatch(string userToken, MusixMatchApiContext context)` or `new MusixMatch(MusixMatchToken token)` when a caller already has a token.

All new Musixmatch methods should expose async and sync variants. Async is canonical; sync wrappers should call the async method with `.GetAwaiter().GetResult()`.

Musixmatch uses private/undocumented endpoints. Account-changing methods must make that risk visible with XML docs or `[Obsolete]` when the upstream behavior is unstable. These include submissions, profile updates, mission reservations/releases, authentication changes, language/mood updates, and any endpoint that can modify crowd/account state.

Do not add a GraphQL package for missions. Mission calls should use DevBase.Net `Request` with a JSON POST body and Musixmatch-specific parsing.

## Common Mistakes To Avoid

### Bypassing Strict/Lenient Helpers

```csharp
// Wrong in handled provider failures
throw new MyApiException("Not found");

// Correct
return Throw<MyResponse>(new MyApiException("Not found"));
```

### Handling Cancellation As An API Failure

```csharp
// Wrong
catch (OperationCanceledException ex)
{
    return Throw<MyResponse>(ex);
}

// Correct
catch (OperationCanceledException)
{
    throw;
}
```

### Moving Provider Rules Into The Base Class

```csharp
// Wrong: cross-client transport or parsing policy in ApiClient
// Correct: provider method builds, sends, checks, and parses its own request
```

### Recreating Clients For Every Request

```csharp
// Wrong
foreach (var id in ids)
{
    var deezer = new Deezer();
    await deezer.GetSong(id);
}

// Correct
var deezer = new Deezer();
foreach (var id in ids)
{
    await deezer.GetSong(id);
}
```

## Testing Guidance

Request handling tests should stay broad and offline:

- Assert `ApiClient` remains limited to error/default handling.
- Assert provider clients do not reference cross-client request helpers.
- Assert all provider clients construct and send DevBase.Net requests in provider-owned code.
- Test lenient and strict behavior for handled provider failures.
- Test cancellation propagation separately from handled API errors.
- Use fake responses or local fixtures. Avoid live services in normal CI.

Credentialed integration tests should be explicit, rate-limit aware, and skipped unless credentials are intentionally supplied.

Musixmatch tests should default to fixture and source-contract coverage:

- public surface coverage for the MusixmatchClientLib-equivalent method families
- constructor and context behavior
- token request parameter mapping without live token issuance
- URL signing and standard query parameters
- response header/body parsing
- representative DTO parsing for tracks, lyrics, subtitles, richsync, catalog, user/crowd, and missions
- account-changing method body shape without sending live requests

## Performance Guidance

- Reuse client instances.
- Keep provider-specific request tuning close to the endpoint method.
- Pass cancellation tokens through fan-out and UI-triggered workflows.
- Cache stable metadata where it does not change provider semantics.
- Use provider-supported batch endpoints when available.
- Add retries/backoff only where the provider behavior is understood.

## Version

Current version: **2.0.0**
Target framework: **.NET 9.0**

## Dependencies

- DevBase.Net
- DevBase
- DevBase.Format
- DevBase.Cryptography
- Newtonsoft.Json
