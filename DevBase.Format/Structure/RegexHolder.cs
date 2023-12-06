﻿namespace DevBase.Format.Structure
{
    public class RegexHolder
    {
        public const string REGEX_TIMESTAMP = "\\[(([0-9]*)\\:([0-9]*)\\.([0-9]*))\\]";
        public const string REGEX_DETAILED_TIMESTAMP = "\\[(([0-9]*)\\:([0-9]*)\\:([0-9]*)\\.([0-9]*))\\]";
        public const string REGEX_METADATA = "(\\[{0}:(.*)\\])";
        public const string REGEX_GARBAGE = "\\D(\\?{0,2}).([:]).([\\w /]*)";
        public const string REGEX_ENV = "(\\w*)\\=\"(\\w*)";
        public const string REGEX_SRT_TIMESTAMPS = "(\\d...........)\\D....(............)";
        public const string REGEX_ELRC_DATA = "(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])(\\s-\\s)(\\[)([0-9]*)([:])([0-9]*)([:])(\\d+\\.\\d+|\\d+)(\\])\\s(.*$)";
        
        public const string REGEX_KLYRICS_TIMESTAMPS = "(\\[)([0-9]*)(\\,)([0-9]*)(\\])";
        public const string REGEX_KLRICS_DATA = "((\\()([0-9])(\\,)([0-9]*)(\\))([\\D]*)(\\()([0-9]*)(\\,)([0-9]*)(\\))(\\s))";
        public const string REGEX_KLYRICS_END = "(((\\()([0-9])(\\,)([0-9]*)(\\))([\\D]*$)))";
    }
}
