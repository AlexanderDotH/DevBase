namespace DevBase.Net.Batch;

/// <summary>
/// Provides information about the progress of a batch execution.
/// </summary>
public sealed record BatchProgressInfo(
    string BatchName,
    int Completed,
    int Total,
    int Errors
)
{
    /// <summary>
    /// Gets the percentage of requests completed.
    /// </summary>
    public double PercentComplete => Total > 0 ? (double)Completed / Total * 100 : 0;
    
    /// <summary>
    /// Gets the number of requests remaining.
    /// </summary>
    public int Remaining => Total - Completed;
}
