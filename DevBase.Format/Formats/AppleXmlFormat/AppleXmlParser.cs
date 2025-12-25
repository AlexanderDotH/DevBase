using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using DevBase.Format.Formats.AppleXmlFormat.Xml;
using DevBase.Format.Structure;
using DevBase.Generics;

namespace DevBase.Format.Formats.AppleXmlFormat;

/// <summary>
/// Parser for Apple's basic XML format (TTML based with no timing).
/// </summary>
public class AppleXmlParser : FileFormat<string, AList<RawLyric>>
{
    /// <summary>
    /// Parses the Apple XML string content into a list of raw lyrics.
    /// </summary>
    /// <param name="from">The XML string content.</param>
    /// <returns>A list of <see cref="RawLyric"/> objects.</returns>
    public override AList<RawLyric> Parse(string from)
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

        if (!tt.Timing.SequenceEqual("None"))
            return Error<object>("Wrong timing format");

        AList<RawLyric> rawLyrics = new AList<RawLyric>();

        for (int i = 0; i < tt.Body.Div.Count; i++)
            rawLyrics.AddRange(ProcessBlock(tt.Body.Div[i]));

        return rawLyrics;
    }

    private AList<RawLyric> ProcessBlock(XmlDiv block)
    {
        AList<RawLyric> proceeded = new AList<RawLyric>();

        for (var i = 0; i < block.P.Count; i++)
        {
            string p = block.P[i];

            RawLyric rawLyric = new RawLyric()
            {
                Text = p
            };
            
            proceeded.Add(rawLyric);
        }
        
        return proceeded;
    }
    
    /// <summary>
    /// Attempts to parse the Apple XML string content.
    /// </summary>
    /// <param name="rawTtmlResponse">The XML string content.</param>
    /// <param name="rawLyrics">The parsed list of raw lyrics, or null if parsing fails.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public override bool TryParse(string rawTtmlResponse, out AList<RawLyric> rawLyrics)
    {
        string unescaped = Regex.Unescape(rawTtmlResponse);
        
        if (!unescaped.Contains("itunes:timing=\"None\""))
        {
            rawLyrics = null;
            return Error<bool>("Wrong timing format");
        }

        AList<RawLyric> rawParsedLyrics = Parse(rawTtmlResponse);

        if (rawParsedLyrics == null || rawParsedLyrics.IsEmpty())
        {
            rawLyrics = null;
            return Error<bool>("The parsed lyrics are null or empty");
        }

        rawLyrics = rawParsedLyrics;
        return true;
    }
}