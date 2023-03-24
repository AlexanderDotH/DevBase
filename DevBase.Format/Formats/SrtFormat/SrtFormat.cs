using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.SrtFormat;

public class SrtFormat : IFileFormat<AList<PreciseLyricElement>>
{
    public AList<PreciseLyricElement> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }

    public AList<PreciseLyricElement> FormatFromString(string lyricString)
    {
        AList<string> lines = new AString(lyricString).AsList();
        AList<AList<string>> sliced = lines.Slice(4);

        AList<PreciseLyricElement> elements = new AList<PreciseLyricElement>();

        for (int i = 0; i < sliced.Length; i++)
        {
            AList<string> currentList = sliced.Get(i);

            if (currentList.Length == 4)
            {
                Match match = Regex.Match(currentList.Get(1), 
                    RegexHolder.REGEX_SRT_TIMESTAMPS);

                TimeSpan startTime = TimeSpan.Parse(match.Groups[1].Value);
                TimeSpan endTime = TimeSpan.Parse(match.Groups[2].Value);

                elements.Add(new PreciseLyricElement(
                    Convert.ToInt64(startTime.TotalMilliseconds),
                    Convert.ToInt64(endTime.TotalMilliseconds),
                    currentList.Get(2)));
            }
        }

        return elements;
    }
}