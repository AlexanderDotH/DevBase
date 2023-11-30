using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.ElrcFormat;

public class ElrcParser : IFileFormat<AList<RichLyrics>>
{
    private readonly string _indent;

    public ElrcParser()
    {
        this._indent = "    ";
    }
    
    public AList<RichLyrics> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }

    public string FormatToString(AList<RichLyrics> richSync)
    {
        StringBuilder sb = new StringBuilder();
        
        for (var i = 0; i < richSync.Length; i++)
        {
            RichLyrics line = richSync.Get(i);
        
            TimeSpan rStartTime = TimeSpan.FromMilliseconds(Convert.ToDouble(line.Start));
            TimeSpan rEndTime = TimeSpan.FromMilliseconds(Convert.ToDouble(line.End));
            
            string startTime = rStartTime.ToString();
            string endTime = rEndTime.ToString();
            string t = line.FullLine;
        
            sb.AppendLine($"[{startTime}] - [{endTime}] {t}");
            
            if (line.Elements.Count == 0)
                continue;
        
            sb.AppendLine("[");
            
            for (var j = 0; j < line.Elements.Count; j++)
            {
                RichLyricsElement character = line.Elements[j];
        
                string cLine = character.Line;
                
                if (cLine == null || cLine.Length == 0)
                    continue;
        
                TimeSpan cStartTime = TimeSpan.FromMilliseconds(Convert.ToDouble(character.Start));
                TimeSpan cEndTime = TimeSpan.FromMilliseconds(Convert.ToDouble(character.End));
                
                sb.AppendLine($"{this._indent}[{cStartTime}] - [{cEndTime}] {cLine}");
            }
            
            sb.AppendLine("]");
        }

        return sb.ToString();
    }
    
    public AList<RichLyrics> FormatFromString(string lyricString)
    {
        AString input = new AString(lyricString);
        AList<string> lines = input.AsList();

        AList<AList<string>> blocks = FindBlocks(lines);

        AList<RichLyrics> proceeded = new AList<RichLyrics>();

        for (int i = 0; i < blocks.Length; i++)
            proceeded.Add(ProcessBlock(blocks.Get(i)));

        return proceeded;
    }
    
    private RichLyrics ProcessBlock(AList<string> block)
    {
        string first = block.Get(0);

        if (!first.StartsWith("["))
            throw new System.Exception("Invalid block");

        AList<string> entries = block.GetRangeAsAList(2, block.Length - 2);

        Regex regex = new Regex(RegexHolder.REGEX_ELRC_DATA, RegexOptions.Multiline);

        if (!regex.IsMatch(first))
            throw new System.Exception("Invalid head block");

        RichLyrics head = ParseHead(regex, first);

        for (int i = 0; i < entries.Length; i++)
        {
            string bodyElement = entries.Get(i).Replace(this._indent, string.Empty);
            head.Elements.Add(ParseElement(regex, bodyElement));
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
    
    private long ParseTimeSpan(Match regexMatch, bool firstSpan)
    {
        StringBuilder timeStamp = new StringBuilder();
        timeStamp.Append(firstSpan ? regexMatch.Groups[2].Value : regexMatch.Groups[10].Value);
        timeStamp.Append(":");
        timeStamp.Append(firstSpan ? regexMatch.Groups[4].Value : regexMatch.Groups[12].Value);
        timeStamp.Append(":");
        timeStamp.Append(firstSpan ? regexMatch.Groups[6].Value : regexMatch.Groups[14].Value);
        
        TimeSpan timeSpan = TimeSpan.Parse(timeStamp.ToString());
        return Convert.ToInt64(timeSpan.TotalMilliseconds);
    }

    private RichLyrics ParseHead(Regex regex, string head)
    {
        Match firstMatch = regex.Match(head);
        
        long hStart = ParseTimeSpan(firstMatch, true);
        long hEnd = ParseTimeSpan(firstMatch, false);
        string hLine = ParseLine(firstMatch);

        return new RichLyrics()
        {
            Start = hStart,
            End = hEnd,
            FullLine = hLine,
            Elements = new List<RichLyricsElement>()
        };
    }
    
    private RichLyricsElement ParseElement(Regex regex, string bodyElement)
    {
        Match match = regex.Match(bodyElement);
        
        long hStart = ParseTimeSpan(match, true);
        long hEnd = ParseTimeSpan(match, false);
        string hLine = ParseLine(match);

        return new RichLyricsElement()
        {
            Start = hStart,
            End = hEnd,
            Line = hLine
        };
    }
}