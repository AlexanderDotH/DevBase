using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
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
        AList<RichLyrics> richLyrics = new AList<RichLyrics>();
        List<(MatchCollection, MatchCollection, MatchCollection)> lines = GetLines(lyricString);

        for (var i = 0; i < lines.Count; i++)
        {
            (MatchCollection Timestamps, MatchCollection WordElements, MatchCollection LastWordElements)
                line = lines[i];
            richLyrics.Add(this.ProcessLine(line.Timestamps, line.WordElements, line.LastWordElements));
        }

        return richLyrics;
    }
        
    public string FormatToString(AList<RichLyrics> content)
    {
        throw new NotSupportedException();
    }

    private RichLyrics ProcessLine(
        MatchCollection timestamps, 
        MatchCollection wordElements,
        MatchCollection lastWordElements)
    {
        int lineStartTimeStamp = Convert.ToInt32(timestamps[0].Groups[2].Value);
        int lineEndTimeStamp = lineStartTimeStamp + Convert.ToInt32(timestamps[0].Groups[4].Value);

        (string FullWord, List<RichLyricsElement> Elements) proceeded = ProcessElements(lineStartTimeStamp, wordElements, lastWordElements);

        RichLyrics richLyrics = new RichLyrics()
        {
            Start = lineStartTimeStamp,
            End = lineEndTimeStamp,
            FullLine = proceeded.FullWord,
            Elements = proceeded.Elements
        };

        return richLyrics;
    }

    private (string, List<RichLyricsElement>) ProcessElements(int lineStartTimeStamp, MatchCollection wordElements, MatchCollection lastWordElement)
    {
        List<RichLyricsElement> elements = new List<RichLyricsElement>();
        StringBuilder fullWord = new StringBuilder();

        int lastStartTimeStamp = lineStartTimeStamp;
        
        for (int i = 0; i < wordElements.Count; i++)
        {
            Match match = wordElements[i];
            
            int elementStartTimeStamp = Convert.ToInt32(match.Groups[5].Value);
            int timeStampStartTime = lineStartTimeStamp + elementStartTimeStamp;
            string word = GetWord(match, false);

            RichLyricsElement richLyricsElement = new RichLyricsElement()
            {
                Start = lastStartTimeStamp,
                End = timeStampStartTime,
                Line = word
            };
            
            elements.Add(richLyricsElement);
            fullWord.Append($"{word} ");

            lastStartTimeStamp = timeStampStartTime + Convert.ToInt32(match.Groups[11].Value);
        }

        for (int i = 0; i < lastWordElement.Count; i++)
        {
            Match match = lastWordElement[i];

            int timeStampStartTime = lastStartTimeStamp + Convert.ToInt32(match.Groups[6].Value); 
            string word = GetWord(match, true);
            
            RichLyricsElement lastElement = new RichLyricsElement()
            {
                Start = lastStartTimeStamp,
                End = timeStampStartTime,
                Line = word
            };
            
            fullWord.Append($"{word}");
            
            elements.Add(lastElement);
        }

        return (fullWord.ToString(), elements);
    }

    private string GetWord(Match match, bool isLast)
    {
        string prefix = Combine(match, 2, 6);
        string suffix = Combine(match, 8, 13);

        if (isLast)
            prefix = Combine(match, 3, 7);
        
        string raw = match.Groups[1].Value;

        raw = raw.Replace(prefix, string.Empty);
        
        if (!isLast)
            raw = raw.Replace(suffix, string.Empty);

        return raw;
    }

    private string Combine(Match match, int min, int max)
    {
        StringBuilder combined = new StringBuilder();

        for (int i = min; i <= max; i++)
            combined.Append(match.Groups[i].Value);

        return combined.ToString();
    }
    
    private List<(MatchCollection, MatchCollection, MatchCollection)> GetLines(string lyricString)
    {
        AList<string> lines = new AString(lyricString).AsList();

        List<(MatchCollection, MatchCollection, MatchCollection)> matchLines =
            new List<(MatchCollection, MatchCollection, MatchCollection)>();

        for (int i = 0; i < lines.Length; i++)
        {
            (MatchCollection TimeStamps, MatchCollection WordElements, MatchCollection LastWordElement) line =
                GetLine(lines.Get(i));
            
            if (line.TimeStamps == null || line.WordElements == null || line.LastWordElement == null)
                continue;
            
            matchLines.Add(line);
        }

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
}