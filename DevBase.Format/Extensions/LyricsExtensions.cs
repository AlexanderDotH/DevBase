using System.Text;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Utilities;

namespace DevBase.Format.Extensions;

/// <summary>
/// Provides extension methods for converting between different lyric structures and text formats.
/// </summary>
public static class LyricsExtensions
{
    /// <summary>
    /// Converts a list of raw lyrics to a plain text string.
    /// </summary>
    /// <param name="rawElements">The list of raw lyrics.</param>
    /// <returns>A string containing the lyrics.</returns>
    public static string ToPlainText(this AList<RawLyric> rawElements)
    {
        StringBuilder rawLyrics = new StringBuilder();

        for (int i = 0; i < rawElements.Length; i++)
            rawLyrics.AppendLine(rawElements.Get(i).Text);

        return rawLyrics.ToString();
    }
    
    /// <summary>
    /// Converts a list of time-stamped lyrics to a plain text string.
    /// </summary>
    /// <param name="elements">The list of time-stamped lyrics.</param>
    /// <returns>A string containing the lyrics.</returns>
    public static string ToPlainText(this AList<TimeStampedLyric> elements)
    {
        StringBuilder rawLyrics = new StringBuilder();

        for (int i = 0; i < elements.Length; i++)
            rawLyrics.AppendLine(elements.Get(i).Text);

        return rawLyrics.ToString();
    }
    
    /// <summary>
    /// Converts a list of rich time-stamped lyrics to a plain text string.
    /// </summary>
    /// <param name="richElements">The list of rich time-stamped lyrics.</param>
    /// <returns>A string containing the lyrics.</returns>
    public static string ToPlainText(this AList<RichTimeStampedLyric> richElements)
    {
        StringBuilder rawLyrics = new StringBuilder();

        for (int i = 0; i < richElements.Length; i++)
            rawLyrics.AppendLine(richElements.Get(i).Text);

        return rawLyrics.ToString();
    }
    
    /// <summary>
    /// Converts a list of time-stamped lyrics to raw lyrics (removing timestamps).
    /// </summary>
    /// <param name="timeStampedLyrics">The list of time-stamped lyrics.</param>
    /// <returns>A list of raw lyrics.</returns>
    public static AList<RawLyric> ToRawLyrics(this AList<TimeStampedLyric> timeStampedLyrics)
    {
        AList<RawLyric> rawLyrics = new AList<RawLyric>();

        for (int i = 0; i < timeStampedLyrics.Length; i++)
        {
            TimeStampedLyric timeStampedLyric = timeStampedLyrics.Get(i);

            RawLyric rawLyric = new RawLyric()
            {
                Text = timeStampedLyric.Text
            };
            
            rawLyrics.Add(rawLyric);
        }

        return rawLyrics;
    }
    
    /// <summary>
    /// Converts a list of rich time-stamped lyrics to raw lyrics (removing timestamps and extra data).
    /// </summary>
    /// <param name="richTimeStampedLyrics">The list of rich time-stamped lyrics.</param>
    /// <returns>A list of raw lyrics.</returns>
    public static AList<RawLyric> ToRawLyrics(this AList<RichTimeStampedLyric> richTimeStampedLyrics)
    {
        AList<RawLyric> rawLyrics = new AList<RawLyric>();

        for (int i = 0; i < richTimeStampedLyrics.Length; i++)
        {
            RichTimeStampedLyric richTimeStampedLyric = richTimeStampedLyrics.Get(i);

            RawLyric rawLyric = new RawLyric()
            {
                Text = richTimeStampedLyric.Text
            };
            
            rawLyrics.Add(rawLyric);
        }

        return rawLyrics;
    }
    
    /// <summary>
    /// Converts a list of rich time-stamped lyrics to standard time-stamped lyrics (simplifying the structure).
    /// </summary>
    /// <param name="richElements">The list of rich time-stamped lyrics.</param>
    /// <returns>A list of time-stamped lyrics.</returns>
    public static AList<TimeStampedLyric> ToTimeStampedLyrics(this AList<RichTimeStampedLyric> richElements)
    {
        AList<TimeStampedLyric> timeStampedLyrics = new AList<TimeStampedLyric>();

        for (int i = 0; i < richElements.Length; i++)
        {
            RichTimeStampedLyric stampedLyric = richElements.Get(i);

            TimeStampedLyric timeStampedLyric = new TimeStampedLyric()
            {
                Text = stampedLyric.Text,
                StartTime = stampedLyric.StartTime
            };
            
            timeStampedLyrics.Add(timeStampedLyric);
        }
        
        return timeStampedLyrics;
    }
}