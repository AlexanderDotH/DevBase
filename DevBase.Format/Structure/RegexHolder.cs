using System.Text.RegularExpressions;

namespace DevBase.Format.Structure
{
    /// <summary>
    /// Holds compiled Regular Expressions for various lyric formats.
    /// </summary>
    public class RegexHolder
    {
        /// <summary>Regex pattern for standard LRC format.</summary>
        public const string REGEX_LRC = "((\\[)([0-9]*)([:])([0-9]*)([:]|[.])(\\d+\\.\\d+|\\d+)(\\]))((\\s|.).*$)";
        /// <summary>Regex pattern for garbage/metadata lines.</summary>
        public const string REGEX_GARBAGE = "\\D(\\?{0,2}).([:]).([\\w /]*)";
        /// <summary>Regex pattern for environment variables/metadata.</summary>
        public const string REGEX_ENV = "(\\w*)\\=\"(\\w*)";
        /// <summary>Regex pattern for SRT timestamps.</summary>
        public const string REGEX_SRT_TIMESTAMPS = "([0-9:,]*)(\\W(-->)\\W)([0-9:,]*)";
        /// <summary>Regex pattern for Enhanced LRC (ELRC) format data.</summary>
        public const string REGEX_ELRC_DATA = "(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])(\\s-\\s)(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])\\s(.*$)";
        /// <summary>Regex pattern for KLyrics word format.</summary>
        public const string REGEX_KLYRICS_WORD = "(\\()([0-9]*)(\\,)([0-9]*)(\\))([^\\(\\)\\[\\]\\n]*)";
        /// <summary>Regex pattern for KLyrics timestamp format.</summary>
        public const string REGEX_KLYRICS_TIMESTAMPS = "(\\[)([0-9]*)(\\,)([0-9]*)(\\])";

        /// <summary>Compiled Regex for standard LRC format.</summary>
        public static Regex RegexLrc = new Regex(REGEX_LRC, RegexOptions.Compiled);
        /// <summary>Compiled Regex for garbage/metadata lines.</summary>
        public static Regex RegexGarbage = new Regex(REGEX_GARBAGE, RegexOptions.Compiled);
        /// <summary>Compiled Regex for environment variables/metadata.</summary>
        public static Regex RegexEnv = new Regex(REGEX_ENV, RegexOptions.Compiled);
        /// <summary>Compiled Regex for SRT timestamps.</summary>
        public static Regex RegexSrtTimeStamps = new Regex(REGEX_SRT_TIMESTAMPS, RegexOptions.Compiled);
        /// <summary>Compiled Regex for Enhanced LRC (ELRC) format data.</summary>
        public static Regex RegexElrc = new Regex(REGEX_ELRC_DATA, RegexOptions.Compiled);
        /// <summary>Compiled Regex for KLyrics word format.</summary>
        public static Regex RegexKlyricsWord = new Regex(REGEX_KLYRICS_WORD, RegexOptions.Compiled);
        /// <summary>Compiled Regex for KLyrics timestamp format.</summary>
        public static Regex RegexKlyricsTimeStamps = new Regex(REGEX_KLYRICS_TIMESTAMPS, RegexOptions.Compiled);

        // public const string REGEX_KLYRICS_TIMESTAMPS = "(\\[)([0-9]*)(\\,)([0-9]*)(\\])";
        // public const string REGEX_KLRICS_DATA = "((\\()([0-9])(\\,)([0-9]*)(\\))([\\D]*)(\\()([0-9]*)(\\,)([0-9]*)(\\))(\\s))";
        // public const string REGEX_KLYRICS_END = "(((\\()([0-9])(\\,)([0-9]*)(\\))([\\D]*$)))";
    }
}
