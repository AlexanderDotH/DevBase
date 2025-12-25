using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// State tracking for rate limiting and retry simulation
var rateLimitState = new ConcurrentDictionary<string, RateLimitInfo>();
var retryState = new ConcurrentDictionary<string, int>();

// =============================================================================
// HEALTH CHECK
// =============================================================================
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// =============================================================================
// BASIC HTTP METHODS
// =============================================================================

app.MapGet("/api/get", () => Results.Ok(new { 
    method = "GET", 
    message = "Hello from GET",
    timestamp = DateTime.UtcNow 
}));

app.MapPost("/api/post", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();
    return Results.Ok(new { 
        method = "POST", 
        receivedBody = body,
        contentType = request.ContentType,
        contentLength = request.ContentLength,
        timestamp = DateTime.UtcNow 
    });
});

app.MapPut("/api/put", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();
    return Results.Ok(new { 
        method = "PUT", 
        receivedBody = body,
        timestamp = DateTime.UtcNow 
    });
});

app.MapDelete("/api/delete/{id}", (int id) => Results.Ok(new { 
    method = "DELETE", 
    deletedId = id,
    timestamp = DateTime.UtcNow 
}));

app.MapPatch("/api/patch/{id}", async (int id, HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();
    return Results.Ok(new { 
        method = "PATCH", 
        patchedId = id,
        receivedBody = body,
        timestamp = DateTime.UtcNow 
    });
});

// =============================================================================
// QUERY PARAMETERS & HEADERS
// =============================================================================

app.MapGet("/api/query", (HttpRequest request) =>
{
    var queryParams = request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
    return Results.Ok(new { query = queryParams });
});

app.MapGet("/api/headers", (HttpRequest request) =>
{
    var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
    return Results.Ok(new { headers });
});

app.MapGet("/api/echo-header/{headerName}", (string headerName, HttpRequest request) =>
{
    if (request.Headers.TryGetValue(headerName, out var value))
        return Results.Ok(new { header = headerName, value = value.ToString() });
    return Results.NotFound(new { error = $"Header '{headerName}' not found" });
});

// =============================================================================
// FILE UPLOAD
// =============================================================================

app.MapPost("/api/upload", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "Expected multipart/form-data" });

    var form = await request.ReadFormAsync();
    var files = form.Files.Select(f => new {
        name = f.Name,
        fileName = f.FileName,
        contentType = f.ContentType,
        length = f.Length
    }).ToList();

    var formFields = form.Where(f => f.Key != null && !form.Files.Any(file => file.Name == f.Key))
        .ToDictionary(f => f.Key, f => f.Value.ToString());

    return Results.Ok(new { 
        filesReceived = files.Count,
        files,
        formFields,
        totalSize = files.Sum(f => f.length)
    });
});

app.MapPost("/api/upload-single", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "Expected multipart/form-data" });

    var form = await request.ReadFormAsync();
    var file = form.Files.FirstOrDefault();
    
    if (file == null)
        return Results.BadRequest(new { error = "No file uploaded" });

    using var ms = new MemoryStream();
    await file.CopyToAsync(ms);
    var content = ms.ToArray();

    return Results.Ok(new {
        fileName = file.FileName,
        contentType = file.ContentType,
        length = file.Length,
        md5 = Convert.ToHexString(System.Security.Cryptography.MD5.HashData(content))
    });
});

// =============================================================================
// RATE LIMITING SIMULATION
// =============================================================================

app.MapGet("/api/rate-limited", (HttpRequest request) =>
{
    var clientId = request.Headers["X-Client-Id"].FirstOrDefault() ?? "default";
    var now = DateTime.UtcNow;
    
    var info = rateLimitState.GetOrAdd(clientId, _ => new RateLimitInfo { WindowStart = now });
    
    // Reset window every 10 seconds
    if ((now - info.WindowStart).TotalSeconds > 10)
    {
        info.WindowStart = now;
        info.RequestCount = 0;
    }
    
    info.RequestCount++;
    
    // Allow 5 requests per window
    if (info.RequestCount > 5)
    {
        var retryAfter = 10 - (int)(now - info.WindowStart).TotalSeconds;
        return Results.Json(
            new { error = "Rate limit exceeded", retryAfterSeconds = retryAfter },
            statusCode: 429,
            contentType: "application/json"
        );
    }
    
    return Results.Ok(new { 
        remaining = 5 - info.RequestCount,
        resetIn = 10 - (int)(now - info.WindowStart).TotalSeconds
    });
});

