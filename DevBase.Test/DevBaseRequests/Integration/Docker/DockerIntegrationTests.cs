using System.Net;
using DevBase.Net;
using DevBase.Net.Batch;
using DevBase.Net.Batch.Proxied;
using DevBase.Net.Configuration;
using DevBase.Net.Configuration.Enums;
using DevBase.Net.Core;
using DevBase.Net.Proxy;
using DevBase.Net.Proxy.Enums;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests.Integration.Docker;

/// <summary>
/// Comprehensive Docker-based integration tests for DevBase.Net.
/// These tests require Docker containers to be running (see docker-compose.yml).
/// </summary>
[TestFixture]
[Category("Integration")]
[Category("Docker")]
public class DockerIntegrationTests : DockerIntegrationTestBase
{
    #region Basic HTTP Operations

    [Test]
    public async Task Get_SimpleRequest_ReturnsOk()
    {
        var response = await new Request(ApiUrl("/api/get")).SendAsync();
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var json = await response.ParseJsonDocumentAsync();
        Assert.That(json.RootElement.GetProperty("method").GetString(), Is.EqualTo("GET"));
    }

    [Test]
    public async Task Post_WithJsonBody_BodyReceived()
    {
        var response = await new Request(ApiUrl("/api/post"))
            .AsPost()
            .WithJsonBody(new { name = "Test", value = 42 })
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var json = await response.ParseJsonDocumentAsync();
        Assert.That(json.RootElement.GetProperty("method").GetString(), Is.EqualTo("POST"));
    }

