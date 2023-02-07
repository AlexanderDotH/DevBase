namespace DevBase.Format.Structure
{
    public class RegexHolder
    {
        public const string REGEX_TIMESTAMP = "\\[(([0-9]*)\\:([0-9]*)\\.([0-9]*))\\]";
        public const string REGEX_DETAILED_TIMESTAMP = "\\[(([0-9]*)\\:([0-9]*)\\:([0-9]*)\\.([0-9]*))\\]";
        public const string REGEX_METADATA = "(\\[{0}:(.*)\\])";
        public const string REGEX_GARBAGE = "\\D(\\?{0,2}).([:]).([\\w /]*)";
        public const string REGEX_ENV = "(\\w*)\\=\"(\\w*)";
    }
}
