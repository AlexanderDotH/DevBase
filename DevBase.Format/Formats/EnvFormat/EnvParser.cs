﻿using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.EnvFormat
{
    public class EnvParser : FileFormat<string, ATupleList<string, string>>
    {
        // I just hate to see this pile of garbage but its not my priority and it still works. I guess?
        public override ATupleList<string, string> Parse(string from)
        {
            AList<string> lines = new AString(from).AsList();

            ATupleList<string, string> elements = new ATupleList<string, string>();

            lines.ForEach(s =>
            {
                if (s.Contains("="))
                {
                    if (RegexHolder.RegexEnv.IsMatch(s))
                    {
                        Match match = RegexHolder.RegexEnv.Match(s);
                        elements.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }
                }
            });

            return elements;
        }
        
        public override bool TryParse(string from, out ATupleList<string, string> parsed)
        {
            ATupleList<string, string> p = Parse(from);

            if (p == null || p.IsEmpty())
            {
                parsed = null;
                return Error<bool>("The parsed result is null or empty");
            }

            parsed = p;
            return true;
        }
    }
}
