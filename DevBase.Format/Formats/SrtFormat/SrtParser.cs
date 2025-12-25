using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.SrtFormat;

/// <summary>
/// Parser for the SRT (SubRip Subtitle) file format.
/// Parses SRT content into a list of rich time-stamped lyrics (with start and end times).
/// </summary>
public class SrtParser : FileFormat<string, AList<RichTimeStampedLyric>>    
{
    /// <summary>
    /// Parses the SRT string content into a list of rich time-stamped lyrics.
    /// </summary>
    /// <param name="from">The SRT string content.</param>
    /// <returns>A list of <see cref="RichTimeStampedLyric"/> objects.</returns>
    public override AList<RichTimeStampedLyric> Parse(string from)
    {
        AList<string> lines = new AString(from).AsList();
        AList<AList<string>> sliced = lines.Slice(4);

        AList<RichTimeStampedLyric> richTimeStampedLyrics = new AList<RichTimeStampedLyric>();

        for (int i = 0; i < sliced.Length; i++)
        {
            AList<string> currentList = sliced.Get(i);

            if (currentList.Length == 4)
            {
                Match match = RegexHolder.RegexSrtTimeStamps.Match(currentList.Get(1));
                
                TimeSpan startTime = TimeSpan.Parse(match.Groups[1].Value);
                TimeSpan endTime = TimeSpan.Parse(match.Groups[4].Value);

                RichTimeStampedLyric timeStampedLyric = new RichTimeStampedLyric()
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    Text = currentList.Get(2)
                };

                richTimeStampedLyrics.Add(timeStampedLyric);
            }
        }

        return richTimeStampedLyrics;
    }

    /// <summary>
    /// Attempts to parse the SRT string content.
    /// </summary>
    /// <param name="from">The SRT string content.</param>
    /// <param name="parsed">The parsed list of lyrics, or null if parsing fails.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public override bool TryParse(string from, out AList<RichTimeStampedLyric> parsed)
    {
        AList<RichTimeStampedLyric> p = Parse(from);
        
        if (p == null || p.IsEmpty())
        {
            parsed = null;
            return Error<bool>("The parsed lyrics are null or empty");
        }

        parsed = p;
        return true;
    }
}