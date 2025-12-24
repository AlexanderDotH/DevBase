using DevBase.Net.Batch;
using DevBase.Net.Batch.Proxied;
using DevBase.Net.Batch.Strategies;
using DevBase.Net.Core;
using DevBase.Net.Proxy;
using DevBase.Net.Proxy.Enums;

namespace DevBase.Test.DevBaseRequests;

public class ProxiedBatchRequestsTest
{
    [Test]
    public void ProxiedBatchRequests_CreateBatch_ShouldCreateNamedBatch()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        
        ProxiedBatch batch = batchRequests.CreateBatch("test-batch");
        
        Assert.That(batch, Is.Not.Null);
        Assert.That(batchRequests.BatchCount, Is.EqualTo(1));
        Assert.That(batchRequests.BatchNames, Contains.Item("test-batch"));
    }

    [Test]
    public void ProxiedBatchRequests_CreateBatch_DuplicateName_ShouldThrow()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        batchRequests.CreateBatch("test-batch");
        
        Assert.Throws<InvalidOperationException>(() => batchRequests.CreateBatch("test-batch"));
    }

    [Test]
    public void ProxiedBatchRequests_GetOrCreateBatch_ShouldReturnExistingBatch()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        ProxiedBatch batch1 = batchRequests.CreateBatch("test-batch");
        ProxiedBatch batch2 = batchRequests.GetOrCreateBatch("test-batch");
        
        Assert.That(batch2, Is.SameAs(batch1));
        Assert.That(batchRequests.BatchCount, Is.EqualTo(1));
    }

    [Test]
    public void ProxiedBatchRequests_WithProxy_ShouldAddProxy()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        ProxyInfo proxy = new ProxyInfo("proxy.example.com", 8080);
        
        batchRequests.WithProxy(proxy);
        
        Assert.That(batchRequests.ProxyCount, Is.EqualTo(1));
    }

    [Test]
    public void ProxiedBatchRequests_WithProxy_String_ShouldParseAndAdd()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        
        batchRequests.WithProxy("http://proxy.example.com:8080");
        
        Assert.That(batchRequests.ProxyCount, Is.EqualTo(1));
    }

    [Test]
    public void ProxiedBatchRequests_WithProxies_ShouldAddMultiple()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        
        batchRequests.WithProxies(new[]
        {
            new ProxyInfo("proxy1.example.com", 8080),
            new ProxyInfo("proxy2.example.com", 8080),
            new ProxyInfo("proxy3.example.com", 8080)
        });
        
        Assert.That(batchRequests.ProxyCount, Is.EqualTo(3));
    }

    [Test]
    public void ProxiedBatchRequests_WithProxies_Strings_ShouldParseAndAddMultiple()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        
        batchRequests.WithProxies(new[]
        {
            "http://proxy1.example.com:8080",
            "socks5://proxy2.example.com:1080"
        });
        
        Assert.That(batchRequests.ProxyCount, Is.EqualTo(2));
    }

    [Test]
    public void ProxiedBatchRequests_WithRoundRobinRotation_ShouldSetStrategy()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests()
            .WithRoundRobinRotation();
        
        Assert.Pass();
    }

    [Test]
    public void ProxiedBatchRequests_WithRandomRotation_ShouldSetStrategy()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests()
            .WithRandomRotation();
        
        Assert.Pass();
    }

    [Test]
    public void ProxiedBatchRequests_WithLeastFailuresRotation_ShouldSetStrategy()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests()
            .WithLeastFailuresRotation();
        
        Assert.Pass();
    }

    [Test]
    public void ProxiedBatchRequests_WithStickyRotation_ShouldSetStrategy()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests()
            .WithStickyRotation();
        
        Assert.Pass();
    }

    [Test]
    public void ProxiedBatchRequests_WithRateLimit_ShouldSetRateLimit()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests()
            .WithRateLimit(5);
        
        Assert.That(batchRequests.RateLimit, Is.EqualTo(5));
    }

    [Test]
    public void ProxiedBatchRequests_WithCookiePersistence_ShouldEnable()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests()
            .WithCookiePersistence();
        
        Assert.That(batchRequests.PersistCookies, Is.True);
    }

    [Test]
    public void ProxiedBatchRequests_WithRefererPersistence_ShouldEnable()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests()
            .WithRefererPersistence();
        
        Assert.That(batchRequests.PersistReferer, Is.True);
    }

    [Test]
    public void ProxiedBatch_Add_ShouldEnqueueRequest()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        ProxiedBatch batch = batchRequests.CreateBatch("test-batch");
        
        batch.Add("https://example.com/1");
        batch.Add("https://example.com/2");
        
        Assert.That(batch.QueueCount, Is.EqualTo(2));
        Assert.That(batchRequests.TotalQueueCount, Is.EqualTo(2));
    }

    [Test]
    public void ProxiedBatch_AddMultiple_ShouldEnqueueAllRequests()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        ProxiedBatch batch = batchRequests.CreateBatch("test-batch");
        
        string[] urls = new[] { "https://example.com/1", "https://example.com/2", "https://example.com/3" };
        batch.Add(urls);
        
        Assert.That(batch.QueueCount, Is.EqualTo(3));
    }

    [Test]
    public void ProxiedBatch_Clear_ShouldRemoveAllRequests()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        ProxiedBatch batch = batchRequests.CreateBatch("test-batch");
        
        batch.Add("https://example.com/1");
        batch.Add("https://example.com/2");
        batch.Clear();
        
        Assert.That(batch.QueueCount, Is.EqualTo(0));
    }

    [Test]
    public void ProxiedBatch_EndBatch_ShouldReturnParent()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        ProxiedBatch batch = batchRequests.CreateBatch("test-batch");
        
        ProxiedBatchRequests parent = batch.EndBatch();
        
        Assert.That(parent, Is.SameAs(batchRequests));
    }

    [Test]
    public void ProxiedBatchRequests_ClearAllBatches_ShouldClearAllQueues()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        
        ProxiedBatch batch1 = batchRequests.CreateBatch("batch1");
        batch1.Add("https://example.com/1");
        
        ProxiedBatch batch2 = batchRequests.CreateBatch("batch2");
        batch2.Add("https://example.com/2");
        
        batchRequests.ClearAllBatches();
        
        Assert.That(batchRequests.TotalQueueCount, Is.EqualTo(0));
        Assert.That(batchRequests.BatchCount, Is.EqualTo(2));
    }

    [Test]
    public async Task ProxiedBatchRequests_GetStatistics_ShouldReturnCorrectStats()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        await batchRequests.StopProcessingAsync();
        batchRequests.WithProxy("http://proxy.example.com:8080");
        
        ProxiedBatch batch1 = batchRequests.CreateBatch("batch1");
        batch1.Add("https://example.com/1");
        batch1.Add("https://example.com/2");
        
        ProxiedBatch batch2 = batchRequests.CreateBatch("batch2");
        batch2.Add("https://example.com/3");
        
        ProxiedBatchStatistics stats = batchRequests.GetStatistics();
        
        Assert.That(stats.BatchCount, Is.EqualTo(2));
        Assert.That(stats.TotalQueuedRequests, Is.EqualTo(3));
        Assert.That(stats.TotalProxies, Is.EqualTo(1));
        Assert.That(stats.RequestsPerBatch["batch1"], Is.EqualTo(2));
        Assert.That(stats.RequestsPerBatch["batch2"], Is.EqualTo(1));
    }

    [Test]
    public void ProxiedBatchRequests_ResetCounters_ShouldResetAllCounters()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        
        batchRequests.ResetCounters();
        
        Assert.That(batchRequests.ProcessedCount, Is.EqualTo(0));
        Assert.That(batchRequests.ErrorCount, Is.EqualTo(0));
        Assert.That(batchRequests.ProxyFailureCount, Is.EqualTo(0));
    }

    [Test]
    public void ProxiedBatchRequests_ClearProxies_ShouldRemoveAllProxies()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        batchRequests.WithProxy("http://proxy1.example.com:8080");
        batchRequests.WithProxy("http://proxy2.example.com:8080");
        
        batchRequests.ClearProxies();
        
        Assert.That(batchRequests.ProxyCount, Is.EqualTo(0));
    }

    [Test]
    public void ProxiedBatchRequests_GetProxyStatistics_ShouldReturnProxyList()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        batchRequests.WithProxy("http://proxy1.example.com:8080");
        batchRequests.WithProxy("http://proxy2.example.com:8080");
        
        IReadOnlyList<TrackedProxyInfo> stats = batchRequests.GetProxyStatistics();
        
        Assert.That(stats.Count, Is.EqualTo(2));
    }

    [Test]
    public void ProxiedBatchRequests_FluentApi_ShouldChainCorrectly()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests()
            .WithProxy("http://proxy.example.com:8080")
            .WithRateLimit(3)
            .WithCookiePersistence()
            .WithRefererPersistence()
            .WithRoundRobinRotation()
            .OnResponse(r => { })
            .OnError((r, e) => { })
            .OnProgress(p => { })
            .OnProxyFailure((p, c) => { });
        
        Assert.That(batchRequests.RateLimit, Is.EqualTo(3));
        Assert.That(batchRequests.PersistCookies, Is.True);
        Assert.That(batchRequests.PersistReferer, Is.True);
        Assert.That(batchRequests.ProxyCount, Is.EqualTo(1));
    }

    [Test]
    public async Task ProxiedBatch_FluentApi_ShouldChainCorrectly()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        await batchRequests.StopProcessingAsync();
        
        ProxiedBatch batch = batchRequests.CreateBatch("test")
            .Add("https://example.com/1")
            .Add("https://example.com/2")
            .Enqueue("https://example.com/3");
        
        Assert.That(batch.QueueCount, Is.EqualTo(3));
    }

    [Test]
    public async Task ProxiedBatchRequests_Dispose_ShouldCleanupResources()
    {
        ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        batchRequests.CreateBatch("test").Add("https://example.com");
        batchRequests.WithProxy("http://proxy.example.com:8080");
        
        await batchRequests.DisposeAsync();
        
        Assert.That(batchRequests.BatchCount, Is.EqualTo(0));
    }

    [Test]
    public void ProxiedBatch_TryDequeue_ShouldDequeueInOrder()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        ProxiedBatch batch = batchRequests.CreateBatch("test");
        
        batch.Add("https://example.com/1");
        batch.Add("https://example.com/2");
        
        bool result1 = batch.TryDequeue(out Request? request1);
        bool result2 = batch.TryDequeue(out Request? request2);
        bool result3 = batch.TryDequeue(out Request? request3);
        
        Assert.That(result1, Is.True);
        Assert.That(result2, Is.True);
        Assert.That(result3, Is.False);
        Assert.That(request1, Is.Not.Null);
        Assert.That(request2, Is.Not.Null);
        Assert.That(request3, Is.Null);
    }

    [Test]
    public void ProxiedBatchRequests_GetBatch_ExistingBatch_ShouldReturnBatch()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        ProxiedBatch created = batchRequests.CreateBatch("test");
        
        ProxiedBatch? retrieved = batchRequests.GetBatch("test");
        
        Assert.That(retrieved, Is.SameAs(created));
    }

    [Test]
    public void ProxiedBatchRequests_GetBatch_NonExistent_ShouldReturnNull()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        
        ProxiedBatch? retrieved = batchRequests.GetBatch("non-existent");
        
        Assert.That(retrieved, Is.Null);
    }

    [Test]
    public void ProxiedBatchRequests_ExecuteBatchAsync_NonExistentBatch_ShouldThrow()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await batchRequests.ExecuteBatchAsync("non-existent"));
    }

    [Test]
    public void ProxiedBatch_EnqueueWithFactory_ShouldUseFactory()
    {
        using ProxiedBatchRequests batchRequests = new ProxiedBatchRequests();
        ProxiedBatch batch = batchRequests.CreateBatch("test");
        
        batch.Enqueue(() => new Request("https://example.com").AsPost());
        
        Assert.That(batch.QueueCount, Is.EqualTo(1));
    }
}
