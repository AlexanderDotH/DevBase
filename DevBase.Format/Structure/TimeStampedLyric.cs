using System.Diagnostics.CodeAnalysis;

namespace DevBase.Format.Structure;

public class TimeStampedLyric
{
    public string Text { get; set; }
    public TimeSpan StartTime { get; set; }

    public long StartTimestamp
    {
        get => Convert.ToInt64(StartTime.TotalMilliseconds);
    }
}