app.MapGet("/api/rate-limit-strict", (HttpRequest request) =>
{
    // Always returns 429 on first 2 requests, then succeeds
    var clientId = request.Headers["X-Client-Id"].FirstOrDefault() ?? "strict-default";
    var count = retryState.AddOrUpdate(clientId, 1, (_, c) => c + 1);
    
    if (count <= 2)
    {
        return Results.Json(
            new { error = "Rate limit exceeded", attempt = count },
            statusCode: 429,
            contentType: "application/json"
        );
    }
    
    // Reset for next test
    retryState.TryRemove(clientId, out _);
    return Results.Ok(new { success = true, attemptsTaken = count });
});

app.MapPost("/api/rate-limit/reset", () =>
{
    rateLimitState.Clear();
    retryState.Clear();
    return Results.Ok(new { reset = true });
});

// =============================================================================
// RETRY SIMULATION
// =============================================================================

app.MapGet("/api/retry-eventually", (HttpRequest request) =>
{
    var clientId = request.Headers["X-Client-Id"].FirstOrDefault() ?? "retry-default";
    var count = retryState.AddOrUpdate(clientId, 1, (_, c) => c + 1);
    
    // Fail first 3 attempts with 503
    if (count <= 3)
    {
        return Results.Json(
            new { error = "Service temporarily unavailable", attempt = count },
            statusCode: 503,
            contentType: "application/json"
        );
    }
    
    retryState.TryRemove(clientId, out _);
    return Results.Ok(new { success = true, attemptsTaken = count });
});

app.MapGet("/api/fail-once", (HttpRequest request) =>
{
    var clientId = request.Headers["X-Client-Id"].FirstOrDefault() ?? "fail-once-default";
    var count = retryState.AddOrUpdate(clientId, 1, (_, c) => c + 1);
    
    if (count == 1)
    {
        return Results.Json(
            new { error = "Temporary failure" },
            statusCode: 500,
            contentType: "application/json"
        );
    }
    
    retryState.TryRemove(clientId, out _);
    return Results.Ok(new { success = true });
});

app.MapGet("/api/always-fail", () =>
{
    return Results.Json(
        new { error = "This endpoint always fails" },
        statusCode: 500,
        contentType: "application/json"
    );
});

// =============================================================================
// DELAY SIMULATION
// =============================================================================

app.MapGet("/api/delay/{ms:int}", async (int ms) =>
{
    await Task.Delay(Math.Min(ms, 30000)); // Cap at 30 seconds
    return Results.Ok(new { delayed = true, ms });
});

app.MapGet("/api/timeout", async () =>
{
    await Task.Delay(TimeSpan.FromMinutes(5)); // Will timeout most clients
    return Results.Ok(new { completed = true });
});

// =============================================================================
// ERROR RESPONSES
// =============================================================================

app.MapGet("/api/error/{code:int}", (int code) =>
{
    return Results.Json(
        new { error = $"Error {code}", code },
        statusCode: code,
        contentType: "application/json"
    );
});

app.MapGet("/api/error/random", () =>
{
    var codes = new[] { 400, 401, 403, 404, 500, 502, 503 };
    var code = codes[Random.Shared.Next(codes.Length)];
    return Results.Json(
        new { error = $"Random error {code}", code },
        statusCode: code,
        contentType: "application/json"
    );
});

// =============================================================================
// AUTHENTICATION
// =============================================================================

