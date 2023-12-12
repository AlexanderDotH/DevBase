using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using DevBase.Format.Formats.AppleLrcXmlFormat.Xml;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;

namespace DevBase.Format.Formats.AppleLrcXmlFormat;

public class AppleLrcXmlParser : FileFormat
{
    public AList<TimeStampedLyric> FormatFromFile(string filePath)
    {
        throw new NotImplementedException();
    }

    public C Parse<F, C>(F from)
    {
        
    }
    
    public AList<TimeStampedLyric> FormatFromString(string lyricString)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(XmlTt));

        string unescaped = Regex.Unescape(lyricString);

        using StringReader reader = new StringReader(unescaped);
        using XmlReader xmlReader = XmlReader.Create(reader);

        if (!serializer.CanDeserialize(xmlReader))
            throw new System.Exception("Cannot read Xml file");

        XmlTt tt = (XmlTt)serializer.Deserialize(xmlReader);

        if (tt == null)
            throw new System.Exception("Failed to parse xml file");

        if (!tt.Timing.SequenceEqual("Line"))
            throw new System.Exception("Wrong timing format");

        AList<TimeStampedLyric> timeStampedLyrics = new AList<TimeStampedLyric>();

        for (int i = 0; i < tt.Body.Block.Parts.Count; i++)
        {
            XmlP part = tt.Body.Block.Parts[i];

            TimeStampedLyric lyric = new TimeStampedLyric()
            {
                StartTime = TimeUtils.ParseTimeStamp(part.Begin),
                Text = part.Text
            };
            
            timeStampedLyrics.Add(lyric);
        }

        return timeStampedLyrics;
    }

    public string FormatToString(AList<TimeStampedLyric> content)
    {
        throw new NotImplementedException();
    }

    public override C ParseTo<F, C>(F from)
    {
        if (typeof(C) is AList<TimeStampedLyric> && typeof(F) is string)
        {
            string content = (string)from;
        }
    }
}