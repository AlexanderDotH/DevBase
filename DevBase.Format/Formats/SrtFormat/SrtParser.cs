using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.SrtFormat;

public class SrtParser : FileFormat<string, AList<RichTimeStampedLyric>>
{
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
                Match match = Regex.Match(currentList.Get(1), 
                    RegexHolder.REGEX_SRT_TIMESTAMPS);
                
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

    public override bool TryParse(string from, out AList<RichTimeStampedLyric> parsed)
    {
        AList<RichTimeStampedLyric> p = Parse(from);
        
        if (p == null || p.IsEmpty())
        {
            parsed = null;
            return Error("The parsed lyrics are null or empty");
        }

        parsed = p;
        return true;
    }
}