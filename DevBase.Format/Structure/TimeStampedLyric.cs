using System.Diagnostics.CodeAnalysis;

namespace DevBase.Format.Structure;

/// <summary>
/// Represents a lyric line with a start time.
/// </summary>
public class TimeStampedLyric
{
    /// <summary>
    /// Gets or sets the text of the lyric line.
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Gets or sets the start time of the lyric line.
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Gets the start timestamp in total milliseconds.
    /// </summary>
    public long StartTimestamp
    {
        get => Convert.ToInt64(StartTime.TotalMilliseconds);
    }
}