using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.SrtFormat;

public class SrtParser : IFileFormat<AList<RichTimeStampedLyric>>
{
    public AList<RichTimeStampedLyric> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }

    public AList<RichTimeStampedLyric> FormatFromString(string lyricString)
    {
        AList<string> lines = new AString(lyricString).AsList();
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

    public string FormatToString(AList<RichTimeStampedLyric> content)
    {
        throw new NotSupportedException("Not supported yet, it will be implemented if necessary");
    }
}