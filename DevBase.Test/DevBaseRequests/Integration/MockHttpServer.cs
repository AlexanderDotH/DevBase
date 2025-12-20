using System.Net;
using System.Text;
using System.Text.Json;

namespace DevBase.Test.DevBaseRequests.Integration;

/// <summary>
/// A mock HTTP server for integration testing.
/// Provides various endpoints for testing different HTTP scenarios.
/// </summary>
public sealed class MockHttpServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly CancellationTokenSource _cts;
    private readonly Task _serverTask;
    private readonly Dictionary<string, Func<HttpListenerRequest, MockResponse>> _routes;
    
    public int Port { get; }
    public string BaseUrl => $"http://localhost:{Port}";
    public int RequestCount { get; private set; }
    public List<RecordedRequest> RecordedRequests { get; } = new();

    public MockHttpServer(int port = 0)
    {
        Port = port == 0 ? GetAvailablePort() : port;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{Port}/");
        _cts = new CancellationTokenSource();
        _routes = new Dictionary<string, Func<HttpListenerRequest, MockResponse>>(StringComparer.OrdinalIgnoreCase);
        
        SetupDefaultRoutes();
        
        _listener.Start();
        _serverTask = Task.Run(ProcessRequestsAsync);
    }

    private static int GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private void SetupDefaultRoutes()
    {
        // GET /api/json - Returns a simple JSON object
        AddRoute("GET /api/json", _ => new MockResponse
        {
            StatusCode = 200,
            ContentType = "application/json",
            Body = JsonSerializer.Serialize(new
            {
                message = "Hello, World!",
                timestamp = DateTime.UtcNow.ToString("O"),
                success = true
            })
        });

        // GET /api/users - Returns a list of users
        AddRoute("GET /api/users", _ => new MockResponse
        {
            StatusCode = 200,
            ContentType = "application/json",
            Body = JsonSerializer.Serialize(new
            {
                users = new[]
                {
                    new { id = 1, name = "Alice", email = "alice@example.com", active = true },
                    new { id = 2, name = "Bob", email = "bob@example.com", active = true },
                    new { id = 3, name = "Charlie", email = "charlie@example.com", active = false }
                },
                total = 3
            })
        });

        // GET /api/users/{id} - Returns a single user
        AddRoute("GET /api/users/1", _ => new MockResponse
        {
            StatusCode = 200,
            ContentType = "application/json",
            Body = JsonSerializer.Serialize(new { id = 1, name = "Alice", email = "alice@example.com", active = true })
        });

        AddRoute("GET /api/users/999", _ => new MockResponse
        {
            StatusCode = 404,
            ContentType = "application/json",
            Body = JsonSerializer.Serialize(new { error = "User not found", code = "USER_NOT_FOUND" })
        });

        // POST /api/users - Create a new user
        AddRoute("POST /api/users", req =>
        {
            using var reader = new StreamReader(req.InputStream);
            var body = reader.ReadToEnd();
            
            return new MockResponse
            {
                StatusCode = 201,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new
                {
                    id = 4,
                    created = true,
                    receivedBody = body
                }),
                Headers = { ["Location"] = "/api/users/4" }
            };
        });

        // PUT /api/users/1 - Update a user
        AddRoute("PUT /api/users/1", req =>
        {
            using var reader = new StreamReader(req.InputStream);
            var body = reader.ReadToEnd();
            
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new { id = 1, updated = true, receivedBody = body })
            };
        });

        // DELETE /api/users/1 - Delete a user
        AddRoute("DELETE /api/users/1", _ => new MockResponse
        {
            StatusCode = 204,
            ContentType = null,
            Body = null
        });

        // PATCH /api/users/1 - Partially update a user
        AddRoute("PATCH /api/users/1", req =>
        {
            using var reader = new StreamReader(req.InputStream);
            var body = reader.ReadToEnd();
            
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new { id = 1, patched = true, receivedBody = body })
            };
        });

        // GET /api/headers - Echo back request headers
        AddRoute("GET /api/headers", req => new MockResponse
        {
            StatusCode = 200,
            ContentType = "application/json",
            Body = JsonSerializer.Serialize(new
            {
                headers = req.Headers.AllKeys.ToDictionary(k => k!, k => req.Headers[k])
            })
        });

        // GET /api/query - Echo back query parameters
        AddRoute("GET /api/query", req => new MockResponse
        {
            StatusCode = 200,
            ContentType = "application/json",
            Body = JsonSerializer.Serialize(new
            {
                query = req.QueryString.AllKeys.ToDictionary(k => k!, k => req.QueryString[k])
            })
        });

        // GET /api/delay/{ms} - Delayed response
        AddRoute("GET /api/delay/100", async _ =>
        {
            await Task.Delay(100);
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new { delayed = true, ms = 100 })
            };
        });

        AddRoute("GET /api/delay/500", async _ =>
        {
            await Task.Delay(500);
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new { delayed = true, ms = 500 })
            };
        });

        // GET /api/error/500 - Internal Server Error
        AddRoute("GET /api/error/500", _ => new MockResponse
        {
            StatusCode = 500,
            ContentType = "application/json",
            Body = JsonSerializer.Serialize(new { error = "Internal Server Error", code = "INTERNAL_ERROR" })
        });

        // GET /api/error/503 - Service Unavailable (for retry testing)
        var retryAttempt = 0;
        AddRoute("GET /api/retry", _ =>
        {
            retryAttempt++;
            if (retryAttempt < 3)
            {
                return new MockResponse
                {
                    StatusCode = 503,
                    ContentType = "application/json",
                    Body = JsonSerializer.Serialize(new { error = "Service temporarily unavailable", attempt = retryAttempt }),
                    Headers = { ["Retry-After"] = "1" }
                };
            }
            
            retryAttempt = 0;
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new { success = true, message = "Finally succeeded!" })
            };
        });

        // GET /api/redirect - 301 Redirect
        AddRoute("GET /api/redirect", _ => new MockResponse
        {
            StatusCode = 301,
            Headers = { ["Location"] = "/api/json" }
        });

        // GET /api/redirect/302 - 302 Redirect
        AddRoute("GET /api/redirect/302", _ => new MockResponse
        {
            StatusCode = 302,
            Headers = { ["Location"] = "/api/json" }
        });

        // POST /api/form - Form data handling
        AddRoute("POST /api/form", req =>
        {
            using var reader = new StreamReader(req.InputStream);
            var body = reader.ReadToEnd();
            
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new
                {
                    contentType = req.ContentType,
                    body = body,
                    contentLength = req.ContentLength64
                })
            };
        });

        // GET /api/large - Large JSON response
        AddRoute("GET /api/large", _ =>
        {
            var items = Enumerable.Range(1, 1000)
                .Select(i => new { id = i, name = $"Item {i}", value = i * 10 })
                .ToList();
            
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new { items, count = items.Count })
            };
        });

        // GET /api/nested - Deeply nested JSON
        AddRoute("GET /api/nested", _ => new MockResponse
        {
            StatusCode = 200,
            ContentType = "application/json",
            Body = JsonSerializer.Serialize(new
            {
                level1 = new
                {
                    level2 = new
                    {
                        level3 = new
                        {
                            data = new[] { "a", "b", "c" },
                            value = 42
                        }
                    }
                }
            })
        });

        // GET /api/auth - Requires Authorization header
        AddRoute("GET /api/auth", req =>
        {
            var authHeader = req.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return new MockResponse
                {
                    StatusCode = 401,
                    ContentType = "application/json",
                    Body = JsonSerializer.Serialize(new { error = "Unauthorized", code = "NO_AUTH" }),
                    Headers = { ["WWW-Authenticate"] = "Bearer" }
                };
            }

            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new
                {
                    authenticated = true,
                    authHeader = authHeader
                })
            };
        });

        // GET /api/cookies - Set and read cookies
        AddRoute("GET /api/cookies", req =>
        {
            var cookies = req.Cookies;
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new
                {
                    receivedCookies = cookies.Cast<Cookie>().Select(c => new { c.Name, c.Value }).ToList()
                }),
                Cookies = { new Cookie("session", "abc123"), new Cookie("user", "test") }
            };
        });

        // GET /api/text - Plain text response
        AddRoute("GET /api/text", _ => new MockResponse
        {
            StatusCode = 200,
            ContentType = "text/plain",
            Body = "This is plain text response"
        });

        // GET /api/xml - XML response
        AddRoute("GET /api/xml", _ => new MockResponse
        {
            StatusCode = 200,
            ContentType = "application/xml",
            Body = "<?xml version=\"1.0\"?><root><message>Hello XML</message></root>"
        });

        // GET /api/rate-limit - Rate limit simulation
        var rateLimitCounter = 0;
        AddRoute("GET /api/rate-limit", _ =>
        {
            rateLimitCounter++;
            if (rateLimitCounter > 5)
            {
                rateLimitCounter = 0;
                return new MockResponse
                {
                    StatusCode = 429,
                    ContentType = "application/json",
                    Body = JsonSerializer.Serialize(new { error = "Rate limit exceeded" }),
                    Headers = { ["Retry-After"] = "60", ["X-RateLimit-Remaining"] = "0" }
                };
            }

            return new MockResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new { remaining = 5 - rateLimitCounter }),
                Headers = { ["X-RateLimit-Remaining"] = (5 - rateLimitCounter).ToString() }
            };
        });

        // GET /api/echo - Echo everything
        AddRoute("POST /api/echo", req =>
        {
            using var reader = new StreamReader(req.InputStream);
            var body = reader.ReadToEnd();
            
            return new MockResponse
            {
                StatusCode = 200,
                ContentType = req.ContentType ?? "application/octet-stream",
                Body = body
            };
        });
    }

    public void AddRoute(string route, Func<HttpListenerRequest, MockResponse> handler)
    {
        _routes[route] = handler;
    }

    public void AddRoute(string route, Func<HttpListenerRequest, Task<MockResponse>> asyncHandler)
    {
        _routes[route] = req => asyncHandler(req).GetAwaiter().GetResult();
    }

    private async Task ProcessRequestsAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context));
            }
            catch (HttpListenerException) when (_cts.Token.IsCancellationRequested)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        RequestCount++;
        var request = context.Request;
        var response = context.Response;

        // Record the request
        RecordedRequests.Add(new RecordedRequest
        {
            Method = request.HttpMethod,
            Path = request.Url?.AbsolutePath ?? "/",
            Query = request.Url?.Query ?? "",
            Headers = request.Headers.AllKeys.ToDictionary(k => k!, k => request.Headers[k] ?? ""),
            Timestamp = DateTime.UtcNow
        });

        var routeKey = $"{request.HttpMethod} {request.Url?.AbsolutePath}";
        
        MockResponse mockResponse;
        if (_routes.TryGetValue(routeKey, out var handler))
        {
            mockResponse = handler(request);
        }
        else
        {
            mockResponse = new MockResponse
            {
                StatusCode = 404,
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new { error = "Not Found", path = request.Url?.AbsolutePath })
            };
        }

        try
        {
            response.StatusCode = mockResponse.StatusCode;
            
            if (mockResponse.ContentType != null)
                response.ContentType = mockResponse.ContentType;

            foreach (var header in mockResponse.Headers)
            {
                response.Headers[header.Key] = header.Value;
            }

            foreach (var cookie in mockResponse.Cookies)
            {
                response.Cookies.Add(cookie);
            }

            if (mockResponse.Body != null)
            {
                var buffer = Encoding.UTF8.GetBytes(mockResponse.Body);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }
        finally
        {
            response.Close();
        }
    }

    public void ResetCounters()
    {
        RequestCount = 0;
        RecordedRequests.Clear();
    }

    public void Dispose()
    {
        _cts.Cancel();
        _listener.Stop();
        _listener.Close();
        
        try { _serverTask.Wait(TimeSpan.FromSeconds(2)); }
        catch { /* ignore */ }
        
        _cts.Dispose();
    }
}

public class MockResponse
{
    public int StatusCode { get; set; } = 200;
    public string? ContentType { get; set; }
    public string? Body { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public List<Cookie> Cookies { get; set; } = new();
}

public class RecordedRequest
{
    public required string Method { get; init; }
    public required string Path { get; init; }
    public required string Query { get; init; }
    public required Dictionary<string, string> Headers { get; init; }
    public DateTime Timestamp { get; init; }
}

// Required for GetAvailablePort
file class TcpListener : System.Net.Sockets.TcpListener
{
    public TcpListener(IPAddress localaddr, int port) : base(localaddr, port) { }
}
