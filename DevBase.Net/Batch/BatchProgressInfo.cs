namespace DevBase.Net.Batch;

public sealed record BatchProgressInfo(
    string BatchName,
    int Completed,
    int Total,
    int Errors
)
{
    public double PercentComplete => Total > 0 ? (double)Completed / Total * 100 : 0;
    public int Remaining => Total - Completed;
}
