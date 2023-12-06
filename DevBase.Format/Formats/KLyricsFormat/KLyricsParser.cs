using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.KLyricsFormat;

public class KLyricsParser : IFileFormat<AList<RichLyrics>>
{
    private readonly Regex _regexTimeStamp;
    private readonly Regex _regexElements;
    private readonly Regex _regexLastWord;

    public KLyricsParser()
    {
        this._regexTimeStamp = new Regex(RegexHolder.REGEX_KLYRICS_TIMESTAMPS);
        this._regexElements = new Regex(RegexHolder.REGEX_KLRICS_DATA);
        this._regexLastWord = new Regex(RegexHolder.REGEX_KLYRICS_END);
    }
    
    public AList<RichLyrics> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }

    public AList<RichLyrics> FormatFromString(string lyricString)
    {
        List<(MatchCollection, MatchCollection, MatchCollection)> lines = GetLines(lyricString);
        
    }

    private RichLyrics ProcessLine(
        MatchCollection timestamps, 
        MatchCollection wordElements,
        MatchCollection lastWordElements)
    {
        int lineStartTimeStamp = Convert.ToInt32(timestamps[0].Groups[2].Value);
        int lineEndTimeStamp = Convert.ToInt32(timestamps[0].Groups[4].Value);

    }

    private List<RichLyricsElement> ProcessElements(int lineStartTimeStamp, MatchCollection wordElements)
    {
        List<RichLyricsElement> elements = new List<RichLyricsElement>();

        for (int i = 0; i < wordElements.Count; i++)
        {
            Match match = wordElements[i];
            
            int elementStartTimeStamp = Convert.ToInt32(match.Groups[5].Value);
            
            if (wordElements.Count )
        }
    }

    private List<(MatchCollection, MatchCollection, MatchCollection)> GetLines(string lyricString)
    {
        AList<string> lines = new AString(lyricString).AsList();

        List<(MatchCollection, MatchCollection, MatchCollection)> matchLines =
            new List<(MatchCollection, MatchCollection, MatchCollection)>();
        
        for (int i = 0; i < lines.Length; i++)
            matchLines.Add(GetLine(lines.Get(i)));

        return matchLines;
    }

    public (MatchCollection TimeStamps, MatchCollection WordElements, MatchCollection LastWordElement) GetLine(
        string current)
    {
        MatchCollection timestamps = null;
        MatchCollection wordElements = null;
        MatchCollection lastWordElement = null;
        
        if (this._regexTimeStamp.IsMatch(current))
            timestamps = this._regexTimeStamp.Matches(current);

        if (this._regexElements.IsMatch(current))
            wordElements = this._regexElements.Matches(current);

        if (this._regexLastWord.IsMatch(current))
            lastWordElement = this._regexLastWord.Matches(current);

        return (timestamps, wordElements, lastWordElement);
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