using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using DevBase.Format.Formats.AppleXmlFormat.Xml;
using DevBase.Format.Structure;
using DevBase.Generics;

namespace DevBase.Format.Formats.AppleXmlFormat;

public class AppleXmlParser : FileFormat<string, AList<RawLyric>>
{
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