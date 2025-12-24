namespace DevBase.Net.Batch.Proxied;

public sealed record ProxiedBatchStatistics(
    int BatchCount,
    int TotalQueuedRequests,
    int ProcessedRequests,
    int ErrorCount,
    int ProxyFailureCount,
    int TotalProxies,
    int AvailableProxies,
    Dictionary<string, int> RequestsPerBatch
)
{
    public double SuccessRate => ProcessedRequests > 0
        ? (double)(ProcessedRequests - ErrorCount) / ProcessedRequests * 100
        : 0;

    public double ProxyAvailabilityRate => TotalProxies > 0
        ? (double)AvailableProxies / TotalProxies * 100
        : 0;
}
