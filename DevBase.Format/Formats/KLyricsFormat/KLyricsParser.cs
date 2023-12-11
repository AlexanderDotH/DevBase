using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.KLyricsFormat;

public class KLyricsParser : IFileFormat<AList<RichTimeStampedLyric>>
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
    
    public AList<RichTimeStampedLyric> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }

    public AList<RichTimeStampedLyric> FormatFromString(string lyricString)
    {
        AList<RichTimeStampedLyric> richLyrics = new AList<RichTimeStampedLyric>();
        List<(MatchCollection, MatchCollection, MatchCollection)> lines = GetLines(lyricString);

        for (var i = 0; i < lines.Count; i++)
        {
            (MatchCollection Timestamps, MatchCollection WordElements, MatchCollection LastWordElements)
                line = lines[i];
            richLyrics.Add(this.ProcessLine(line.Timestamps, line.WordElements, line.LastWordElements));
        }

        return richLyrics;
    }
        
    public string FormatToString(AList<RichTimeStampedLyric> content)
    {
        throw new NotSupportedException();
    }

    private RichTimeStampedLyric ProcessLine(
        MatchCollection timestamps, 
        MatchCollection wordElements,
        MatchCollection lastWordElements)
    {
        double lineStartTimeStamp = Convert.ToDouble(timestamps[0].Groups[2].Value);
        double lineEndTimeStamp = lineStartTimeStamp + Convert.ToDouble(timestamps[0].Groups[4].Value);

        TimeSpan sStartTime = TimeSpan.FromMilliseconds(lineStartTimeStamp);
        TimeSpan sEndTime = TimeSpan.FromMilliseconds(lineStartTimeStamp);
        
        (string FullWord, AList<RichTimeStampedWord> Elements) proceeded = ProcessElements(lineStartTimeStamp, wordElements, lastWordElements);

        RichTimeStampedLyric richTimeStampedLyric = new RichTimeStampedLyric()
        {
            StartTime = sStartTime,
            EndTime = sEndTime,
            Text = proceeded.FullWord,
            Words = proceeded.Elements
        };

        return richTimeStampedLyric;
    }

    private (string, AList<RichTimeStampedWord>) ProcessElements(
        double lineStartTimeStamp, 
        MatchCollection wordElements, 
        MatchCollection lastWordElement)
    {
        AList<RichTimeStampedWord> elements = new AList<RichTimeStampedWord>();
        StringBuilder fullWord = new StringBuilder();

        double lastStartTimeStamp = lineStartTimeStamp;
        
        for (int i = 0; i < wordElements.Count; i++)
        {
            Match match = wordElements[i];
            
            double elementStartTimeStamp = Convert.ToDouble(match.Groups[5].Value);
            double timeStampStartTime = lineStartTimeStamp + elementStartTimeStamp;
            string word = GetWord(match, false);

            TimeSpan sStartTime = TimeSpan.FromMilliseconds(elementStartTimeStamp);
            TimeSpan sEndTime = TimeSpan.FromMilliseconds(timeStampStartTime);
            
            RichTimeStampedWord richWord = new RichTimeStampedWord()
            {
                StartTime = sStartTime,
                EndTime = sEndTime,
                Word = word
            };
            
            elements.Add(richWord);
            fullWord.Append($"{word} ");

            lastStartTimeStamp = timeStampStartTime + Convert.ToInt32(match.Groups[11].Value);
        }

        for (int i = 0; i < lastWordElement.Count; i++)
        {
            Match match = lastWordElement[i];

            double timeStampStartTime = lastStartTimeStamp + Convert.ToInt32(match.Groups[6].Value); 
            string word = GetWord(match, true);

            TimeSpan sStartTime = TimeSpan.FromMilliseconds(lastStartTimeStamp);
            TimeSpan sEndTime = TimeSpan.FromMilliseconds(timeStampStartTime);
            
            RichTimeStampedWord richWord = new RichTimeStampedWord()
            {
                StartTime = sStartTime,
                EndTime = sEndTime,
                Word = word
            };
            
            fullWord.Append($"{word}");
            
            elements.Add(richWord);
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