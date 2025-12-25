using System.Text;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Typography;

namespace DevBase.Format.Formats.RlrcFormat;

/// <summary>
/// Parser for the RLRC (Raw Lyrics) file format.
/// Essentially parses a list of lines as raw lyrics.
/// </summary>
// Don't ask me why I made a parser for just a file full of \n. It just fits into the ecosystem
public class RlrcParser : RevertableFileFormat<string, AList<RawLyric>>
{
    /// <summary>
    /// Parses the raw lyric string content into a list of raw lyrics.
    /// </summary>
    /// <param name="from">The raw lyric string content.</param>
    /// <returns>A list of <see cref="RawLyric"/> objects.</returns>
    public override AList<RawLyric> Parse(string from)
    {
        AList<string> lines = new AString(from).AsList();

        AList<RawLyric> parsedRawLyrics = new AList<RawLyric>();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines.Get(i);

            RawLyric rawLyric = new RawLyric()
            {
                Text = line
            };
            
            parsedRawLyrics.Add(rawLyric);
        }

        return parsedRawLyrics;
    }

    /// <summary>
    /// Attempts to parse the raw lyric string content.
    /// </summary>
    /// <param name="from">The raw lyric string content.</param>
    /// <param name="parsed">The parsed list of raw lyrics, or null if parsing fails.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public override bool TryParse(string from, out AList<RawLyric> parsed)
    {
        AList<RawLyric> p = Parse(from);
        
        if (p == null || p.IsEmpty())
        {
            parsed = null;
            return Error<bool>("The parsed lyrics are null or empty");
        }

        parsed = p;
        return true;
    }

    /// <summary>
    /// Reverts a list of raw lyrics back to a string format.
    /// </summary>
    /// <param name="to">The list of raw lyrics to revert.</param>
    /// <returns>The string representation.</returns>
    public override string Revert(AList<RawLyric> to)
    {
        StringBuilder revertedLyrics = new StringBuilder();

        for (int i = 0; i < to.Length; i++)
        {
            RawLyric rawLyric = to.Get(i);
            
            if (rawLyric == null)
                continue;

            revertedLyrics.AppendLine(rawLyric.Text);
        }

        return revertedLyrics.ToString();
    }

    /// <summary>
    /// Attempts to revert a list of raw lyrics to string format.
    /// </summary>
    /// <param name="to">The list of raw lyrics to revert.</param>
    /// <param name="from">The string representation, or null if reverting fails.</param>
    /// <returns>True if reverting was successful; otherwise, false.</returns>
    public override bool TryRevert(AList<RawLyric> to, out string from)
    {
        string r = Revert(to);

        if (string.IsNullOrEmpty(r))
        {
            from = null;
            return Error<bool>("The parsed lyrics are null or empty");
        }

        from = r;
        return true;
    }
}