app.MapGet("/api/auth/basic", (HttpRequest request) =>
{
    var authHeader = request.Headers.Authorization.FirstOrDefault();
    if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
    {
        return Results.Json(
            new { error = "Unauthorized" },
            statusCode: 401,
            contentType: "application/json"
        );
    }
    
    var credentials = System.Text.Encoding.UTF8.GetString(
        Convert.FromBase64String(authHeader.Substring(6)));
    var parts = credentials.Split(':');
    
    if (parts.Length == 2 && parts[0] == "testuser" && parts[1] == "testpass")
    {
        return Results.Ok(new { authenticated = true, user = parts[0] });
    }
    
    return Results.Json(
        new { error = "Invalid credentials" },
        statusCode: 401,
        contentType: "application/json"
    );
});

app.MapGet("/api/auth/bearer", (HttpRequest request) =>
{
    var authHeader = request.Headers.Authorization.FirstOrDefault();
    if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
    {
        return Results.Json(
            new { error = "Unauthorized" },
            statusCode: 401,
            contentType: "application/json"
        );
    }
    
    var token = authHeader.Substring(7);
    if (token == "valid-test-token")
    {
        return Results.Ok(new { authenticated = true, token });
    }
    
    return Results.Json(
        new { error = "Invalid token" },
        statusCode: 401,
        contentType: "application/json"
    );
});

// =============================================================================
// COOKIES
// =============================================================================

app.MapGet("/api/cookies/set", (HttpContext context) =>
{
    context.Response.Cookies.Append("session", "abc123", new CookieOptions { HttpOnly = true });
    context.Response.Cookies.Append("user", "testuser");
    return Results.Ok(new { cookiesSet = new[] { "session", "user" } });
});

app.MapGet("/api/cookies/get", (HttpRequest request) =>
{
    var cookies = request.Cookies.ToDictionary(c => c.Key, c => c.Value);
    return Results.Ok(new { cookies });
});

// =============================================================================
// LARGE RESPONSES
// =============================================================================

app.MapGet("/api/large/{count:int}", (int count) =>
{
    var items = Enumerable.Range(1, Math.Min(count, 10000))
        .Select(i => new { id = i, name = $"Item {i}", data = new string('x', 100) })
        .ToList();
    return Results.Ok(new { count = items.Count, items });
});

app.MapGet("/api/stream/{chunks:int}", async (int chunks, HttpContext context) =>
{
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync("[");
    
    for (int i = 0; i < Math.Min(chunks, 100); i++)
    {
        if (i > 0) await context.Response.WriteAsync(",");
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { chunk = i, data = new string('x', 1000) }));
        await context.Response.Body.FlushAsync();
        await Task.Delay(10);
    }
    
    await context.Response.WriteAsync("]");
});

// =============================================================================
// PROXY DETECTION
// =============================================================================

app.MapGet("/api/proxy-check", (HttpRequest request) =>
{
    var proxyHeaders = new[] { "X-Forwarded-For", "X-Real-IP", "Via", "X-Proxy-Id" };
    var detectedHeaders = proxyHeaders
        .Where(h => request.Headers.ContainsKey(h))
        .ToDictionary(h => h, h => request.Headers[h].ToString());
    
    return Results.Ok(new {
        clientIp = request.HttpContext.Connection.RemoteIpAddress?.ToString(),
        proxyDetected = detectedHeaders.Count > 0,
        proxyHeaders = detectedHeaders
    });
});

// =============================================================================
// BATCH TESTING
// =============================================================================

app.MapGet("/api/batch/{id:int}", (int id) =>
{
    return Results.Ok(new { 
        id, 
        processed = true,
        timestamp = DateTime.UtcNow 
    });
});

app.MapPost("/api/batch/submit", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();
    
    // Simulate processing delay
    await Task.Delay(Random.Shared.Next(10, 100));
    
    return Results.Ok(new {
        received = true,
        bodyLength = body.Length,
        processedAt = DateTime.UtcNow
    });
});

app.Run();

// =============================================================================
// HELPER CLASSES
// =============================================================================

class RateLimitInfo
{
    public DateTime WindowStart { get; set; }
    public int RequestCount { get; set; }
}
