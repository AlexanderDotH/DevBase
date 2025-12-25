namespace DevBase.Net.Batch;

/// <summary>
/// Provides statistical information about the batch engine's operation.
/// </summary>
public sealed record BatchStatistics(
    int BatchCount,
    int TotalQueuedRequests,
    int ProcessedRequests,
    int ErrorCount,
    Dictionary<string, int> RequestsPerBatch
)
{
    /// <summary>
    /// Gets the success rate percentage of processed requests.
    /// </summary>
    public double SuccessRate => ProcessedRequests > 0 
        ? (double)(ProcessedRequests - ErrorCount) / ProcessedRequests * 100 
        : 0;
}
