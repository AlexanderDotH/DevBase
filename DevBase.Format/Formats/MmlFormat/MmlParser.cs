using System.Text.RegularExpressions;
using DevBase.Format.Formats.MmlFormat.Json;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;
using DevBase.IO;
using Newtonsoft.Json;

namespace DevBase.Format.Formats.MmlFormat
{
    public class MmlParser : IFileFormat<LrcObject>
    {
        public LrcObject FormatFromFile(string filePath)
        {
            AFileObject file = AFile.ReadFile(filePath);
            return FormatFromString(file.ToStringData());
        }

        public LrcObject FormatFromString(string lyricString)
        {
            AList<LyricElement> lyricElements = new AList<LyricElement>();

            MmlElement[] parsedElements = JsonConvert.DeserializeObject<MmlElement[]>(lyricString);

            if (parsedElements == null)
                return default;

            for (int i = 0; i < parsedElements.Length; i++)
            {
                MmlElement mmlElement = parsedElements[i];

                if (mmlElement == null)
                    continue;

                long timeStamp = 0;
                timeStamp += (long)TimeSpan.FromMinutes(mmlElement.Time.Minutes).TotalMilliseconds;
                timeStamp += (long)TimeSpan.FromSeconds(mmlElement.Time.Seconds).TotalMilliseconds;
                timeStamp += (long)TimeSpan.FromMilliseconds(mmlElement.Time.Hundredths).TotalMilliseconds;

                if (IsLyricLineTrash(mmlElement.Text))
                    lyricElements.Add(new LyricElement(
                        timeStamp, 
                        LyricsUtils.EditLine(mmlElement.Text)));
                    
            }

            LrcObject lrcObject = new LrcObject();
            lrcObject.Lyrics = lyricElements;
            return lrcObject;
        }
        
        private bool IsLyricLineTrash(string line)
        {
            return Regex.IsMatch(line, RegexHolder.REGEX_GARBAGE);
        }
    }
}
