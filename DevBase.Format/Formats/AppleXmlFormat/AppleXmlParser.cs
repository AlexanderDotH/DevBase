using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using DevBase.Format.Formats.AppleXmlFormat.Xml;
using DevBase.Format.Formats.RmmlFormat.Json;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Dumpify;

namespace DevBase.Format.Formats.AppleXmlFormat;

public class AppleXmlParser : IFileFormat<AList<RichLyrics>>
{
    public AList<RichLyrics> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }

    public AList<RichLyrics> FormatFromString(string lyricString)
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

        if (!tt.Timing.SequenceEqual("Word"))
            throw new System.Exception("Wrong timing format");

        AList<RichLyrics> richLyrics = new AList<RichLyrics>();

        for (var i = 0; i < tt.Body.LyricsBlocks.Count; i++)
        {
            XmlLyricsBlock block = tt.Body.LyricsBlocks[i];

            for (var j = 0; j < block.Lyrics.Count; j++)
            {
                richLyrics.Add(ProcessBlock(block.Lyrics[j]));
            }
        }

        return richLyrics;
    }

    private RichLyrics ProcessBlock(XmlLyric lyricBlock)
    {
        List<RichLyricsElement> richLyricsElements = new List<RichLyricsElement>();

        StringBuilder fullText = new StringBuilder();

        for (var k = 0; k < lyricBlock.LyricElements.Count; k++)
        {
            XmlLyricElement currentElement = lyricBlock.LyricElements[k];

            // TODO: Add background voices (ELRC v2?)
            if (!String.IsNullOrEmpty(currentElement.Role))
                continue;
            
            TimeSpan lStartTimeSpan = ParseTimeStamp(currentElement.Begin);
            TimeSpan lEndTimeSpan = ParseTimeStamp(currentElement.End);

            richLyricsElements.Add(new RichLyricsElement()
            {
                Start = Convert.ToInt64(lStartTimeSpan.TotalMilliseconds),
                End = Convert.ToInt64(lEndTimeSpan.TotalMilliseconds),
                Line = currentElement.Text
            });

            fullText.Append(
                k != lyricBlock.LyricElements.Count() - 1 ? currentElement.Text + " " : currentElement.Text);
        }

        TimeSpan sTimeSpan = ParseTimeStamp(lyricBlock.Begin);
        TimeSpan eTimeSpan = ParseTimeStamp(lyricBlock.End);

        RichLyrics richLyric = new RichLyrics()
        {
            Start = Convert.ToInt64(sTimeSpan.TotalMilliseconds),
            End = Convert.ToInt64(eTimeSpan.TotalMilliseconds),
            FullLine = fullText.ToString(),
            Elements = richLyricsElements
        };

        return richLyric;
    }

    private TimeSpan ParseTimeStamp(string time)
    {
        TimeSpan parsed;

        string[] formats = new[] { "ss\\.fff", "m\\:ss\\.fff", "mm\\:ss\\.fff", "hh\\:mm\\:ss\\.fff" };

        for (int i = 0; i < formats.Length; i++)
        {
            if (TimeSpan.TryParseExact(time, formats[i], null, TimeSpanStyles.None, out parsed))
                return parsed;
        }

        throw new System.Exception("Cannot format timestamp");
    }
    
    public string FormatToString(AList<RichLyrics> content)
    {
        throw new NotSupportedException();
    }
}