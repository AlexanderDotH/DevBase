using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.LrcFormat
{
    public class LrcParser<T> : IFileFormat<AList<TimeStampedLyric>>
    {
        public AList<TimeStampedLyric> FormatFromFile(string filePath)
        {
            AFileObject file = AFile.ReadFile(filePath);
            return FormatFromString(file.ToStringData());
        }

        public AList<TimeStampedLyric> FormatFromString(string lyricString)
        {
            AList<TimeStampedLyric> lyricElements = new AList<TimeStampedLyric>();

            AList<string> linesAList = new AString(lyricString).AsList();

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

        public string FormatToString(AList<TimeStampedLyric> content)
        {
            throw new NotSupportedException();
        }
        
        private TimeStampedLyric? ParseStringToLyrics(string lyricLine)
        {
            if (lyricLine == null)
                return null;

            Match match = Regex.Match(lyricLine, RegexHolder.REGEX_TIMESTAMP);

            bool hasHours = false;

            if (match.Length == 0)
            {
                match = Regex.Match(lyricLine, RegexHolder.REGEX_DETAILED_TIMESTAMP);

                if (match.Success)
                {
                    hasHours = true;
                }
            }

            if (!match.Success)
                return null;

            string hour = hasHours ? match.Groups[2].Value : "00";
            string minutes = hasHours ? match.Groups[3].Value : match.Groups[2].Value;
            string seconds = hasHours ? match.Groups[4].Value : match.Groups[3].Value;
            string milliseconds = hasHours ? match.Groups[5].Value : match.Groups[4].Value;

            TimeSpan timeSpan = TimeSpan.Parse(hour + ":" + minutes + ":" + seconds + "." + milliseconds);

            string line = lyricLine.Replace(match.Groups[0].Value, String.Empty);

            if (IsLyricLineTrash(line))
                return null;
            
            string proceededLine = LyricsUtils.EditLine(line);
            
            TimeStampedLyric lyricElement = new TimeStampedLyric()
            {
                Text = proceededLine,
                StartTime = timeSpan
            };
            
            return lyricElement;
        }

        private bool IsLyricLineTrash(string line)
        {
            return Regex.IsMatch(line, RegexHolder.REGEX_GARBAGE);
        }
    }
}
