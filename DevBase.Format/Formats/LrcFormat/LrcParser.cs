using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.LrcFormat
{
    public class LrcParser<T> : IFileFormat<LrcObject>
    {
        public LrcObject FormatFromFile(string filePath)
        {
            AFileObject file = AFile.ReadFile(filePath);
            return FormatFromString(file.ToStringData());
        }

        public LrcObject FormatFromString(string lyricString)
        {
            AList<LyricElement> lyricElements = new AList<LyricElement>();

            AList<string> linesAList = new AString(lyricString).AsList();

            LrcObject fullLrcObject = null;

            if (linesAList.Length > 7)
            {
                fullLrcObject = ParseMetaData(linesAList.GetRangeAsList(0, 7));
            }

            for (int i = 0; i < linesAList.Length; i++)
            {
                string lineInList = linesAList.Get(i);

                LyricElement lyricElement = ParseStringToLyrics(lineInList, i, linesAList.Length);
                if (lyricElement != null)
                {
                    lyricElements.Add(lyricElement);
                }
            }

            if (fullLrcObject == null)
            {
                fullLrcObject = new LrcObject();
            }

            fullLrcObject.Lyrics = lyricElements;

            return fullLrcObject;
        }

        private string ParseMetaDataPart(List<string> input, string meta)
        {
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == null)
                    continue;
                
                string line = input[i];
                
                if (line.Contains(string.Format("[{0}:", meta)))
                {
                    string regex = string.Format(RegexHolder.REGEX_METADATA, meta);

                    if (Regex.IsMatch(line, regex))
                    {
                        return Regex.Match(line, regex).Groups[2].Value;
                    }
                }
            }

            return string.Empty;
        }

        private LrcObject ParseMetaData(List<string> rawMetaData)
        {
            LrcObject lrcObject = new LrcObject();

            lrcObject.Artist = ParseMetaDataPart(rawMetaData, "ar");
            lrcObject.Album = ParseMetaDataPart(rawMetaData, "al");
            lrcObject.Title = ParseMetaDataPart(rawMetaData, "ti");
            lrcObject.Author = ParseMetaDataPart(rawMetaData, "au");
            lrcObject.By = ParseMetaDataPart(rawMetaData, "by");
            lrcObject.Offset = ParseMetaDataPart(rawMetaData, "offset").Equals(string.Empty) ? 0 : Convert.ToInt32(ParseMetaDataPart(rawMetaData, "offset"));
            lrcObject.Re = ParseMetaDataPart(rawMetaData, "re");
            lrcObject.Version = ParseMetaDataPart(rawMetaData, "ve");

            return lrcObject;
        }

        private LyricElement? ParseStringToLyrics(string lyricLine, int lineIndex, int maxLines)
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
            
            string lyricElementLine = LyricsUtils.EditLine(line);
            LyricElement lyricElement = new LyricElement(Convert.ToInt64(timeSpan.TotalMilliseconds), lyricElementLine);
            return lyricElement;
        }

        private bool IsLyricLineTrash(string line)
        {
            return Regex.IsMatch(line, RegexHolder.REGEX_GARBAGE);
        }
    }
}
