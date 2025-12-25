using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

/// <summary>
/// Represents songwriters information in Apple LRC XML.
/// </summary>
[XmlRoot(ElementName="songwriters", Namespace="http://music.apple.com/lyric-ttml-internal")]
public class XmlSongwriters
{
    [XmlElement(ElementName="songwriter", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public List<string> Songwriter { get; set; } 
}