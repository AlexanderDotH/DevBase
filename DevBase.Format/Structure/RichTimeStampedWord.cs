namespace DevBase.Format.Structure;

/// <summary>
/// Represents a single word in a lyric with start and end times.
/// </summary>
public class RichTimeStampedWord
{
    /// <summary>
    /// Gets or sets the word text.
    /// </summary>
    public string Word { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the start time of the word.
    /// </summary>
    public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
    
    /// <summary>
    /// Gets or sets the end time of the word.
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
}