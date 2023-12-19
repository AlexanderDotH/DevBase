﻿using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.ElrcFormat;

public class ElrcParser : RevertableFileFormat<string, AList<RichTimeStampedLyric>>
{
    private readonly string _indent;

    private readonly Regex _elrcDataRegex;
    
    public ElrcParser()
    {
        this._indent = "    ";
        this._elrcDataRegex = new Regex(RegexHolder.REGEX_ELRC_DATA, RegexOptions.Multiline);
    }
    
    public override AList<RichTimeStampedLyric> Parse(string from)
    {
        AString input = new AString(from);
        AList<string> lines = input.AsList();

        AList<AList<string>> blocks = FindBlocks(lines);

        AList<RichTimeStampedLyric> proceeded = new AList<RichTimeStampedLyric>();

        for (int i = 0; i < blocks.Length; i++)
            proceeded.Add(ProcessBlock(blocks.Get(i)));

        return proceeded;
    }

    public override string Revert(AList<RichTimeStampedLyric> to)
    {
        StringBuilder sb = new StringBuilder();
        
        for (var i = 0; i < to.Length; i++)
        {
            RichTimeStampedLyric line = to.Get(i);
        
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

        if (sb.Length == 0)
            return Error("Block not found");
        
        return sb.ToString();
    }
    
    private RichTimeStampedLyric ProcessBlock(AList<string> block)
    {
        string first = block.Get(0);

        if (!first.StartsWith("["))
            return Error("Invalid block at block position 0");

        AList<string> entries = block.GetRangeAsAList(2, block.Length - 2);

        if (!this._elrcDataRegex.IsMatch(first))
            return Error("Invalid head block");

        RichTimeStampedLyric head = ParseHead(first);

        for (int i = 0; i < entries.Length; i++)
        {
            // A bit unclean but I'm too lazy for now
            string bodyElement = entries.Get(i).Replace(this._indent, string.Empty);
            head.Words.Add(ParseElement(bodyElement));
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

        TimeSpan parsed;

        if (!TimeUtils.TryParseTimeStamp(timeStamp.ToString(), out parsed))
            return Error($"Cannot parse timespan {timeStamp.ToString()}");
        
        return parsed;
    }

    private RichTimeStampedLyric ParseHead(string head)
    {
        Match firstMatch = this._elrcDataRegex.Match(head);
        
        TimeSpan hStart = ParseTimeSpan(firstMatch, true);
        TimeSpan hEnd = ParseTimeSpan(firstMatch, false);
        string hLine = ParseLine(firstMatch);

        return new RichTimeStampedLyric()
        {
            StartTime = hStart,
            EndTime = hEnd,
            Text = LyricsUtils.EditLine(hLine),
            Words = new AList<RichTimeStampedWord>()
        };
    }
    
    private RichTimeStampedWord ParseElement(string bodyElement)
    {
        Match match = this._elrcDataRegex.Match(bodyElement);
        
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