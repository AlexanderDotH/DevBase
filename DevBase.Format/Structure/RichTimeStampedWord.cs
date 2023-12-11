namespace DevBase.Format.Structure;

public class RichTimeStampedWord
{
    public string Word { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
    
    public long StartTimestamp
    {
        get => Convert.ToInt64(StartTime.TotalMilliseconds);
    }
    
    public long EndTimestamp
    {
        get => Convert.ToInt64(EndTime.TotalMilliseconds);
    }
}