using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.ElrcFormat;

public class ElrcParser : IFileFormat<AList<RichTimeStampedLyric>>
{
    private readonly string _indent;

    public ElrcParser()
    {
        this._indent = "    ";
    }
    
    public AList<RichTimeStampedLyric> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }

    public string FormatToString(AList<RichTimeStampedLyric> richSync)
    {
        StringBuilder sb = new StringBuilder();
        
        for (var i = 0; i < richSync.Length; i++)
        {
            RichTimeStampedLyric line = richSync.Get(i);
        
            string startTime = line.StartTime.ToString();
            string endTime = line.EndTime.ToString();
            string t = line.Text;
        
            sb.AppendLine($"[{startTime}] - [{endTime}] {t}");
            
            if (line.Words.Length == 0)
                continue;
        
            sb.AppendLine("[");
            
            for (var j = 0; j < line.Words.Length; j++)
            {
                RichTimeStampedWord word = line.Words.Get(j);
        
                string cLine = word.Word;
                
                if (cLine == null || cLine.Length == 0)
                    continue;
        
                sb.AppendLine($"{this._indent}[{word.StartTime}] - [{word.EndTime}] {cLine}");
            }
            
            sb.AppendLine("]");
        }

        return sb.ToString();
    }
    
    public AList<RichTimeStampedLyric> FormatFromString(string lyricString)
    {
        AString input = new AString(lyricString);
        AList<string> lines = input.AsList();

        AList<AList<string>> blocks = FindBlocks(lines);

        AList<RichTimeStampedLyric> proceeded = new AList<RichTimeStampedLyric>();

        for (int i = 0; i < blocks.Length; i++)
            proceeded.Add(ProcessBlock(blocks.Get(i)));

        return proceeded;
    }
    
    private RichTimeStampedLyric ProcessBlock(AList<string> block)
    {
        string first = block.Get(0);

        if (!first.StartsWith("["))
            throw new System.Exception("Invalid block");

        AList<string> entries = block.GetRangeAsAList(2, block.Length - 2);

        Regex regex = new Regex(RegexHolder.REGEX_ELRC_DATA, RegexOptions.Multiline);

        if (!regex.IsMatch(first))
            throw new System.Exception("Invalid head block");

        RichTimeStampedLyric head = ParseHead(regex, first);

        for (int i = 0; i < entries.Length; i++)
        {
            string bodyElement = entries.Get(i).Replace(this._indent, string.Empty);
            head.Words.Add(ParseElement(regex, bodyElement));
        }
        
        return head;
    }
    
    private AList<AList<string>> FindBlocks(AList<string> lines)
    {
        AList<AList<string>> blocks = new AList<AList<string>>();
        
        for (int i = 0; i < lines.Length; i++)
        {
            string current = lines.Get(i);
            string next = string.Empty;

            if (i + 1 < lines.Length)
                next = lines.Get(i + 1);

            if (current[0] == '[' && 
                next.Length != 0 && next[0] == '[')
            {
                for (int j = i + 1; j < lines.Length; j++)
                {
                    string cSeek = lines.Get(j);

                    if (cSeek[0] == ']')
                    {
                        blocks.AddRange(lines.GetRangeAsAList(i, j));
                        break;
                    }
                }
            }
        }

        return blocks;
    }
    
    private string ParseLine(Match regexMatch) => regexMatch.Groups[16].Value;
    
    private TimeSpan ParseTimeSpan(Match regexMatch, bool firstSpan)
    {
        StringBuilder timeStamp = new StringBuilder();
        timeStamp.Append(firstSpan ? regexMatch.Groups[2].Value : regexMatch.Groups[10].Value);
        timeStamp.Append(":");
        timeStamp.Append(firstSpan ? regexMatch.Groups[4].Value : regexMatch.Groups[12].Value);
        timeStamp.Append(":");
        timeStamp.Append(firstSpan ? regexMatch.Groups[6].Value : regexMatch.Groups[14].Value);
        
        return TimeSpan.Parse(timeStamp.ToString());
    }

    private RichTimeStampedLyric ParseHead(Regex regex, string head)
    {
        Match firstMatch = regex.Match(head);
        
        TimeSpan hStart = ParseTimeSpan(firstMatch, true);
        TimeSpan hEnd = ParseTimeSpan(firstMatch, false);
        string hLine = ParseLine(firstMatch);

        return new RichTimeStampedLyric()
        {
            StartTime = hStart,
            EndTime = hEnd,
            Text = hLine,
            Words = new AList<RichTimeStampedWord>()
        };
    }
    
    private RichTimeStampedWord ParseElement(Regex regex, string bodyElement)
    {
        Match match = regex.Match(bodyElement);
        
        TimeSpan hStart = ParseTimeSpan(match, true);
        TimeSpan hEnd = ParseTimeSpan(match, false);
        string hWord = ParseLine(match);

        return new RichTimeStampedWord()
        {
            StartTime = hStart,
            EndTime = hEnd,
            Word = hWord
        };
    }
}