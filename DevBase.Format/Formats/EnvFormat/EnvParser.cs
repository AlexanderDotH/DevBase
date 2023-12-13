using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.EnvFormat
{
    public class EnvParser : FileFormat<string, ATupleList<string, string>>
    {
        private readonly Regex _regexEnv;

        public EnvParser()
        {
            this._regexEnv = new Regex(RegexHolder.REGEX_ENV);
        }

        // I just hate to see this pile of garbage but its not my priority and it still works. I guess?
        public override ATupleList<string, string> Parse(string from)
        {
            AList<string> lines = new AString(from).AsList();

            ATupleList<string, string> elements = new ATupleList<string, string>();

            lines.ForEach(s =>
            {
                if (s.Contains("="))
                {
                    if (this._regexEnv.IsMatch(s))
                    {
                        Match match = this._regexEnv.Match(s);
                        elements.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }
                }
            });

            return elements;
        }
    }
}
