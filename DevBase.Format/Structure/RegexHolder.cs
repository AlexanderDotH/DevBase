using System.Text.RegularExpressions;

namespace DevBase.Format.Structure
{
    public class RegexHolder
    {
        public const string REGEX_LRC = "((\\[)([0-9]*)([:])([0-9]*)([:]|[.])(\\d+\\.\\d+|\\d+)(\\]))((\\s|.).*$)";
        public const string REGEX_GARBAGE = "\\D(\\?{0,2}).([:]).([\\w /]*)";
        public const string REGEX_ENV = "(\\w*)\\=\"(\\w*)";
        public const string REGEX_SRT_TIMESTAMPS = "([0-9:,]*)(\\W(-->)\\W)([0-9:,]*)";
        public const string REGEX_ELRC_DATA = "(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])(\\s-\\s)(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])\\s(.*$)";
        public const string REGEX_KLYRICS_WORD = "(\\()([0-9]*)(\\,)([0-9]*)(\\))([^\\(\\)\\[\\]\\n]*)";
        public const string REGEX_KLYRICS_TIMESTAMPS = "(\\[)([0-9]*)(\\,)([0-9]*)(\\])";

        public static Regex RegexLrc = new Regex(REGEX_LRC, RegexOptions.Compiled);
        public static Regex RegexGarbage = new Regex(REGEX_GARBAGE, RegexOptions.Compiled);
        public static Regex RegexEnv = new Regex(REGEX_ENV, RegexOptions.Compiled);
        public static Regex RegexSrtTimeStamps = new Regex(REGEX_SRT_TIMESTAMPS, RegexOptions.Compiled);
        public static Regex RegexElrc = new Regex(REGEX_ELRC_DATA, RegexOptions.Compiled);
        public static Regex RegexKlyricsWord = new Regex(REGEX_KLYRICS_WORD, RegexOptions.Compiled);
        public static Regex RegexKlyricsTimeStamps = new Regex(REGEX_KLYRICS_TIMESTAMPS, RegexOptions.Compiled);

        // public const string REGEX_KLYRICS_TIMESTAMPS = "(\\[)([0-9]*)(\\,)([0-9]*)(\\])";
        // public const string REGEX_KLRICS_DATA = "((\\()([0-9])(\\,)([0-9]*)(\\))([\\D]*)(\\()([0-9]*)(\\,)([0-9]*)(\\))(\\s))";
        // public const string REGEX_KLYRICS_END = "(((\\()([0-9])(\\,)([0-9]*)(\\))([\\D]*$)))";
    }
}
