using System.Text.RegularExpressions;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Format.Formats.EnvFormat
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
            AList<string> lines = new AString(environment).AsList();

            ATupleList<string, string> elements = new ATupleList<string, string>();

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
