using DevBase.Generics;

namespace DevBase.Format.Structure;

/// <summary>
/// Represents a lyric line with start/end times and individual word timestamps.
/// </summary>
public class RichTimeStampedLyric
{
    /// <summary>
    /// Gets or sets the full text of the lyric line.
    /// </summary>
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the start time of the lyric line.
    /// </summary>
    public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
    
    /// <summary>
    /// Gets or sets the end time of the lyric line.
    /// </summary>
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
    
    /// <summary>
    /// Gets the start timestamp in total milliseconds.
    /// </summary>
    public long StartTimestamp
    {
        get => Convert.ToInt64(StartTime.TotalMilliseconds);
    }
    
    /// <summary>
    /// Gets the end timestamp in total milliseconds.
    /// </summary>
    public long EndTimestamp
    {
        get => Convert.ToInt64(EndTime.TotalMilliseconds);
    }
    
    /// <summary>
    /// Gets or sets the list of words with their own timestamps within this line.
    /// </summary>
    public AList<RichTimeStampedWord> Words { get; set; } = new AList<RichTimeStampedWord>();
}