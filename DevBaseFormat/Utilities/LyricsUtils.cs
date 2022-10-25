using System.Text.RegularExpressions;
using DevBaseFormat.Structure;

namespace DevBaseFormat.Utilities
{
    class LyricsUtils
    {
        public static string EditLine(string line)
        {
            if (line == null)
            {
                return "♪";
            }

            if (line.Length > 0)
            {
                if (line.Equals("") || line.Equals(" "))
                {
                    line = line.Replace(line, "♪");
                }

                if (Regex.IsMatch(line, RegexHolder.REGEX_TIMESTAMP) ||
                    Regex.IsMatch(line, RegexHolder.REGEX_DETAILED_TIMESTAMP))
                {
                    line = Regex.Replace(line, RegexHolder.REGEX_TIMESTAMP, string.Empty);
                    line = Regex.Replace(line, RegexHolder.REGEX_DETAILED_TIMESTAMP, string.Empty);
                }

                if (line.Contains("\\n"))
                    line = line.Replace("\\n", string.Empty);

                line = line.Trim();

            }
            else
            {
                line = "♪";
            }

            return line;
        }
    }
}
