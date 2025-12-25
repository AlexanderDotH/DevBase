using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using System.Xml;
using System.Xml.Serialization;
using DevBase.Format.Formats.AppleLrcXmlFormat.Xml;
using DevBase.Format.Structure;
using DevBase.Format.Utilities;
using DevBase.Generics;

namespace DevBase.Format.Formats.AppleLrcXmlFormat;

/// <summary>
/// Parser for Apple's LRC XML format (TTML based).
/// </summary>
public class AppleLrcXmlParser : FileFormat<string, AList<TimeStampedLyric>>
{
    /// <summary>
    /// Parses the Apple LRC XML string content into a list of time-stamped lyrics.
    /// </summary>
    /// <param name="from">The XML string content.</param>
    /// <returns>A list of <see cref="TimeStampedLyric"/> objects.</returns>
    public override AList<TimeStampedLyric> Parse(string from)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(XmlTt));

        string unescaped = Regex.Unescape(from);

        using StringReader reader = new StringReader(unescaped);
        using XmlReader xmlReader = XmlReader.Create(reader);

        if (!serializer.CanDeserialize(xmlReader))
            return Error<object>("Cannot read Xml file");

        XmlTt tt = (XmlTt)serializer.Deserialize(xmlReader);

        if (tt == null)
            return Error<object>("Failed to parse xml file");

        if (!tt.Timing.SequenceEqual("Line"))
            return Error<object>("Wrong timing format");

        AList<TimeStampedLyric> timeStampedLyrics = new AList<TimeStampedLyric>();

        for (int i = 0; i < tt.Body.Div.Count; i++)
            timeStampedLyrics.AddRange(ProcessBlock(tt.Body.Div[i]));

        return timeStampedLyrics;
    }

    private AList<TimeStampedLyric> ProcessBlock(XmlDiv block)
    {
        AList<TimeStampedLyric> timeStampedLyrics = new AList<TimeStampedLyric>();

        for (var i = 0; i < block.P.Count; i++)
        {
            XmlP part = block.P[i];

            TimeSpan startTime;
            
            if (!TimeUtils.TryParseTimeStamp(part.Begin, out startTime))
                return Error<object>($"Error parsing timestamp {part.Begin}");
            
            TimeStampedLyric timeStampedLyric = new TimeStampedLyric()
            {
                StartTime = startTime,
                Text = LyricsUtils.EditLine(part.Text)
            };
            
            timeStampedLyrics.Add(timeStampedLyric);
        }

        return timeStampedLyrics;
    }
    
    /// <summary>
    /// Attempts to parse the Apple LRC XML string content.
    /// </summary>
    /// <param name="rawTtmlResponse">The XML string content.</param>
    /// <param name="timeStamped">The parsed list of lyrics, or null if parsing fails.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public override bool TryParse(string rawTtmlResponse, out AList<TimeStampedLyric> timeStamped)
    {
        string unescaped = Regex.Unescape(rawTtmlResponse);
        
        if (!unescaped.Contains("itunes:timing=\"Line\""))
        {
            timeStamped = null;
            return Error<bool>("Wrong timing format");
        }

        AList<TimeStampedLyric> timedLyrics = Parse(rawTtmlResponse);

        if (timedLyrics == null || timedLyrics.IsEmpty())
        {
            timeStamped = null;
            return Error<bool>("The parsed lyrics are null or empty");
        }

        timeStamped = timedLyrics;
        return true;
    }
}