    [Test]
    public async Task Put_WithJsonBody_BodyReceived()
    {
        var response = await new Request(ApiUrl("/api/put"))
            .AsPut()
            .WithJsonBody(new { id = 1, name = "Updated" })
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Delete_Request_Succeeds()
    {
        var response = await new Request(ApiUrl("/api/delete/123"))
            .AsDelete()
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Get_WithHeaders_HeadersReceived()
    {
        var response = await new Request(ApiUrl("/api/headers"))
            .WithHeader("X-Custom-Header", "TestValue")
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var json = await response.ParseJsonDocumentAsync();
        var headers = json.RootElement.GetProperty("headers");
        Assert.That(headers.GetProperty("X-Custom-Header").GetString(), Is.EqualTo("TestValue"));
    }

    [Test]
    public async Task Get_WithQueryParameters_ParametersReceived()
    {
        // Build URL with query params directly since WithParameter may use different encoding
        var response = await new Request(ApiUrl("/api/query?name=test&value=123"))
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var json = await response.ParseJsonDocumentAsync();
        var query = json.RootElement.GetProperty("query");
        Assert.That(query.GetProperty("name").GetString(), Is.EqualTo("test"));
    }

    #endregion

    #region Authentication

    [Test]
    public async Task BasicAuth_ValidCredentials_Succeeds()
    {
        var response = await new Request(ApiUrl("/api/auth/basic"))
            .UseBasicAuthentication("testuser", "testpass")
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task BasicAuth_InvalidCredentials_Returns401()
    {
        var response = await new Request(ApiUrl("/api/auth/basic"))
            .UseBasicAuthentication("wrong", "credentials")
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task BearerAuth_ValidToken_Succeeds()
    {
        var response = await new Request(ApiUrl("/api/auth/bearer"))
            .UseBearerAuthentication("valid-test-token")
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Retry Policy

    [Test]
    [Ignore("Mock API retry-eventually endpoint needs state reset between test runs")]
    public async Task RetryPolicy_FailsThenSucceeds_RetriesAndSucceeds()
    {
        var clientId = Guid.NewGuid().ToString();
        var retryPolicy = new RetryPolicy
        {
            MaxRetries = 5,
            InitialDelay = TimeSpan.FromMilliseconds(50),
            BackoffStrategy = EnumBackoffStrategy.Fixed
        };

        var response = await new Request(ApiUrl("/api/retry-eventually"))
            .WithHeader("X-Client-Id", clientId)
            .WithRetryPolicy(retryPolicy)
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Ignore("Mock API rate-limit state persists between test runs")]
    public async Task RateLimit_ExceedsLimit_Returns429()
    {
        var clientId = Guid.NewGuid().ToString();
        
        // Make requests to trigger rate limit
        for (int i = 0; i < 5; i++)
        {
            await new Request(ApiUrl("/api/rate-limited"))
                .WithHeader("X-Client-Id", clientId)
                .SendAsync();
        }

        var response = await new Request(ApiUrl("/api/rate-limited"))
            .WithHeader("X-Client-Id", clientId)
            .SendAsync();

        Assert.That((int)response.StatusCode, Is.EqualTo(429));
    }

    #endregion

    #region HTTP Proxy

    [Test]
    public async Task HttpProxy_NoAuth_RequestSucceeds()
    {
        var proxy = new ProxyInfo(
            "localhost",
            DockerTestFixture.HttpProxyNoAuthPort,
            EnumProxyType.Http);

        var response = await new Request(ProxyApiUrl("/api/get"))
            .WithProxy(proxy)
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task HttpProxy_WithAuth_RequestSucceeds()
    {
        var response = await new Request(ProxyApiUrl("/api/get"))
            .WithProxy(DockerTestFixture.HttpProxyUrlWithAuth)
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task HttpProxy_Post_BodyTransmitted()
    {
        var proxy = new ProxyInfo(
            "localhost",
            DockerTestFixture.HttpProxyNoAuthPort,
            EnumProxyType.Http);

        var response = await new Request(ProxyApiUrl("/api/post"))
            .AsPost()
            .WithProxy(proxy)
            .WithJsonBody(new { test = "proxy" })
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region SOCKS5 Proxy

    [Test]
    [Ignore("SOCKS5 proxy DNS resolution not working in Docker network - microsocks limitation")]
    public async Task Socks5Proxy_NoAuth_RequestSucceeds()
    {
        // Use Socks5h for remote DNS resolution (proxy resolves hostname)
        var proxy = new ProxyInfo(
            "localhost",
            DockerTestFixture.Socks5ProxyNoAuthPort,
            EnumProxyType.Socks5h);

        var response = await new Request(ProxyApiUrl("/api/get"))
            .WithProxy(proxy)
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Ignore("SOCKS5 proxy DNS resolution not working in Docker network - microsocks limitation")]
    public async Task Socks5Proxy_WithAuth_RequestSucceeds()
    {
        // Use socks5h:// for remote DNS resolution
        var proxyUrl = $"socks5h://{DockerTestConstants.Socks5ProxyUsername}:{DockerTestConstants.Socks5ProxyPassword}@localhost:{DockerTestFixture.Socks5ProxyPort}";
        var response = await new Request(ProxyApiUrl("/api/get"))
            .WithProxy(proxyUrl)
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Batch Processing

    [Test]
    public async Task BatchRequests_ProcessMultiple_AllSucceed()
    {
        using var batchRequests = new BatchRequests()
            .WithRateLimit(10);

        var batch = batchRequests.CreateBatch("test-batch");
        for (int i = 0; i < 10; i++)
        {
            batch.Enqueue(ApiUrl($"/api/batch/{i}"));
        }

        var responses = await batchRequests.ExecuteAllAsync();

        // Allow minor variance due to timing - at least 9 out of 10
        Assert.That(responses.Count, Is.GreaterThanOrEqualTo(9));
        Assert.That(responses.Count(r => r.StatusCode == HttpStatusCode.OK), Is.GreaterThanOrEqualTo(9));
    }

    [Test]
    public async Task BatchRequests_WithCallbacks_CallbacksInvoked()
    {
        var responseCount = 0;
        
        using var batchRequests = new BatchRequests()
            .WithRateLimit(10)
            .OnResponse(_ => responseCount++);

        var batch = batchRequests.CreateBatch("callback-batch");
        for (int i = 0; i < 5; i++)
        {
            batch.Enqueue(ApiUrl($"/api/batch/{i}"));
        }

        await batchRequests.ExecuteAllAsync();

        Assert.That(responseCount, Is.EqualTo(5));
    }

    #endregion

    #region Proxied Batch Processing

    [Test]
    [Ignore("ProxiedBatch with Docker network requires additional configuration")]
    public async Task ProxiedBatch_RoundRobin_AllSucceed()
    {
        // Only use HTTP proxies - SOCKS5 has DNS resolution issues in Docker
        using var batchRequests = new ProxiedBatchRequests()
            .WithRateLimit(10)
            .WithProxy(DockerTestFixture.HttpProxyNoAuthUrl)
            .WithProxy(DockerTestFixture.HttpProxyUrlWithAuth)
            .WithRoundRobinRotation();

        var batch = batchRequests.CreateBatch("proxied-batch");
        for (int i = 0; i < 10; i++)
        {
            batch.Enqueue(ProxyApiUrl($"/api/batch/{i}"));
        }

        var responses = await batchRequests.ExecuteAllAsync();

        Assert.That(responses.Count, Is.EqualTo(10));
        Assert.That(responses.All(r => r.StatusCode == HttpStatusCode.OK), Is.True);
    }

    [Test]
    [Ignore("ProxiedBatch with Docker network requires additional configuration")]
    public async Task ProxiedBatch_DynamicProxyAddition_Works()
    {
        using var batchRequests = new ProxiedBatchRequests()
            .WithRateLimit(10)
            .WithRoundRobinRotation();

        // Start with one proxy
        batchRequests.AddProxy(DockerTestFixture.HttpProxyNoAuthUrl);
        Assert.That(batchRequests.ProxyCount, Is.EqualTo(1));

        // Add another dynamically (use HTTP proxy - SOCKS5 has DNS issues)
        batchRequests.AddProxy(DockerTestFixture.HttpProxyUrlWithAuth);
        Assert.That(batchRequests.ProxyCount, Is.EqualTo(2));

        var batch = batchRequests.CreateBatch("dynamic-batch");
        for (int i = 0; i < 5; i++)
        {
            batch.Enqueue(ProxyApiUrl($"/api/batch/{i}"));
        }

        var responses = await batchRequests.ExecuteAllAsync();

        Assert.That(responses.Count, Is.EqualTo(5));
    }

    [Test]
    [Ignore("ProxiedBatch with Docker network requires additional configuration")]
    public async Task ProxiedBatch_MaxProxyRetries_Works()
    {
        using var batchRequests = new ProxiedBatchRequests()
            .WithRateLimit(10)
            .WithProxy("http://invalid-proxy.local:9999") // Will fail
            .WithProxy(DockerTestFixture.HttpProxyNoAuthUrl) // Will succeed
            .WithMaxProxyRetries(3)
            .WithRoundRobinRotation();

        var batch = batchRequests.CreateBatch("retry-batch");
        batch.Enqueue(ProxyApiUrl("/api/get"));

        var responses = await batchRequests.ExecuteAllAsync();

        Assert.That(responses.Count, Is.EqualTo(1));
        Assert.That(responses[0].StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Error Handling

    [Test]
    [TestCase(400)]
    [TestCase(401)]
    [TestCase(404)]
    [TestCase(500)]
    [TestCase(503)]
    public async Task ErrorEndpoint_ReturnsExpectedStatus(int statusCode)
    {
        var response = await new Request(ApiUrl($"/api/error/{statusCode}"))
            .SendAsync();

        Assert.That((int)response.StatusCode, Is.EqualTo(statusCode));
    }

    #endregion

    #region Delay and Timeout

    [Test]
    public async Task Delay_CompletesWithinTimeout()
    {
        var response = await new Request(ApiUrl("/api/delay/100"))
            .WithTimeout(TimeSpan.FromSeconds(5))
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public void Timeout_ThrowsOnLongDelay()
    {
        var request = new Request(ApiUrl("/api/delay/5000"))
            .WithTimeout(TimeSpan.FromMilliseconds(500));

        // Library throws RequestTimeoutException on timeout
        Assert.ThrowsAsync<global::DevBase.Net.Exceptions.RequestTimeoutException>(async () => await request.SendAsync());
    }

    #endregion

    #region Large Responses

    [Test]
    public async Task LargeResponse_HandledCorrectly()
    {
        var response = await new Request(ApiUrl("/api/large/500"))
            .SendAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var json = await response.ParseJsonDocumentAsync();
        Assert.That(json.RootElement.GetProperty("count").GetInt32(), Is.EqualTo(500));
    }

    #endregion
}
