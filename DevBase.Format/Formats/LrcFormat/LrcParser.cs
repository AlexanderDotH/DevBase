﻿using System.Text;
using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.LrcFormat
{
    public class LrcParser : RevertableFileFormat<string, AList<TimeStampedLyric>>
    {
        private readonly Regex _regexLrc;
        private readonly Regex _regexGarbage;

        public LrcParser()
        {
            this._regexLrc = new Regex(RegexHolder.REGEX_LRC, RegexOptions.Multiline);
            this._regexGarbage = new Regex(RegexHolder.REGEX_GARBAGE);
        }
        
        public override AList<TimeStampedLyric> Parse(string from)
        {
            AList<TimeStampedLyric> lyricElements = new AList<TimeStampedLyric>();

            AList<string> linesAList = new AString(from).AsList();

            for (int i = 0; i < linesAList.Length; i++)
            {
                string lineInList = linesAList.Get(i);

                TimeStampedLyric? lyricElement = ParseStringToLyrics(lineInList);
                
                if (lyricElement != null)
                {
                    lyricElements.Add(lyricElement);
                }
            }

            return lyricElements;
        }
        
        public override string Revert(AList<TimeStampedLyric> to)
        {
            StringBuilder lrcContent = new StringBuilder();

            for (int i = 0; i < to.Length; i++)
            {
                TimeStampedLyric stampedLyric = to.Get(i);
                lrcContent.AppendLine($"[{stampedLyric.StartTime.ToString()}] {stampedLyric.Text}");
            }

            return lrcContent.ToString();
        }
        
        private TimeStampedLyric? ParseStringToLyrics(string lyricLine)
        {
            if (lyricLine == null)
                return null;

            if (!this._regexLrc.IsMatch(lyricLine))
                return HandleException("LRC regex does not match");

            Match match = this._regexLrc.Match(lyricLine);

            string rawLine = match.Groups[9].Value;
            
            if (this._regexGarbage.IsMatch(rawLine))
                return null;

            TimeSpan startTime;

            string groupTime = match.Groups[1].Value;
            string rawTime = groupTime.Substring(1, groupTime.Length - 2);
            
            if (!TimeUtils.TryParseTimeStamp(rawTime, out startTime))
                return HandleException("Cannot parse timestamp");
            
            string text = LyricsUtils.EditLine(rawLine);
            
            TimeStampedLyric timeStampedLyric = new TimeStampedLyric()
            {
                StartTime = startTime,
                Text = text
            };

            return timeStampedLyric;
        }
    }
}
