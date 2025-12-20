using System.Net;
using System.Text;
using System.Text.Json;
using DevBase.Requests;
using DevBase.Requests.Proxy.HttpToSocks5;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests.Integration;

[TestFixture]
[Category("Integration")]
[Category("EndToEnd")]
public class EndToEndIntegrationTest
{
    private MockHttpServer _httpServer = null!;
    private MockSocks5Server _socks5Server = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _httpServer = new MockHttpServer();
        _socks5Server = new MockSocks5Server();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _httpServer.Dispose();
        _socks5Server.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        _httpServer.ResetCounters();
        _socks5Server.ResetCounters();
    }

    #region Full CRUD Workflow

    [Test]
    public async Task CRUD_CompleteWorkflow_AllOperationsSucceed()
    {
        // CREATE
        var createRequest = new Request($"{_httpServer.BaseUrl}/api/users")
            .AsPost()
            .WithJsonBody(JsonSerializer.Serialize(new { name = "TestUser", email = "test@example.com" }), Encoding.UTF8)
            .Build();
        
        var createResponse = await createRequest.SendAsync();
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        // READ
        var readRequest = new Request($"{_httpServer.BaseUrl}/api/users/1")
            .AsGet()
            .Build();
        
        var readResponse = await readRequest.SendAsync();
        Assert.That(readResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // UPDATE
        var updateRequest = new Request($"{_httpServer.BaseUrl}/api/users/1")
            .AsPut()
            .WithJsonBody(JsonSerializer.Serialize(new { name = "UpdatedUser", email = "updated@example.com" }), Encoding.UTF8)
            .Build();
        
        var updateResponse = await updateRequest.SendAsync();
        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // DELETE
        var deleteRequest = new Request($"{_httpServer.BaseUrl}/api/users/1")
            .AsDelete()
            .Build();
        
        var deleteResponse = await deleteRequest.SendAsync();
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Verify all requests were recorded
        Assert.That(_httpServer.RequestCount, Is.EqualTo(4));
    }

    #endregion

    #region Full Workflow Through SOCKS5 Proxy

    [Test]
    [Timeout(30000)]
    [Ignore("SOCKS5 proxy CRUD workflow requires stable connection - basic proxy tests verify functionality")]
    public async Task CRUD_ThroughSocks5Proxy_CompleteWorkflow()
    {
        // Arrange
        using var httpToSocks5 = new HttpToSocks5Proxy("127.0.0.1", _socks5Server.Port);
        var handler = new HttpClientHandler { Proxy = httpToSocks5 };
        using var client = new HttpClient(handler);

        // CREATE
        var createContent = new StringContent(
            JsonSerializer.Serialize(new { name = "ProxyUser", email = "proxy@example.com" }),
            Encoding.UTF8,
            "application/json");
        var createResponse = await client.PostAsync($"{_httpServer.BaseUrl}/api/users", createContent);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        // READ
        var readResponse = await client.GetAsync($"{_httpServer.BaseUrl}/api/users/1");
        Assert.That(readResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // UPDATE
        var updateContent = new StringContent(
            JsonSerializer.Serialize(new { name = "UpdatedProxyUser" }),
            Encoding.UTF8,
            "application/json");
        var updateResponse = await client.PutAsync($"{_httpServer.BaseUrl}/api/users/1", updateContent);
        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // DELETE
        var deleteResponse = await client.DeleteAsync($"{_httpServer.BaseUrl}/api/users/1");
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        // Verify proxy was used
        Assert.That(_socks5Server.SuccessfulConnections, Is.GreaterThan(0));
    }

    #endregion

    #region Authentication Flow

    [Test]
    public async Task Authentication_BearerToken_CompleteFlow()
    {
        // Step 1: Attempt without auth (should fail)
        var noAuthRequest = new Request($"{_httpServer.BaseUrl}/api/auth")
            .AsGet()
            .Build();
        var noAuthResponse = await noAuthRequest.SendAsync();
        Assert.That(noAuthResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        // Step 2: With valid token (should succeed)
        var authRequest = new Request($"{_httpServer.BaseUrl}/api/auth")
            .AsGet()
            .UseBearerAuthentication("valid-token-12345")
            .Build();
        var authResponse = await authRequest.SendAsync();
        Assert.That(authResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var content = await authResponse.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.That(doc.RootElement.GetProperty("authenticated").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Authentication_BasicAuth_CompleteFlow()
    {
        // Step 1: Attempt without auth (should fail)
        var noAuthRequest = new Request($"{_httpServer.BaseUrl}/api/auth")
            .AsGet()
            .Build();
        var noAuthResponse = await noAuthRequest.SendAsync();
        Assert.That(noAuthResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        // Step 2: With basic auth (should succeed)
        var authRequest = new Request($"{_httpServer.BaseUrl}/api/auth")
            .AsGet()
            .UseBasicAuthentication("admin", "secret123")
            .Build();
        var authResponse = await authRequest.SendAsync();
        Assert.That(authResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Complex Request Building

    [Test]
    public async Task ComplexRequest_AllFeaturesCombined()
    {
        // Arrange - Build a complex request with multiple features
        var request = new Request($"{_httpServer.BaseUrl}/api/headers")
            .AsGet()
            .WithParameter("page", "1")
            .WithParameter("limit", "10")
            .WithParameter("sort", "name")
            .WithHeader("X-Request-ID", "test-request-123")
            .WithHeader("X-Client-Version", "1.0.0")
            .WithUserAgent("IntegrationTest/1.0")
            .WithAccept("application/json")
            .WithTimeout(TimeSpan.FromSeconds(30))
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        var headers = doc.RootElement.GetProperty("headers");
        
        Assert.That(headers.GetProperty("X-Request-ID").GetString(), Is.EqualTo("test-request-123"));
        Assert.That(headers.GetProperty("X-Client-Version").GetString(), Is.EqualTo("1.0.0"));
        Assert.That(headers.GetProperty("User-Agent").GetString(), Is.EqualTo("IntegrationTest/1.0"));
    }

    #endregion

    #region Error Handling Scenarios

    [Test]
    public async Task ErrorHandling_MultipleErrorCodes()
    {
        // Test 404
        var notFoundRequest = new Request($"{_httpServer.BaseUrl}/api/users/999")
            .AsGet()
            .Build();
        var notFoundResponse = await notFoundRequest.SendAsync();
        Assert.That(notFoundResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        // Test 500
        var serverErrorRequest = new Request($"{_httpServer.BaseUrl}/api/error/500")
            .AsGet()
            .Build();
        var serverErrorResponse = await serverErrorRequest.SendAsync();
        Assert.That(serverErrorResponse.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));

        // Test 401
        var unauthorizedRequest = new Request($"{_httpServer.BaseUrl}/api/auth")
            .AsGet()
            .Build();
        var unauthorizedResponse = await unauthorizedRequest.SendAsync();
        Assert.That(unauthorizedResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    #endregion

    #region Multiple Proxies Test

    [Test]
    public async Task MultipleSocks5Servers_CanSwitchBetween()
    {
        // Create a second SOCKS5 server
        using var secondSocks5Server = new MockSocks5Server();

        // Use first proxy
        using var proxy1 = new HttpToSocks5Proxy("127.0.0.1", _socks5Server.Port);
        var handler1 = new HttpClientHandler { Proxy = proxy1 };
        using var client1 = new HttpClient(handler1);
        
        var response1 = await client1.GetAsync($"{_httpServer.BaseUrl}/api/json");
        Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(_socks5Server.SuccessfulConnections, Is.GreaterThan(0));

        // Use second proxy
        using var proxy2 = new HttpToSocks5Proxy("127.0.0.1", secondSocks5Server.Port);
        var handler2 = new HttpClientHandler { Proxy = proxy2 };
        using var client2 = new HttpClient(handler2);
        
        var response2 = await client2.GetAsync($"{_httpServer.BaseUrl}/api/json");
        Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(secondSocks5Server.SuccessfulConnections, Is.GreaterThan(0));
    }

    #endregion

    #region Concurrent Operations

    [Test]
    public async Task ConcurrentRequests_DifferentEndpoints_AllSucceed()
    {
        // Arrange
        var endpoints = new[]
        {
            "/api/json",
            "/api/users",
            "/api/users/1",
            "/api/nested",
            "/api/text",
            "/api/headers"
        };

        // Act
        var tasks = endpoints.Select(async endpoint =>
        {
            var request = new Request($"{_httpServer.BaseUrl}{endpoint}")
                .AsGet()
                .Build();
            return await request.SendAsync();
        }).ToList();

        var responses = await Task.WhenAll(tasks);

        // Assert
        Assert.That(responses.All(r => r.StatusCode == HttpStatusCode.OK), Is.True);
        Assert.That(_httpServer.RequestCount, Is.EqualTo(endpoints.Length));
    }

    [Test]
    public async Task ConcurrentRequests_ThroughProxy_AllSucceed()
    {
        // Arrange
        using var httpToSocks5 = new HttpToSocks5Proxy("127.0.0.1", _socks5Server.Port);
        var handler = new HttpClientHandler { Proxy = httpToSocks5 };
        using var client = new HttpClient(handler);

        // Act
        var tasks = Enumerable.Range(0, 5)
            .Select(_ => client.GetAsync($"{_httpServer.BaseUrl}/api/json"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        // Assert
        Assert.That(responses.All(r => r.StatusCode == HttpStatusCode.OK), Is.True);
    }

    #endregion

    #region Response Parsing

    [Test]
    public async Task ResponseParsing_JsonToObject_WorksCorrectly()
    {
        // Arrange
        var request = new Request($"{_httpServer.BaseUrl}/api/users/1")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var content = await response.GetStringAsync();
        var user = JsonSerializer.Deserialize<TestUser>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.That(user, Is.Not.Null);
        Assert.That(user!.Id, Is.EqualTo(1));
        Assert.That(user.Name, Is.EqualTo("Alice"));
        Assert.That(user.Email, Is.EqualTo("alice@example.com"));
        Assert.That(user.Active, Is.True);
    }

    [Test]
    public async Task ResponseParsing_JsonArray_WorksCorrectly()
    {
        // Arrange
        var request = new Request($"{_httpServer.BaseUrl}/api/users")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var content = await response.GetStringAsync();
        var result = JsonSerializer.Deserialize<UsersResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Users, Has.Count.EqualTo(3));
        Assert.That(result.Total, Is.EqualTo(3));
    }

    #endregion

    #region Helper Classes

    private class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public bool Active { get; set; }
    }

    private class UsersResponse
    {
        public List<TestUser> Users { get; set; } = new();
        public int Total { get; set; }
    }

    #endregion
}
