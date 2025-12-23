# DevBase.Requests

Eine moderne, leistungsstarke HTTP-Client-Bibliothek für .NET mit Fluent API, Proxy-Unterstützung, Retry-Policies und erweiterten Parsing-Funktionen.

## Inhaltsverzeichnis

- [Features](#features)
- [Installation](#installation)
- [Schnellstart](#schnellstart)
- [Grundlegende Verwendung](#grundlegende-verwendung)
  - [Einfache GET-Anfragen](#einfache-get-anfragen)
  - [POST-Anfragen mit JSON](#post-anfragen-mit-json)
  - [Header konfigurieren](#header-konfigurieren)
  - [Query-Parameter](#query-parameter)
- [Response-Verarbeitung](#response-verarbeitung)
  - [String, Bytes und Streams](#string-bytes-und-streams)
  - [JSON-Parsing](#json-parsing)
  - [HTML/XML-Parsing](#htmlxml-parsing)
  - [JsonPath-Parsing](#jsonpath-parsing)
- [Erweiterte Konfiguration](#erweiterte-konfiguration)
  - [Timeout und Cancellation](#timeout-und-cancellation)
  - [Retry-Policies](#retry-policies)
  - [Certificate Validation](#certificate-validation)
  - [Redirects](#redirects)
- [Proxy-Unterstützung](#proxy-unterstützung)
  - [HTTP/HTTPS-Proxy](#httphttps-proxy)
  - [SOCKS4/SOCKS5-Proxy](#socks4socks5-proxy)
  - [Proxy-Chaining](#proxy-chaining)
  - [Proxy-Service](#proxy-service)
- [Authentifizierung](#authentifizierung)
  - [Basic Authentication](#basic-authentication)
  - [Bearer Token](#bearer-token)
- [Body-Builder](#body-builder)
  - [Raw Body](#raw-body)
  - [JSON Body](#json-body)
  - [Form Data](#form-data)
  - [Multipart Form Data](#multipart-form-data)
- [User-Agent Spoofing](#user-agent-spoofing)
  - [Browser-Profile](#browser-profile)
  - [Bogus User-Agents](#bogus-user-agents)
- [Batch-Requests](#batch-requests)
  - [Rate Limiting](#rate-limiting)
  - [Parallelisierung](#parallelisierung)
- [Response-Caching](#response-caching)
- [Interceptors](#interceptors)
- [Metriken](#metriken)
- [Validierung](#validierung)
- [MIME-Types](#mime-types)
- [Architektur](#architektur)
- [Performance-Optimierungen](#performance-optimierungen)
- [API-Referenz](#api-referenz)

---

## Features

- **Fluent API** - Intuitive, verkettbare Methodenaufrufe
- **Async/Await** - Vollständig asynchrone Implementierung
- **Connection Pooling** - Effiziente HTTP-Client-Wiederverwendung
- **Proxy-Support** - HTTP, HTTPS, SOCKS4 und SOCKS5 (inkl. Proxy-Chaining)
- **Retry-Policies** - Konfigurierbare Wiederholungsstrategien mit Backoff
- **Response Caching** - Integriertes Caching mit SHA256-Keys
- **JsonPath-Parsing** - Streaming-fähiges JSON-Parsing
- **Browser Spoofing** - Realistische User-Agent-Generierung
- **Header Validation** - Automatische Header-Validierung
- **Request/Response Interceptors** - Middleware-Pattern
- **Metriken** - Detaillierte Request-Performance-Metriken
- **FrozenDictionary** - Optimierte MIME-Type-Lookups

---

## Installation

```xml
<PackageReference Include="DevBase.Requests" Version="x.x.x" />
```

Oder via NuGet CLI:

```bash
dotnet add package DevBase.Requests
```

---

## Schnellstart

```csharp
using DevBase.Requests.Core;

// Einfache GET-Anfrage
Request request = new Request("https://api.example.com/data");
Response response = await request.SendAsync();
string content = await response.GetStringAsync();

// Mit Fluent API
Response response = await new Request("https://api.example.com/users")
    .AsGet()
    .WithAcceptJson()
    .WithTimeout(TimeSpan.FromSeconds(10))
    .SendAsync();

MyUser user = await response.ParseJsonAsync<MyUser>();
```

---

## Grundlegende Verwendung

### Einfache GET-Anfragen

```csharp
// Variante 1: Konstruktor mit URL
Request request = new Request("https://api.example.com/data");
Response response = await request.SendAsync();

// Variante 2: Factory-Methode
Response response = await Request.Create("https://api.example.com/data")
    .SendAsync();

// Variante 3: URL nachträglich setzen
Response response = await new Request()
    .WithUrl("https://api.example.com/data")
    .SendAsync();
```

### POST-Anfragen mit JSON

```csharp
// Mit Objekt-Serialisierung
MyData data = new MyData { Name = "Test", Value = 42 };

Response response = await new Request("https://api.example.com/create")
    .AsPost()
    .WithJsonBody(data)
    .SendAsync();

// Mit Raw JSON-String
Response response = await new Request("https://api.example.com/create")
    .AsPost()
    .WithJsonBody("{\"name\": \"Test\", \"value\": 42}")
    .SendAsync();
```

### Header konfigurieren

```csharp
Response response = await new Request("https://api.example.com/data")
    .WithHeader("X-Custom-Header", "CustomValue")
    .WithHeader("X-Api-Version", "2.0")
    .WithAccept("application/json", "text/plain")
    .WithUserAgent("MyApp/1.0")
    .SendAsync();
```

### Query-Parameter

```csharp
// Einzelner Parameter
Response response = await new Request("https://api.example.com/search")
    .WithParameter("query", "test")
    .SendAsync();

// Mehrere Parameter
Response response = await new Request("https://api.example.com/search")
    .WithParameters(
        ("query", "test"),
        ("page", "1"),
        ("limit", "50")
    )
    .SendAsync();

// Mit ParameterBuilder
ParameterBuilder builder = new ParameterBuilder();
builder.AddParameter("query", "test");
builder.AddParameters(("page", "1"), ("limit", "50"));

Response response = await new Request("https://api.example.com/search")
    .WithParameters(builder)
    .SendAsync();
```

---

## Response-Verarbeitung

### String, Bytes und Streams

```csharp
Response response = await request.SendAsync();

// Als String
string content = await response.GetStringAsync();

// Als Bytes
byte[] bytes = await response.GetBytesAsync();

// Als Stream
Stream stream = response.GetStream();

// Mit spezifischer Encoding
string content = await response.GetStringAsync(Encoding.UTF8);
```

### JSON-Parsing

```csharp
// Generische Deserialisierung (System.Text.Json)
MyClass result = await response.ParseJsonAsync<MyClass>();

// Mit Newtonsoft.Json
MyClass result = await response.ParseJsonAsync<MyClass>(useSystemTextJson: false);

// Als JsonDocument
JsonDocument doc = await response.ParseJsonDocumentAsync();
```

### HTML/XML-Parsing

```csharp
// HTML-Parsing mit AngleSharp
IDocument htmlDoc = await response.ParseHtmlAsync();
IElement element = htmlDoc.QuerySelector(".my-class");

// XML-Parsing
XDocument xmlDoc = await response.ParseXmlAsync();
```

### JsonPath-Parsing

```csharp
// Einzelwert extrahieren
string name = await response.ParseJsonPathAsync<string>("$.user.name");

// Liste extrahieren
List<int> ids = await response.ParseJsonPathListAsync<int>("$.items[*].id");

// Direkt mit JsonPathParser
JsonPathParser parser = new JsonPathParser();
MyData data = parser.Parse<MyData>(jsonBytes, "$.result.data");

// Streaming-Parser für große Dateien
StreamingJsonPathParser streamParser = new StreamingJsonPathParser();
await foreach (MyItem item in streamParser.ParseStreamAsync<MyItem>(stream, "$.items[*]"))
{
    // Verarbeite jedes Item einzeln
}
```

**Unterstützte JsonPath-Syntax:**
- `$.property` - Objekteigenschaft
- `$[0]` - Array-Index
- `$[*]` - Alle Array-Elemente
- `$.parent.child` - Verschachtelte Eigenschaften
- `$..property` - Rekursive Suche

---

## Erweiterte Konfiguration

### Timeout und Cancellation

```csharp
using CancellationTokenSource cts = new CancellationTokenSource();

Response response = await new Request("https://api.example.com/data")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithCancellationToken(cts.Token)
    .SendAsync();
```

### Retry-Policies

```csharp
// Vordefinierte Policies
Response response = await new Request("https://api.example.com/data")
    .WithRetryPolicy(RetryPolicy.Default)     // 3 Retries, Linear Backoff
    .SendAsync();

Response response = await new Request("https://api.example.com/data")
    .WithRetryPolicy(RetryPolicy.Aggressive)  // 5 Retries, Exponential Backoff
    .SendAsync();

Response response = await new Request("https://api.example.com/data")
    .WithRetryPolicy(RetryPolicy.None)        // Keine Retries
    .SendAsync();

// Benutzerdefinierte Policy
RetryPolicy customPolicy = new RetryPolicy
{
    MaxRetries = 5,
    BaseDelay = TimeSpan.FromSeconds(1),
    BackoffStrategy = EnumBackoffStrategy.Exponential,
    RetryOnTimeout = true,
    RetryOnNetworkError = true,
    RetryOnProxyError = true
};

Response response = await new Request("https://api.example.com/data")
    .WithRetryPolicy(customPolicy)
    .SendAsync();
```

**Backoff-Strategien:**
- `Linear` - Konstante Wartezeit: `BaseDelay`
- `Exponential` - Exponentiell: `BaseDelay * 2^attempt`
- `Jittered` - Exponentiell mit Zufallsvariation

### Certificate Validation

```csharp
// Zertifikatvalidierung deaktivieren (nur für Entwicklung!)
Response response = await new Request("https://self-signed.example.com")
    .WithCertificateValidation(false)
    .SendAsync();
```

### Redirects

```csharp
// Redirects deaktivieren
Response response = await new Request("https://api.example.com/redirect")
    .WithFollowRedirects(false)
    .SendAsync();

// Maximale Redirects begrenzen
Response response = await new Request("https://api.example.com/redirect")
    .WithFollowRedirects(true, maxRedirects: 5)
    .SendAsync();
```

---

## Proxy-Unterstützung

### HTTP/HTTPS-Proxy

```csharp
// Proxy-Info erstellen
ProxyInfo proxyInfo = new ProxyInfo("proxy.example.com", 8080);

// Mit Authentifizierung
ProxyInfo proxyInfo = new ProxyInfo(
    "proxy.example.com", 
    8080, 
    EnumProxyType.Http,
    new NetworkCredential("user", "password")
);

// Proxy verwenden
Response response = await new Request("https://api.example.com/data")
    .WithProxy(proxyInfo)
    .SendAsync();
```

### SOCKS4/SOCKS5-Proxy

```csharp
// SOCKS5-Proxy
ProxyInfo socks5Proxy = new ProxyInfo(
    "socks.example.com", 
    1080, 
    EnumProxyType.Socks5
);

// SOCKS5h (Remote DNS Resolution)
ProxyInfo socks5hProxy = new ProxyInfo(
    "socks.example.com", 
    1080, 
    EnumProxyType.Socks5h
);

Response response = await new Request("https://api.example.com/data")
    .WithProxy(socks5Proxy)
    .SendAsync();
```

### Proxy aus String parsen

```csharp
// Verschiedene Formate
ProxyInfo proxy1 = ProxyInfo.Parse("http://proxy.example.com:8080");
ProxyInfo proxy2 = ProxyInfo.Parse("socks5://user:pass@proxy.example.com:1080");
ProxyInfo proxy3 = ProxyInfo.Parse("192.168.1.1:8080"); // Defaultet zu HTTP
```

### Proxy-Chaining

```csharp
// HTTP-zu-SOCKS5 Proxy (für HttpClient-Kompatibilität)
HttpToSocks5Proxy chainedProxy = new HttpToSocks5Proxy(
    "socks-proxy.example.com", 
    1080,
    "username",
    "password"
);

// Mehrere Proxies verketten
Socks5ProxyInfo[] proxyChain = new[]
{
    new Socks5ProxyInfo("first-proxy.example.com", 1080),
    new Socks5ProxyInfo("second-proxy.example.com", 1080, "user", "pass")
};

HttpToSocks5Proxy chainedProxy = new HttpToSocks5Proxy(proxyChain);
```

### Proxy-Service

```csharp
// Proxy-Pool verwalten
List<TrackedProxyInfo> proxies = new List<TrackedProxyInfo>
{
    new TrackedProxyInfo(ProxyInfo.Parse("http://proxy1.example.com:8080")),
    new TrackedProxyInfo(ProxyInfo.Parse("http://proxy2.example.com:8080")),
    new TrackedProxyInfo(ProxyInfo.Parse("http://proxy3.example.com:8080"))
};

ProxyService proxyService = new ProxyService(proxies);

// Nächsten verfügbaren Proxy holen (Round-Robin)
TrackedProxyInfo? proxy = proxyService.GetNextProxy();

// Zufälligen Proxy holen
TrackedProxyInfo? randomProxy = proxyService.GetRandomAvailableProxy();

// Statistiken abrufen
ProxyTimeoutStats stats = proxyService.GetStats();
Console.WriteLine($"Aktiv: {stats.ActiveCount}, Timeout: {stats.TimedOutCount}");
```

---

## Authentifizierung

### Basic Authentication

```csharp
Response response = await new Request("https://api.example.com/protected")
    .UseBasicAuthentication("username", "password")
    .SendAsync();
```

### Bearer Token

```csharp
Response response = await new Request("https://api.example.com/protected")
    .UseBearerAuthentication("your-jwt-token-here")
    .SendAsync();
```

### JWT-Authentifizierung

```csharp
using DevBase.Requests.Security.Token;

// Mit JWT-Token-String (wird automatisch validiert)
Response response = await new Request("https://api.example.com/protected")
    .UseJwtAuthentication("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")
    .SendAsync();

// Mit AuthenticationToken-Objekt
AuthenticationToken? token = AuthenticationToken.FromString(jwtString);
Response response = await new Request("https://api.example.com/protected")
    .UseJwtAuthentication(token)
    .SendAsync();

// JWT-Token parsen und Claims zugreifen
AuthenticationToken? jwt = AuthenticationToken.FromString(jwtString);
Console.WriteLine($"Subject: {jwt?.Payload.Subject}");
Console.WriteLine($"Issuer: {jwt?.Payload.Issuer}");
Console.WriteLine($"Expires: {jwt?.Payload.ExpiresAt}");

// JWT mit Signatur-Verifizierung
AuthenticationToken? verified = AuthenticationToken.FromString(
    jwtString, 
    verifyToken: true, 
    tokenSecret: "your-secret-key");

if (verified?.Signature.Verified == true)
    Console.WriteLine("Token signature is valid!");
```

**Unterstützte Algorithmen:**
- **HMAC**: HS256, HS384, HS512
- **RSA**: RS256, RS384, RS512
- **ECDSA**: ES256, ES384, ES512
- **RSA-PSS**: PS256, PS384, PS512

---

## Body-Builder

### Raw Body

```csharp
// Text-Body
Response response = await new Request("https://api.example.com/data")
    .AsPost()
    .WithRawBody("Raw text content")
    .SendAsync();

// Mit RequestRawBodyBuilder
RequestRawBodyBuilder bodyBuilder = new RequestRawBodyBuilder();
bodyBuilder.WithText("Custom content", Encoding.UTF8);

Response response = await new Request("https://api.example.com/data")
    .AsPost()
    .WithRawBody(bodyBuilder)
    .SendAsync();

// Binary-Body
byte[] buffer = File.ReadAllBytes("data.bin");
Response response = await new Request("https://api.example.com/upload")
    .AsPost()
    .WithBufferBody(buffer)
    .SendAsync();
```

### JSON Body

```csharp
// Mit Objekt
MyData data = new MyData { Name = "Test" };
Response response = await new Request("https://api.example.com/data")
    .AsPost()
    .WithJsonBody(data)
    .SendAsync();

// Mit JSON-String
Response response = await new Request("https://api.example.com/data")
    .AsPost()
    .WithJsonBody("{\"name\": \"Test\"}")
    .SendAsync();
```

### Form Data

```csharp
// URL-encoded Form
Response response = await new Request("https://api.example.com/login")
    .AsPost()
    .WithEncodedForm(
        ("username", "user"),
        ("password", "pass")
    )
    .SendAsync();

// Mit Builder
RequestEncodedKeyValueListBodyBuilder formBuilder = new RequestEncodedKeyValueListBodyBuilder();
formBuilder.AddText("username", "user");
formBuilder.AddText("password", "pass");

Response response = await new Request("https://api.example.com/login")
    .AsPost()
    .WithEncodedForm(formBuilder)
    .SendAsync();
```

### Multipart Form Data

```csharp
RequestKeyValueListBodyBuilder multipartBuilder = new RequestKeyValueListBodyBuilder();

// Text-Felder
multipartBuilder.AddText("title", "My Document");
multipartBuilder.AddText("description", "A sample file upload");

// Datei-Upload
byte[] fileContent = File.ReadAllBytes("document.pdf");
multipartBuilder.AddFile("document", "document.pdf", fileContent);

Response response = await new Request("https://api.example.com/upload")
    .AsPost()
    .WithForm(multipartBuilder)
    .SendAsync();
```

---

## User-Agent Spoofing

### Browser-Profile

```csharp
using DevBase.Requests.Spoofing;
using DevBase.Requests.Configuration.Enums;

Request request = new Request("https://example.com");

// Browser-Profil anwenden (setzt alle relevanten Headers)
BrowserSpoofing.ApplyBrowserProfile(request, EnumBrowserProfile.Chrome);
BrowserSpoofing.ApplyBrowserProfile(request, EnumBrowserProfile.Firefox);
BrowserSpoofing.ApplyBrowserProfile(request, EnumBrowserProfile.Edge);
BrowserSpoofing.ApplyBrowserProfile(request, EnumBrowserProfile.Safari);

// Referer-Strategie
BrowserSpoofing.ApplyRefererStrategy(request, EnumRefererStrategy.SearchEngine);
BrowserSpoofing.ApplyRefererStrategy(request, EnumRefererStrategy.BaseHost);
BrowserSpoofing.ApplyRefererStrategy(request, EnumRefererStrategy.PreviousUrl, "https://previous.com");
```

### Bogus User-Agents

```csharp
using DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;

// Zufälliger User-Agent
Response response = await new Request("https://example.com")
    .WithBogusUserAgent()
    .SendAsync();

// Spezifischer Browser-Typ
Response response = await new Request("https://example.com")
    .WithBogusUserAgent<BogusChromeUserAgentGenerator>()
    .SendAsync();

// Kombinierte User-Agents
Response response = await new Request("https://example.com")
    .WithBogusUserAgent<BogusChromeUserAgentGenerator, BogusFirefoxUserAgentGenerator>()
    .SendAsync();
```

**Verfügbare Generatoren:**
- `BogusChromeUserAgentGenerator`
- `BogusFirefoxUserAgentGenerator`
- `BogusEdgeUserAgentGenerator`
- `BogusOperaUserAgentGenerator`

---

## Batch-Requests

### Rate Limiting

```csharp
using DevBase.Requests.Core;

Requests batchRequests = new Requests()
    .WithRateLimit(10, TimeSpan.FromSeconds(1))  // 10 Requests pro Sekunde
    .Add("https://api.example.com/item/1")
    .Add("https://api.example.com/item/2")
    .Add("https://api.example.com/item/3");

List<Response> responses = await batchRequests.SendAllAsync();
```

### Parallelisierung

```csharp
Requests batchRequests = new Requests()
    .WithParallelism(5)  // Maximal 5 parallele Requests
    .WithRateLimit(20, TimeSpan.FromSeconds(1))
    .Add(urls);

// Alle Responses auf einmal
List<Response> responses = await batchRequests.SendAllAsync();

// Als Async-Enumerable (Streaming)
await foreach (Response response in batchRequests.SendAllAsyncEnumerable())
{
    Console.WriteLine($"Received: {response.StatusCode}");
}

// Mit Callback
Requests batchRequests = new Requests()
    .OnResponse(response => Console.WriteLine($"Status: {response.StatusCode}"))
    .OnResponse(async response => await ProcessResponseAsync(response))
    .Add(urls);

await batchRequests.SendAllAsync();
```

### Cookie- und Referer-Persistenz

```csharp
Requests session = new Requests()
    .WithCookiePersistence(true)   // Cookies zwischen Requests beibehalten
    .WithRefererPersistence(true)  // Referer automatisch setzen
    .Add("https://example.com/login")
    .Add("https://example.com/dashboard")
    .Add("https://example.com/data");

await session.SendAllAsync();
```

---

## Response-Caching

```csharp
using DevBase.Requests.Cache;

ResponseCache cache = new ResponseCache();

Request request = new Request("https://api.example.com/data");

// Aus Cache holen oder Request ausführen
CachedResponse? cached = await cache.GetAsync(request);
if (cached == null)
{
    Response response = await request.SendAsync();
    await cache.SetAsync(request, response);
}

// Cache invalidieren
cache.Remove(request);
cache.Clear();
```

---

## Interceptors

### Request Interceptor

```csharp
using DevBase.Requests.Interfaces;

public class LoggingRequestInterceptor : IRequestInterceptor
{
    public int Order => 0;  // Ausführungsreihenfolge

    public Task OnRequestAsync(Request request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending request to: {request.Uri}");
        return Task.CompletedTask;
    }
}

// Verwendung
Response response = await new Request("https://api.example.com/data")
    .WithRequestInterceptor(new LoggingRequestInterceptor())
    .SendAsync();
```

### Response Interceptor

```csharp
public class MetricsResponseInterceptor : IResponseInterceptor
{
    public int Order => 0;

    public Task OnResponseAsync(Response response, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Response: {response.StatusCode} in {response.Metrics.TotalDuration.TotalMilliseconds}ms");
        return Task.CompletedTask;
    }
}

Response response = await new Request("https://api.example.com/data")
    .WithResponseInterceptor(new MetricsResponseInterceptor())
    .SendAsync();
```

---

## Metriken

```csharp
Response response = await new Request("https://api.example.com/data").SendAsync();

RequestMetrics metrics = response.Metrics;

Console.WriteLine($"Total Duration: {metrics.TotalDuration.TotalMilliseconds}ms");
Console.WriteLine($"Connect Time: {metrics.ConnectDuration.TotalMilliseconds}ms");
Console.WriteLine($"Time to First Byte: {metrics.TimeToFirstByte.TotalMilliseconds}ms");
Console.WriteLine($"Download Time: {metrics.DownloadDuration.TotalMilliseconds}ms");
Console.WriteLine($"Bytes Received: {metrics.BytesReceived}");
Console.WriteLine($"Retry Count: {metrics.RetryCount}");
Console.WriteLine($"Protocol: {metrics.Protocol}");
Console.WriteLine($"Used Proxy: {metrics.UsedProxy}");
```

---

## Validierung

**Header-Validierung ist standardmäßig aktiviert!** Sie wird automatisch beim `Build()` ausgeführt.

```csharp
// Deaktivieren wenn nötig
Response response = await new Request("https://api.example.com")
    .WithHeaderValidation(false)
    .SendAsync();
```

### Header-Validierung

```csharp
using DevBase.Requests.Validation;

// Basic Auth validieren
ValidationResult result = HeaderValidator.ValidateBasicAuth("Basic dXNlcjpwYXNz");

// Bearer Token validieren
ValidationResult result = HeaderValidator.ValidateBearerAuth("Bearer eyJhbGciOiJIUzI1NiIs...");

// Content-Type validieren
ValidationResult result = HeaderValidator.ValidateContentType("application/json");

// Cookie validieren
ValidationResult result = HeaderValidator.ValidateCookie("session=abc123; user=test");

// Host Header validieren
ValidationResult result = HeaderValidator.ValidateHost("api.example.com", new Uri("https://api.example.com"));

if (!result.IsValid)
{
    Console.WriteLine($"Validation failed: {result.ErrorMessage}");
}
```

### JWT-Validierung

```csharp
using DevBase.Requests.Validation;
using DevBase.Requests.Security.Token;

// JWT-Format validieren
ValidationResult result = HeaderValidator.ValidateJwtToken(jwtToken);

// JWT mit Expiration-Check
ValidationResult result = HeaderValidator.ValidateJwtToken(jwtToken, checkExpiration: true);

// JWT mit Signatur-Verifizierung
ValidationResult result = HeaderValidator.ValidateBearerAuth(
    "Bearer " + jwtToken, 
    verifySignature: true, 
    secret: "your-secret-key", 
    checkExpiration: true);

// JWT parsen
AuthenticationToken? token = HeaderValidator.ParseJwtToken(jwtToken);
Console.WriteLine($"Subject: {token?.Payload.Subject}");
Console.WriteLine($"Expires: {token?.Payload.ExpiresAt}");

// JWT parsen und verifizieren
AuthenticationToken? verified = HeaderValidator.ParseAndVerifyJwtToken(jwtToken, "secret");
if (verified?.Signature.Verified == true)
    Console.WriteLine("Signature valid!");
```

### URL-Validierung

```csharp
using DevBase.Requests.Validation;

ValidationResult result = UrlValidator.Validate("https://api.example.com/path?query=value");

if (!result.IsValid)
{
    Console.WriteLine($"Invalid URL: {result.ErrorMessage}");
}
```

---

## MIME-Types

Die `MimeDictionary`-Klasse bietet performante MIME-Type-Lookups mit `FrozenDictionary` und `AlternateLookup`:

```csharp
using DevBase.Requests.Data.Body.Mime;

MimeDictionary mimeDict = new MimeDictionary();

// MIME-Type als String
string mimeType = mimeDict.GetMimeTypeAsString("json");  // "application/json"
string mimeType = mimeDict.GetMimeTypeAsString(".pdf");  // "application/pdf"

// Als ReadOnlyMemory<char> (keine Allokation)
ReadOnlyMemory<char> mime = mimeDict.GetMimeTypeAsMemory("html");

// Als ReadOnlySpan<char> (keine Allokation)
ReadOnlySpan<char> mime = mimeDict.GetMimeTypeAsSpan("xml");

// TryGet-Varianten
if (mimeDict.TryGetMimeTypeAsString("custom", out string result))
{
    Console.WriteLine($"Found: {result}");
}
```

---

## Architektur

```
DevBase.Requests/
├── Core/
│   ├── Request.cs              # Hauptklasse für HTTP-Requests
│   ├── RequestConfiguration.cs # Fluent API für Request-Konfiguration
│   ├── RequestHttp.cs          # HTTP-Logik (SendAsync, Client-Pooling)
│   ├── Requests.cs             # Batch-Request-Verarbeitung
│   └── Response.cs             # Response-Wrapper mit Parsing-Methoden
├── Data/
│   ├── Body/                   # Body-Builder (Raw, JSON, Form, Multipart)
│   ├── Header/                 # Header-Builder und User-Agent-Generatoren
│   └── Parameters/             # URL-Parameter-Builder
├── Configuration/
│   ├── RetryPolicy.cs          # Retry-Konfiguration
│   ├── HostCheckConfig.cs      # Host-Erreichbarkeitsprüfung
│   └── ...                     # Weitere Konfigurationsklassen
├── Proxy/
│   ├── ProxyInfo.cs            # Proxy-Konfiguration
│   ├── ProxyService.cs         # Proxy-Pool-Verwaltung
│   ├── HttpToSocks5/           # HTTP-zu-SOCKS5-Bridge
│   └── Socks/                  # SOCKS4/5-Client-Implementierung
├── Parsing/
│   ├── JsonPathParser.cs       # JsonPath-Implementierung
│   └── StreamingJsonPathParser.cs # Streaming JsonPath
├── Cache/
│   ├── ResponseCache.cs        # Response-Caching
│   └── CachedResponse.cs       # Cache-Datenstruktur
├── Validation/
│   ├── HeaderValidator.cs      # Header-Validierung
│   └── UrlValidator.cs         # URL-Validierung
├── Spoofing/
│   └── BrowserSpoofing.cs      # Browser-Fingerprint-Spoofing
├── Metrics/
│   └── RequestMetrics.cs       # Performance-Metriken
└── Exceptions/
    ├── NetworkException.cs     # Netzwerk-Fehler
    ├── ProxyException.cs       # Proxy-Fehler
    ├── RateLimitException.cs   # Rate-Limit-Fehler
    └── RequestTimeoutException.cs # Timeout-Fehler
```

---

## Performance-Optimierungen

Diese Bibliothek wurde für maximale Performance optimiert:

### FrozenDictionary für MIME-Types

Die `MimeDictionary` verwendet `System.Collections.Frozen.FrozenDictionary` mit `AlternateLookup` für Span-basierte Lookups ohne Heap-Allokationen:

```csharp
// Keine String-Allokation bei der Suche
ReadOnlySpan<char> query = ".json".AsSpan();
if (mimeTypes.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(query, out ReadOnlyMemory<char> result))
{
    // Gefunden ohne Allokation
}
```

### HTTP-Client-Pooling

Requests werden über einen gemeinsamen `HttpClient`-Pool verarbeitet:

```csharp
// Pool-Konfiguration anpassen
Request.ConfigureConnectionPool(
    connectionLifetime: TimeSpan.FromMinutes(10),
    connectionIdleTimeout: TimeSpan.FromMinutes(5),
    maxConnections: 20
);

// Pool leeren (bei Bedarf)
Request.ClearClientPool();
```

### ArrayPool für Buffer

Große Buffer werden über `ArrayPool<byte>.Shared` verwaltet, um GC-Druck zu reduzieren.

### Explicit Typing

Die gesamte Codebasis verwendet explizite Typen statt `var` für verbesserte Lesbarkeit und Wartbarkeit.

---

## API-Referenz

### Request-Klasse

| Methode | Beschreibung |
|---------|--------------|
| `WithUrl(string)` | URL setzen |
| `WithMethod(HttpMethod)` | HTTP-Methode setzen |
| `AsGet()`, `AsPost()`, etc. | HTTP-Methode-Shortcuts |
| `WithHeader(string, string)` | Header hinzufügen |
| `WithAccept(params string[])` | Accept-Header setzen |
| `WithUserAgent(string)` | User-Agent setzen |
| `WithBogusUserAgent()` | Zufälligen User-Agent generieren |
| `WithJsonBody<T>(T)` | JSON-Body setzen |
| `WithRawBody(string)` | Raw-Body setzen |
| `WithEncodedForm(...)` | URL-encoded Form setzen |
| `WithProxy(ProxyInfo)` | Proxy konfigurieren |
| `WithTimeout(TimeSpan)` | Timeout setzen |
| `WithRetryPolicy(RetryPolicy)` | Retry-Policy setzen |
| `WithCertificateValidation(bool)` | Zertifikatvalidierung |
| `WithFollowRedirects(bool, int)` | Redirect-Verhalten |
| `UseBasicAuthentication(string, string)` | Basic Auth |
| `UseBearerAuthentication(string)` | Bearer Token |
| `WithRequestInterceptor(...)` | Request-Interceptor |
| `WithResponseInterceptor(...)` | Response-Interceptor |
| `SendAsync(CancellationToken)` | Request ausführen |

### Response-Klasse

| Eigenschaft/Methode | Beschreibung |
|---------------------|--------------|
| `StatusCode` | HTTP-Statuscode |
| `IsSuccessStatusCode` | 2xx Status? |
| `Headers` | Response-Headers |
| `ContentType` | Content-Type Header |
| `ContentLength` | Content-Length |
| `Metrics` | Request-Metriken |
| `GetStringAsync()` | Als String lesen |
| `GetBytesAsync()` | Als Bytes lesen |
| `GetStream()` | Als Stream |
| `ParseJsonAsync<T>()` | JSON deserialisieren |
| `ParseJsonDocumentAsync()` | Als JsonDocument |
| `ParseHtmlAsync()` | Als HTML (AngleSharp) |
| `ParseXmlAsync()` | Als XDocument |
| `ParseJsonPathAsync<T>(string)` | JsonPath-Query |
| `StreamLinesAsync()` | Zeilen streamen |
| `StreamChunksAsync(int)` | Chunks streamen |
| `GetCookies()` | Cookies extrahieren |

---

## Lizenz

MIT License - siehe LICENSE-Datei für Details.

---

## Changelog

### Version X.X.X

- Performance-Optimierung: `MimeDictionary` verwendet nun `FrozenDictionary` mit `AlternateLookup`
- Code-Style: Alle `var`-Deklarationen durch explizite Typen ersetzt
- Optimierung: `CachedResponse.Headers` verwendet `FrozenDictionary`
- Optimierung: `JsonUtils.TryGetEntries` mit voralloziertem Dictionary und HashSet
