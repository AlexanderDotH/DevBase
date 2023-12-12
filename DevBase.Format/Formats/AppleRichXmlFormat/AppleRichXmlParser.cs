using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using DevBase.Format.Formats.AppleRichXmlFormat.Xml;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;
using DevBase.IO;

namespace DevBase.Format.Formats.AppleRichXmlFormat;

public class AppleRichXmlParser : IFileFormat<AList<RichTimeStampedLyric>>
{
    public AList<RichTimeStampedLyric> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }

    public AList<RichTimeStampedLyric> FormatFromString(string lyricString)
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

        AList<RichTimeStampedLyric> richLyrics = new AList<RichTimeStampedLyric>();

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

    private RichTimeStampedLyric ProcessBlock(XmlLyric lyricBlock)
    {
        AList<RichTimeStampedWord> words = new AList<RichTimeStampedWord>();

        StringBuilder fullText = new StringBuilder();

        for (var k = 0; k < lyricBlock.LyricElements.Count; k++)
        {
            XmlLyricElement currentElement = lyricBlock.LyricElements[k];

            // TODO: Add background voices (ELRC v2?)
            if (!String.IsNullOrEmpty(currentElement.Role))
                continue;
            
            TimeSpan lStartTimeSpan = ParseTimeStamp(currentElement.Begin);
            TimeSpan lEndTimeSpan = ParseTimeStamp(currentElement.End);

            words.Add(new RichTimeStampedWord()
            {
                StartTime = lStartTimeSpan,
                EndTime = lEndTimeSpan,
                Word = currentElement.Text
            });

            fullText.Append(
                k != lyricBlock.LyricElements.Count() - 1 ? currentElement.Text + " " : currentElement.Text);
        }

        TimeSpan sTimeSpan = TimeUtils.ParseTimeStamp(lyricBlock.Begin);
        TimeSpan eTimeSpan = TimeUtils.ParseTimeStamp(lyricBlock.End);

        RichTimeStampedLyric richLyric = new RichTimeStampedLyric()
        {
            StartTime = sTimeSpan,
            EndTime = eTimeSpan,
            Text = fullText.ToString(),
            Words = words
        };

        return richLyric;
    }
    
    public string FormatToString(AList<RichTimeStampedLyric> content)
    {
        throw new NotSupportedException();
    }
}