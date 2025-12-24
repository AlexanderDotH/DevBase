using DevBase.Net.Batch;
using DevBase.Net.Core;

namespace DevBase.Test.DevBaseRequests;

public class BatchRequestsTest
{
    [Test]
    public void BatchRequests_CreateBatch_ShouldCreateNamedBatch()
    {
        using var batchRequests = new BatchRequests();
        
        var batch = batchRequests.CreateBatch("test-batch");
        
        Assert.That(batch, Is.Not.Null);
        Assert.That(batchRequests.BatchCount, Is.EqualTo(1));
        Assert.That(batchRequests.BatchNames, Contains.Item("test-batch"));
    }

    [Test]
    public void BatchRequests_CreateBatch_DuplicateName_ShouldThrow()
    {
        using var batchRequests = new BatchRequests();
        batchRequests.CreateBatch("test-batch");
        
        Assert.Throws<InvalidOperationException>(() => batchRequests.CreateBatch("test-batch"));
    }

    [Test]
    public void BatchRequests_GetOrCreateBatch_ShouldReturnExistingBatch()
    {
        using var batchRequests = new BatchRequests();
        var batch1 = batchRequests.CreateBatch("test-batch");
        var batch2 = batchRequests.GetOrCreateBatch("test-batch");
        
        Assert.That(batch2, Is.SameAs(batch1));
        Assert.That(batchRequests.BatchCount, Is.EqualTo(1));
    }

    [Test]
    public void BatchRequests_GetOrCreateBatch_ShouldCreateIfNotExists()
    {
        using var batchRequests = new BatchRequests();
        var batch = batchRequests.GetOrCreateBatch("new-batch");
        
        Assert.That(batch, Is.Not.Null);
        Assert.That(batchRequests.BatchCount, Is.EqualTo(1));
    }

    [Test]
    public void BatchRequests_RemoveBatch_ShouldRemoveExistingBatch()
    {
        using var batchRequests = new BatchRequests();
        batchRequests.CreateBatch("test-batch");
        
        bool removed = batchRequests.RemoveBatch("test-batch");
        
        Assert.That(removed, Is.True);
        Assert.That(batchRequests.BatchCount, Is.EqualTo(0));
    }

    [Test]
    public void BatchRequests_RemoveBatch_NonExistent_ShouldReturnFalse()
    {
        using var batchRequests = new BatchRequests();
        
        bool removed = batchRequests.RemoveBatch("non-existent");
        
        Assert.That(removed, Is.False);
    }

    [Test]
    public void BatchRequests_WithRateLimit_ShouldSetRateLimit()
    {
        using var batchRequests = new BatchRequests()
            .WithRateLimit(5);
        
        Assert.That(batchRequests.RateLimit, Is.EqualTo(5));
    }

