﻿using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;
using DevBase.Utilities;

namespace DevBase.Format.Formats.KLyricsFormat;

public class KLyricsParser : FileFormat<string, AList<RichTimeStampedLyric>>
{
    public override AList<RichTimeStampedLyric> Parse(string from)
    {
        AList<RichTimeStampedLyric> richTimeStampedLyrics = new AList<RichTimeStampedLyric>();
        
        AList<string> lines = new AString(from).AsList();

        int firstLine = FindFirstLine(lines);

        AList<string> parsableLines = lines.GetRangeAsAList(firstLine, lines.Length - 1);
        
        for (int i = 0; i < parsableLines.Length; i++)
            richTimeStampedLyrics.Add(ParseSingleLine(parsableLines.Get(i)));
        
        return richTimeStampedLyrics;
    }

    public override bool TryParse(string from, out AList<RichTimeStampedLyric> parsed)
    {
        AList<RichTimeStampedLyric> p = Parse(from);
        
        if (p == null || p.IsEmpty())
        {
            parsed = null;
            return Error<bool>("The parsed lyrics are null or empty");
        }

        parsed = p;
        return true;
    }

    private int FindFirstLine(AList<string> lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (RegexHolder.RegexKlyricsWord.IsMatch(lines.Get(i)))
                return i;
        }

        return Error<object>("Block not found");
    }
    
    private RichTimeStampedLyric ParseSingleLine(string line)
    {
        if (!RegexHolder.RegexKlyricsTimeStamps.IsMatch(line))
            return Error<object>("Timestamp is missing");

        if (!RegexHolder.RegexKlyricsWord.IsMatch(line))
            return Error<object>("Words are missing");
        
        AList<RichTimeStampedWord> words = new AList<RichTimeStampedWord>();
        
        Match timeStamp = RegexHolder.RegexKlyricsTimeStamps.Match(line);
        MatchCollection wordMatches = RegexHolder.RegexKlyricsWord.Matches(line);

        TimeSpan lineStartTime = GetTimeSpan(timeStamp, 2);
        TimeSpan lineEndTime = lineStartTime + GetTimeSpan(timeStamp, 4);

        TimeSpan lineAddDuration = TimeSpan.Zero;
        
        for (int i = 0; i < wordMatches.Count; i++)
        {
            Match currentWordMatch = wordMatches[i];

            TimeSpan lineDuration = GetTimeSpan(currentWordMatch, 4);

            RichTimeStampedWord word = new RichTimeStampedWord()
            {
                StartTime = lineAddDuration + lineStartTime,
                EndTime = lineAddDuration + lineDuration + lineStartTime,
                Word = GetWord(currentWordMatch, 6)
            };
            
            words.Add(word);

            lineAddDuration += lineDuration;
        }

        return new RichTimeStampedLyric()
        {
            StartTime = lineStartTime,
            EndTime = lineEndTime,
            Text = GetFullSentence(words),
            Words = words
        };
    }

    private string GetFullSentence(AList<RichTimeStampedWord> words)
    {
        AList<string> wordList = new AList<string>();

        for (int i = 0; i < words.Length; i++)
        {
            RichTimeStampedWord word = words.Get(i);
            
            if (String.IsNullOrEmpty(word.Word.Trim()))
                continue;
            
            wordList.Add(word.Word);
        }
        
        return StringUtils.Separate(wordList, " ");
    }
    
    private string GetWord(Match match, int group)
    {
        return match.Groups[group].Value;
    }
    
    private TimeSpan GetTimeSpan(Match match, int group)
    {
        string rawData = match.Groups[group].Value;
        double data = Convert.ToDouble(rawData);
        return TimeSpan.FromMilliseconds(data);
    }
}