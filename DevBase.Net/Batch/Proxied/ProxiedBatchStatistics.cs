namespace DevBase.Net.Batch.Proxied;

/// <summary>
/// Provides statistical information about the proxied batch engine's operation.
/// </summary>
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
    /// <summary>
    /// Gets the success rate percentage of processed requests.
    /// </summary>
    public double SuccessRate => ProcessedRequests > 0
        ? (double)(ProcessedRequests - ErrorCount) / ProcessedRequests * 100
        : 0;

    /// <summary>
    /// Gets the rate of available proxies.
    /// </summary>
    public double ProxyAvailabilityRate => TotalProxies > 0
        ? (double)AvailableProxies / TotalProxies * 100
        : 0;
}
