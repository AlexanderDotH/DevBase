using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevBase.Generic;
using DevBase.IO;
using DevBase.Typography;
using DevBase.Utilities;
using DevBaseFormat.Structure;

namespace DevBaseFormat.Formats.EnvFormat
{
    public class EnvParser<T> : IFileFormat<T>
    {
        public T FormatFromFile(string filePath)
        {
            AFileObject file = AFile.ReadFile(filePath);
            return FormatFromString(file.ToStringData());
        }

        public T FormatFromString(string environment)
        {
            GenericList<string> lines = new AString(environment).AsList();

            GenericTupleList<string, string> elements = new GenericTupleList<string, string>();

            lines.ForEach(s =>
            {
                if (s.Contains("="))
                {
                    Regex regex = new Regex(RegexHolder.REGEX_ENV);
                    if (regex.IsMatch(s))
                    {
                        Match match = regex.Match(s);
                        elements.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }
                }
            });

            if (!elements.IsEmpty())
                return (T)(object)elements;

            return default;
        }
    }
}
