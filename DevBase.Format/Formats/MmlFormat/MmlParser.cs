using System.Text.RegularExpressions;
using DevBase.Format.Formats.MmlFormat.Json;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;
using DevBase.IO;
using Newtonsoft.Json;

namespace DevBase.Format.Formats.MmlFormat
{
    /// <summary>
    /// Parser for the MML (Musixmatch Lyric) JSON format.
    /// </summary>
    public class MmlParser : FileFormat<string, AList<TimeStampedLyric>>
    {
        /// <summary>
        /// Parses the MML JSON string content into a list of time-stamped lyrics.
        /// </summary>
        /// <param name="from">The JSON string content.</param>
        /// <returns>A list of <see cref="TimeStampedLyric"/> objects.</returns>
        public override AList<TimeStampedLyric> Parse(string from)
        {
            AList<TimeStampedLyric> timeStampedLyrics = new AList<TimeStampedLyric>();

            MmlElement[] parsedElements = JsonConvert.DeserializeObject<MmlElement[]>(from);

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

        /// <summary>
        /// Attempts to parse the MML JSON string content.
        /// </summary>
        /// <param name="from">The JSON string content.</param>
        /// <param name="parsed">The parsed list of lyrics, or null if parsing fails.</param>
        /// <returns>True if parsing was successful; otherwise, false.</returns>
        public override bool TryParse(string from, out AList<TimeStampedLyric> parsed)
        {
            AList<TimeStampedLyric> p = Parse(from);
            
            if (p == null || p.IsEmpty())
            {
                parsed = null;
                return Error<bool>("The parsed lyrics are null or empty");
            }

            parsed = p;
            return true;
        }

        private bool IsLyricLineTrash(string line)
        {
            return Regex.IsMatch(line, RegexHolder.REGEX_GARBAGE);
        }
    }
}
