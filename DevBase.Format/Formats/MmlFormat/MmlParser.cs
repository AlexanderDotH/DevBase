using System.Text.RegularExpressions;
using DevBase.Format.Formats.MmlFormat.Json;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;
using DevBase.IO;
using Newtonsoft.Json;

namespace DevBase.Format.Formats.MmlFormat
{
    public class MmlParser : IFileFormat<AList<TimeStampedLyric>>
    {
        public AList<TimeStampedLyric> FormatFromFile(string filePath)
        {
            AFileObject file = AFile.ReadFile(filePath);
            return FormatFromString(file.ToStringData());
        }

        public AList<TimeStampedLyric> FormatFromString(string lyricString)
        {
            AList<TimeStampedLyric> timeStampedLyrics = new AList<TimeStampedLyric>();

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

                TimeSpan sTimeSpan = TimeSpan.FromMilliseconds(Convert.ToDouble(timeStamp));
                
                if (IsLyricLineTrash(mmlElement.Text))
                {
                    TimeStampedLyric timeStampedLyric = new TimeStampedLyric()
                    {
                        StartTime = sTimeSpan,
                        Text = mmlElement.Text
                    };
                    
                    timeStampedLyrics.Add(timeStampedLyric);
                }
            }

            return timeStampedLyrics;
        }

        public string FormatToString(AList<TimeStampedLyric> timeStampedLyrics)
        {
            throw new NotSupportedException("Not supported yet, it will be implemented if necessary");

        }

        private bool IsLyricLineTrash(string line)
        {
            return Regex.IsMatch(line, RegexHolder.REGEX_GARBAGE);
        }
    }
}
