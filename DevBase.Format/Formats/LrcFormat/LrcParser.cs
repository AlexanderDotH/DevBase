using System.Text;
using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.LrcFormat
{
    /// <summary>
    /// Parser for the LRC (Lyric) file format.
    /// Supports parsing string content into a list of time-stamped lyrics and reverting them back to string.
    /// </summary>
    public class LrcParser : RevertableFileFormat<string, AList<TimeStampedLyric>>
    {
        /// <summary>
        /// Parses the LRC string content into a list of time-stamped lyrics.
        /// </summary>
        /// <param name="from">The LRC string content.</param>
        /// <returns>A list of <see cref="TimeStampedLyric"/> objects.</returns>
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

        /// <summary>
        /// Attempts to parse the LRC string content.
        /// </summary>
        /// <param name="from">The LRC string content.</param>
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

        /// <summary>
        /// Reverts a list of time-stamped lyrics back to LRC string format.
        /// </summary>
        /// <param name="to">The list of lyrics to revert.</param>
        /// <returns>The LRC string representation.</returns>
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

        /// <summary>
        /// Attempts to revert a list of lyrics to LRC string format.
        /// </summary>
        /// <param name="to">The list of lyrics to revert.</param>
        /// <param name="from">The LRC string representation, or null if reverting fails.</param>
        /// <returns>True if reverting was successful; otherwise, false.</returns>
        public override bool TryRevert(AList<TimeStampedLyric> to, out string from)
        {
            string r = Revert(to);

            if (string.IsNullOrEmpty(r))
            {
                from = null; 
                return Error<bool>("The parsed lyrics are null or empty");
            }

            from = r;
            return true;
        }

        private TimeStampedLyric? ParseStringToLyrics(string lyricLine)
        {
            if (lyricLine == null)
                return null;

            if (RegexHolder.RegexGarbage.IsMatch(lyricLine))
                return null;
            
            if (!RegexHolder.RegexLrc.IsMatch(lyricLine))
                return Error<object>("LRC regex does not match");
            
            Match match = RegexHolder.RegexLrc.Match(lyricLine);

            string rawLine = match.Groups[9].Value;

            TimeSpan startTime;

            string groupTime = match.Groups[1].Value;
            string rawTime = groupTime.Substring(1, groupTime.Length - 2);
            
            if (!TimeUtils.TryParseTimeStamp(rawTime, out startTime))
                return Error<object>("Cannot parse timestamp");
            
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
