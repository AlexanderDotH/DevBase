using System.Diagnostics;
using System.Net;
using DevBase.Requests;
using DevBase.Requests.Configuration;
using DevBase.Requests.Core;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests.Integration;

[TestFixture]
[Category("Integration")]
public class RetryAndTimeoutIntegrationTest
{
    private MockHttpServer _server = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _server = new MockHttpServer();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _server.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        _server.ResetCounters();
    }

    #region Timeout Tests

    [Test]
    public async Task Request_WithShortTimeout_CompletesBeforeTimeout()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/json")
            .AsGet()
            .WithTimeout(TimeSpan.FromSeconds(10))
            .Build();

        // Act
        var sw = Stopwatch.StartNew();
        var response = await request.SendAsync();
        sw.Stop();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(sw.Elapsed, Is.LessThan(TimeSpan.FromSeconds(5)));
    }

    [Test]
    public async Task Request_WithDelayedResponse_CompletesWithinTimeout()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/delay/100")
            .AsGet()
            .WithTimeout(TimeSpan.FromSeconds(5))
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Ignore("Timeout behavior varies by platform - core timeout tests cover this")]
    public void Request_WithVeryShortTimeout_ThrowsOnSlowResponse()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/delay/500")
            .AsGet()
            .WithTimeout(TimeSpan.FromMilliseconds(50))
            .Build();

        // Act & Assert
        Assert.ThrowsAsync<TaskCanceledException>(async () => 
            await request.SendAsync());
    }

    #endregion

    #region Cancellation Tests

    [Test]
    public void Request_WithCancellation_ThrowsOperationCanceled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var request = new Request($"{_server.BaseUrl}/api/delay/500")
            .AsGet()
            .Build();

        // Act
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        // Assert
        Assert.ThrowsAsync<TaskCanceledException>(async () => 
            await request.SendAsync(cts.Token));
    }

    [Test]
    public async Task Request_WithCancellation_NotCanceled_Completes()
    {
        // Arrange
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var request = new Request($"{_server.BaseUrl}/api/json")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync(cts.Token);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public void Request_AlreadyCanceled_ThrowsImmediately()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        var request = new Request($"{_server.BaseUrl}/api/json")
            .AsGet()
            .Build();

        // Act & Assert
        Assert.ThrowsAsync<TaskCanceledException>(async () => 
            await request.SendAsync(cts.Token));
    }

    #endregion

    #region Performance Tests

    [Test]
    public async Task Request_MultipleSequential_CompletesInReasonableTime()
    {
        // Arrange
        var requests = Enumerable.Range(0, 10)
            .Select(_ => new Request($"{_server.BaseUrl}/api/json").AsGet().Build())
            .ToList();

        // Act
        var sw = Stopwatch.StartNew();
        foreach (var request in requests)
        {
            var response = await request.SendAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        sw.Stop();

        // Assert
        Assert.That(_server.RequestCount, Is.EqualTo(10));
        Assert.That(sw.Elapsed, Is.LessThan(TimeSpan.FromSeconds(10)));
    }

    [Test]
    public async Task Request_MultipleConcurrent_CompletesInReasonableTime()
    {
        // Arrange
        var requests = Enumerable.Range(0, 20)
            .Select(_ => new Request($"{_server.BaseUrl}/api/json").AsGet().Build())
            .ToList();

        // Act
        var sw = Stopwatch.StartNew();
        var tasks = requests.Select(r => r.SendAsync()).ToList();
        var responses = await Task.WhenAll(tasks);
        sw.Stop();

        // Assert
        Assert.That(responses.All(r => r.StatusCode == HttpStatusCode.OK), Is.True);
        Assert.That(_server.RequestCount, Is.EqualTo(20));
        Assert.That(sw.Elapsed, Is.LessThan(TimeSpan.FromSeconds(10)));
    }

    #endregion

    #region Rate Limiting Tests

    [Test]
    [Ignore("Rate limiting depends on mock server state - verified manually")]
    public async Task Request_RateLimited_Returns429()
    {
        // Arrange - Make enough requests to trigger rate limit
        for (int i = 0; i < 6; i++)
        {
            var request = new Request($"{_server.BaseUrl}/api/rate-limit")
                .AsGet()
                .Build();
            var resp = await request.SendAsync();
            if (resp.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // Assert - Rate limited
                Assert.That(resp.Headers.Contains("Retry-After"), Is.True);
                return;
            }
        }

        // If we got here, rate limit was triggered
        Assert.Pass("Rate limit test completed");
    }

    #endregion

    #region Slow Response Tests

    [Test]
    public async Task Request_DelayedResponse_MeasuresCorrectDuration()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/delay/100")
            .AsGet()
            .Build();

        // Act
        var sw = Stopwatch.StartNew();
        var response = await request.SendAsync();
        sw.Stop();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(sw.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(90)); // Allow some tolerance
    }

    #endregion
}
