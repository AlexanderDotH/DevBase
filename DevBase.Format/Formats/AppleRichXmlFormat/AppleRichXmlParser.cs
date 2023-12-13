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

public class AppleRichXmlParser : FileFormat<string, AList<RichTimeStampedLyric>>
{
    public override AList<RichTimeStampedLyric> Parse(string from)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(XmlTt));

        string unescaped = Regex.Unescape(from);

        using StringReader reader = new StringReader(unescaped);
        using XmlReader xmlReader = XmlReader.Create(reader);

        if (!serializer.CanDeserialize(xmlReader))
            return HandleException("Cannot read Xml file");

        XmlTt tt = (XmlTt)serializer.Deserialize(xmlReader);

        if (tt == null)
            return HandleException("Failed to parse xml file");

        if (!tt.Timing.SequenceEqual("Word"))
            return HandleException("Wrong timing format");

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
            
            TimeSpan lStartTimeSpan;
            TimeSpan lEndTimeSpan;

            if (!TimeUtils.TryParseTimeStamp(currentElement.Begin, out lStartTimeSpan))
                return HandleException($"Error parsing timestamp {currentElement.Begin}");
            
            if (!TimeUtils.TryParseTimeStamp(currentElement.End, out lEndTimeSpan))
                return HandleException($"Error parsing timestamp {currentElement.End}");
            
            words.Add(new RichTimeStampedWord()
            {
                StartTime = lStartTimeSpan,
                EndTime = lEndTimeSpan,
                Word = LyricsUtils.EditLine(currentElement.Text)
            });

            fullText.Append(
                k != lyricBlock.LyricElements.Count() - 1 ? currentElement.Text + " " : currentElement.Text);
        }

        TimeSpan sTimeSpan;
        TimeSpan eTimeSpan;

        if (!TimeUtils.TryParseTimeStamp(lyricBlock.Begin, out sTimeSpan))
            return HandleException($"Error parsing timestamp {lyricBlock.Begin}");
        
        if (!TimeUtils.TryParseTimeStamp(lyricBlock.End, out eTimeSpan))
            return HandleException($"Error parsing timestamp {lyricBlock.End}");
        
        RichTimeStampedLyric richLyric = new RichTimeStampedLyric()
        {
            StartTime = sTimeSpan,
            EndTime = eTimeSpan,
            Text = LyricsUtils.EditLine(fullText.ToString()),
            Words = words
        };

        return richLyric;
    }
}