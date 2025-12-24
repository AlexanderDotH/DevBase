namespace DevBase.Net.Batch;

public sealed record BatchStatistics(
    int BatchCount,
    int TotalQueuedRequests,
    int ProcessedRequests,
    int ErrorCount,
    Dictionary<string, int> RequestsPerBatch
)
{
    public double SuccessRate => ProcessedRequests > 0 
        ? (double)(ProcessedRequests - ErrorCount) / ProcessedRequests * 100 
        : 0;
}
