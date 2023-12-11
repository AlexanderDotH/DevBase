using DevBase.Format.Structure;
using DevBase.Generics;

namespace DevBase.Format.Formats.AppleLrcXmlFormat;

public class AppleLrcXmlParser : IFileFormat<AList<TimeStampedLyric>>
{
    public AList<TimeStampedLyric> FormatFromFile(string filePath)
    {
        throw new NotImplementedException();
    }

    public AList<TimeStampedLyric> FormatFromString(string lyricString)
    {
        return null;
    }

    public string FormatToString(AList<TimeStampedLyric> content)
    {
        throw new NotImplementedException();
    }
}