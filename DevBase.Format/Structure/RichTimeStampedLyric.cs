using DevBase.Generics;

namespace DevBase.Format.Structure;

public class RichTimeStampedLyric
{
    public string Text { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;

    public AList<RichTimeStampedWord> Words { get; set; } = new AList<RichTimeStampedWord>();

    public long StartTimestamp
    {
        get => Convert.ToInt64(StartTime.TotalMilliseconds);
    }
    
    public long EndTimestamp
    {
        get => Convert.ToInt64(EndTime.TotalMilliseconds);
    }
}