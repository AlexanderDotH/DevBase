using System.Text;
using DevBase.Format.Formats.RmmlFormat.Json;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Newtonsoft.Json;

namespace DevBase.Format.Formats.RmmlFormat;

/// <summary>
/// Parser for the RMML (Rich Musixmatch Lyric?) or similar JSON-based rich lyric format.
/// Parses JSON content with character-level offsets.
/// </summary>
public class RmmlParser : FileFormat<string, AList<RichTimeStampedLyric>>
{
    /// <summary>
    /// Parses the RMML JSON string content into a list of rich time-stamped lyrics.
    /// </summary>
    /// <param name="from">The JSON string content.</param>
    /// <returns>A list of <see cref="RichTimeStampedLyric"/> objects.</returns>
    public override AList<RichTimeStampedLyric> Parse(string from)
    {
        RichSyncLine[] parsedLyrics = JsonConvert.DeserializeObject<RichSyncLine[]>(from);
        
        if (parsedLyrics == null)
            return default;

        AList<RichTimeStampedLyric> richLyrics = new AList<RichTimeStampedLyric>();

        for (var i = 0; i < parsedLyrics.Length; i++)
        {
            RichSyncLine r = parsedLyrics[i];
            
            TimeSpan rStartTime = TimeSpan.FromSeconds(r.TimeStart);
            TimeSpan rEndTime = TimeSpan.FromSeconds(r.TimeEnd);
            
            RichTimeStampedLyric element = new RichTimeStampedLyric()
            {
                StartTime = rStartTime,
                EndTime = rEndTime,
                Text = r.FullLine,
                Words = new AList<RichTimeStampedWord>()
            };
            
            if (r.SingleCharOffsets != null && r.SingleCharOffsets.Count != 0)
            {
                TimeSpan lastOffset = TimeSpan.Zero;
                
                for (var j = 0; j < r.SingleCharOffsets.Count; j++)
                {
                    RichSyncChar l = r.SingleCharOffsets[j];

                    TimeSpan lOffset = TimeSpan.FromSeconds(l.Offset);
                    
                    TimeSpan lStartTime = rStartTime + lastOffset;
                    TimeSpan lEndTime = rStartTime + lOffset;
                    
                    RichTimeStampedWord richWord = new RichTimeStampedWord()
                    {
                        StartTime = lStartTime,
                        EndTime = lEndTime,
                        Word = l.Char
                    };
                    
                    element.Words.Add(richWord);
                    lastOffset = lOffset;
                }
            }
            
            richLyrics.Add(element);
        }
        
        return richLyrics;
    }

    /// <summary>
    /// Attempts to parse the RMML JSON string content.
    /// </summary>
    /// <param name="from">The JSON string content.</param>
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