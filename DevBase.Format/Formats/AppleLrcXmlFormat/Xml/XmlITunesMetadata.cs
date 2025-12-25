using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

/// <summary>
/// Represents iTunes specific metadata in Apple LRC XML.
/// </summary>
[XmlRoot(ElementName="iTunesMetadata", Namespace="http://music.apple.com/lyric-ttml-internal")]
public class XmlITunesMetadata
{
    [XmlElement(ElementName="songwriters", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public XmlSongwriters Songwriters { get; set; } 

    [XmlAttribute(AttributeName="xmlns", Namespace="")] 
    public string Xmlns { get; set; } 

    [XmlAttribute(AttributeName="leadingSilence", Namespace="")] 
    public double LeadingSilence { get; set; } 

    [XmlText] 
    public string Text { get; set; } 
}