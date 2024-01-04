using System.Text;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Typography;

namespace DevBase.Format.Formats.RlrcFormat;

// Don't ask me why I made a parser for just a file full of \n. It just fits into the ecosystem
public class RlrcParser : RevertableFileFormat<string, AList<RawLyric>>
{
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