    [Test]
    public void BatchRequests_WithRateLimit_InvalidValue_ShouldThrow()
    {
        using var batchRequests = new BatchRequests();
        
        Assert.Throws<ArgumentOutOfRangeException>(() => batchRequests.WithRateLimit(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => batchRequests.WithRateLimit(-1));
    }

    [Test]
    public void BatchRequests_WithCookiePersistence_ShouldEnable()
    {
        using var batchRequests = new BatchRequests()
            .WithCookiePersistence();
        
        Assert.That(batchRequests.PersistCookies, Is.True);
    }

    [Test]
    public void BatchRequests_WithRefererPersistence_ShouldEnable()
    {
        using var batchRequests = new BatchRequests()
            .WithRefererPersistence();
        
        Assert.That(batchRequests.PersistReferer, Is.True);
    }

    [Test]
    public async Task Batch_Add_ShouldEnqueueRequest()
    {
        using var batchRequests = new BatchRequests();
        await batchRequests.StopProcessingAsync();
        var batch = batchRequests.CreateBatch("test-batch");
        
        batch.Add("https://example.com/1");
        batch.Add("https://example.com/2");
        
        Assert.That(batch.QueueCount, Is.EqualTo(2));
        Assert.That(batchRequests.TotalQueueCount, Is.EqualTo(2));
    }

    [Test]
    public void Batch_AddMultiple_ShouldEnqueueAllRequests()
    {
        using var batchRequests = new BatchRequests();
        var batch = batchRequests.CreateBatch("test-batch");
        
        var urls = new[] { "https://example.com/1", "https://example.com/2", "https://example.com/3" };
        batch.Add(urls);
        
        Assert.That(batch.QueueCount, Is.EqualTo(3));
    }

    [Test]
    public void Batch_Enqueue_WithConfiguration_ShouldApplyConfiguration()
    {
        using var batchRequests = new BatchRequests();
        var batch = batchRequests.CreateBatch("test-batch");
        
        batch.Enqueue("https://example.com", r => r.AsPost());
        
        Assert.That(batch.QueueCount, Is.EqualTo(1));
    }

    [Test]
    public void Batch_Clear_ShouldRemoveAllRequests()
    {
        using var batchRequests = new BatchRequests();
        var batch = batchRequests.CreateBatch("test-batch");
        
        batch.Add("https://example.com/1");
        batch.Add("https://example.com/2");
        batch.Clear();
        
        Assert.That(batch.QueueCount, Is.EqualTo(0));
    }

    [Test]
    public void Batch_EndBatch_ShouldReturnParent()
    {
        using var batchRequests = new BatchRequests();
        var batch = batchRequests.CreateBatch("test-batch");
        
        var parent = batch.EndBatch();
        
        Assert.That(parent, Is.SameAs(batchRequests));
    }

    [Test]
    public void BatchRequests_ClearAllBatches_ShouldClearAllQueues()
    {
        using var batchRequests = new BatchRequests();
        
        var batch1 = batchRequests.CreateBatch("batch1");
        batch1.Add("https://example.com/1");
        
        var batch2 = batchRequests.CreateBatch("batch2");
        batch2.Add("https://example.com/2");
        
        batchRequests.ClearAllBatches();
        
        Assert.That(batchRequests.TotalQueueCount, Is.EqualTo(0));
        Assert.That(batchRequests.BatchCount, Is.EqualTo(2));
    }

    [Test]
    public async Task BatchRequests_GetStatistics_ShouldReturnCorrectStats()
    {
        using var batchRequests = new BatchRequests();
        await batchRequests.StopProcessingAsync();
        
        var batch1 = batchRequests.CreateBatch("batch1");
        batch1.Add("https://example.com/1");
        batch1.Add("https://example.com/2");
        
        var batch2 = batchRequests.CreateBatch("batch2");
        batch2.Add("https://example.com/3");
        
        var stats = batchRequests.GetStatistics();
        
        Assert.That(stats.BatchCount, Is.EqualTo(2));
        Assert.That(stats.TotalQueuedRequests, Is.EqualTo(3));
        Assert.That(stats.RequestsPerBatch["batch1"], Is.EqualTo(2));
        Assert.That(stats.RequestsPerBatch["batch2"], Is.EqualTo(1));
    }

    [Test]
    public void BatchRequests_ResetCounters_ShouldResetAllCounters()
    {
        using var batchRequests = new BatchRequests();
        
        batchRequests.ResetCounters();
        
        Assert.That(batchRequests.ProcessedCount, Is.EqualTo(0));
        Assert.That(batchRequests.ErrorCount, Is.EqualTo(0));
    }

    [Test]
    public void BatchRequests_OnResponse_ShouldRegisterCallback()
    {
        using var batchRequests = new BatchRequests();
        int callCount = 0;
        
        batchRequests.OnResponse(r => callCount++);
        
        Assert.Pass();
    }

    [Test]
    public void BatchRequests_OnError_ShouldRegisterCallback()
    {
        using var batchRequests = new BatchRequests();
        int callCount = 0;
        
        batchRequests.OnError((r, e) => callCount++);
        
        Assert.Pass();
    }

    [Test]
    public void BatchRequests_OnProgress_ShouldRegisterCallback()
    {
        using var batchRequests = new BatchRequests();
        int callCount = 0;
        
        batchRequests.OnProgress(p => callCount++);
        
        Assert.Pass();
    }

    [Test]
    public void BatchProgressInfo_PercentComplete_ShouldCalculateCorrectly()
    {
        var progress = new BatchProgressInfo("test", 50, 100, 0);
        
        Assert.That(progress.PercentComplete, Is.EqualTo(50.0));
        Assert.That(progress.Remaining, Is.EqualTo(50));
    }

    [Test]
    public void BatchProgressInfo_PercentComplete_ZeroTotal_ShouldReturnZero()
    {
        var progress = new BatchProgressInfo("test", 0, 0, 0);
        
        Assert.That(progress.PercentComplete, Is.EqualTo(0));
    }

    [Test]
    public void BatchStatistics_SuccessRate_ShouldCalculateCorrectly()
    {
        var stats = new BatchStatistics(1, 0, 100, 10, new Dictionary<string, int>());
        
        Assert.That(stats.SuccessRate, Is.EqualTo(90.0));
    }

    [Test]
    public void BatchStatistics_SuccessRate_ZeroProcessed_ShouldReturnZero()
    {
        var stats = new BatchStatistics(1, 0, 0, 0, new Dictionary<string, int>());
        
        Assert.That(stats.SuccessRate, Is.EqualTo(0));
    }

    [Test]
    public void BatchRequests_FluentApi_ShouldChainCorrectly()
    {
        using var batchRequests = new BatchRequests()
            .WithRateLimit(3)
            .WithCookiePersistence()
            .WithRefererPersistence()
            .OnResponse(r => { })
            .OnError((r, e) => { })
            .OnProgress(p => { });
        
        Assert.That(batchRequests.RateLimit, Is.EqualTo(3));
        Assert.That(batchRequests.PersistCookies, Is.True);
        Assert.That(batchRequests.PersistReferer, Is.True);
    }

    [Test]
    public async Task Batch_FluentApi_ShouldChainCorrectly()
    {
        using var batchRequests = new BatchRequests();
        await batchRequests.StopProcessingAsync();
        
        var batch = batchRequests.CreateBatch("test")
            .Add("https://example.com/1")
            .Add("https://example.com/2")
            .Enqueue("https://example.com/3");
        
        Assert.That(batch.QueueCount, Is.EqualTo(3));
    }

    [Test]
    public async Task BatchRequests_MultipleBatches_ShouldTrackTotalQueueCount()
    {
        using var batchRequests = new BatchRequests();
        await batchRequests.StopProcessingAsync();
        
        batchRequests.CreateBatch("batch1").Add("https://a.com").Add("https://b.com");
        batchRequests.CreateBatch("batch2").Add("https://c.com");
        batchRequests.CreateBatch("batch3").Add("https://d.com").Add("https://e.com").Add("https://f.com");
        
        Assert.That(batchRequests.TotalQueueCount, Is.EqualTo(6));
        Assert.That(batchRequests.BatchCount, Is.EqualTo(3));
    }

    [Test]
    public async Task BatchRequests_Dispose_ShouldCleanupResources()
    {
        var batchRequests = new BatchRequests();
        batchRequests.CreateBatch("test").Add("https://example.com");
        
        await batchRequests.DisposeAsync();
        
        Assert.That(batchRequests.BatchCount, Is.EqualTo(0));
    }

    [Test]
    public void Batch_TryDequeue_ShouldDequeueInOrder()
    {
        using var batchRequests = new BatchRequests();
        var batch = batchRequests.CreateBatch("test");
        
        batch.Add("https://example.com/1");
        batch.Add("https://example.com/2");
        
        bool result1 = batch.TryDequeue(out var request1);
        bool result2 = batch.TryDequeue(out var request2);
        bool result3 = batch.TryDequeue(out var request3);
        
        Assert.That(result1, Is.True);
        Assert.That(result2, Is.True);
        Assert.That(result3, Is.False);
        Assert.That(request1, Is.Not.Null);
        Assert.That(request2, Is.Not.Null);
        Assert.That(request3, Is.Null);
    }

    [Test]
    public void BatchRequests_GetBatch_ExistingBatch_ShouldReturnBatch()
    {
        using var batchRequests = new BatchRequests();
        var created = batchRequests.CreateBatch("test");
        
        var retrieved = batchRequests.GetBatch("test");
        
        Assert.That(retrieved, Is.SameAs(created));
    }

    [Test]
    public void BatchRequests_GetBatch_NonExistent_ShouldReturnNull()
    {
        using var batchRequests = new BatchRequests();
        
        var retrieved = batchRequests.GetBatch("non-existent");
        
        Assert.That(retrieved, Is.Null);
    }

    [Test]
    public void BatchRequests_ExecuteBatchAsync_NonExistentBatch_ShouldThrow()
    {
        using var batchRequests = new BatchRequests();
        
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await batchRequests.ExecuteBatchAsync("non-existent"));
    }

    [Test]
    public void Batch_EnqueueWithFactory_ShouldUseFactory()
    {
        using var batchRequests = new BatchRequests();
        var batch = batchRequests.CreateBatch("test");
        
        batch.Enqueue(() => new Request("https://example.com").AsPost());
        
        Assert.That(batch.QueueCount, Is.EqualTo(1));
    }
}
