using System.Runtime.CompilerServices;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.KLyricsFormat;

public class KLyricsParser : IFileFormat<AList<RichLyrics>>
{
    public AList<RichLyrics> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }

    public AList<RichLyrics> FormatFromString(string lyricString)
    {
        
    }

    private int GetFirstLine(string lyricString)
    {
        AList<string> lines = new AString(lyricString).AsList();

        for (int i = 0; i < lines.Length; i++)
        {
            string c = lines.Get(i);

            (TimeSpan StartTime, TimeSpan EndTime) timeSpans = ParseTimeStamp(lyricString);
            
        }
    }

    
    
    public (TimeSpan StartTime, TimeSpan EndTime) ParseTimeStamp(string line)
    {
        string startTime = string.Empty;
        string endTime = string.Empty;
        
        if (line[0] == '[')
        {
            if (!line.Contains(","))
                return (TimeSpan.Zero, TimeSpan.Zero);

            int s = 0;
                
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == ',')
                {
                    startTime = line.Substring(1, j);
                    s = j;
                } 
                else if (line[j] == ']')
                {
                    endTime = line.Substring(s, line.Length);
                }
            }
        }

        int startTimestamp;

        if (!int.TryParse(startTime, out startTimestamp))
            throw new System.Exception("Cannot parse timestamp");
        
        int endTimestamp;

        if (!int.TryParse(endTime, out endTimestamp))
            throw new System.Exception("Cannot parse timestamp");
        
        TimeSpan startSpan = TimeSpan.FromMilliseconds(startTimestamp);
        TimeSpan endSpan = TimeSpan.FromMilliseconds(endTimestamp);

        return (startSpan, endSpan);
    }

    public string FormatToString(AList<RichLyrics> content)
    {
        throw new NotSupportedException();
    }
}