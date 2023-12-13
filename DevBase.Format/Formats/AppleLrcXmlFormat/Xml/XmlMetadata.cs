using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

[XmlRoot(ElementName="metadata", Namespace="http://www.w3.org/ns/ttml")]
public class XmlMetadata
{
    [XmlElement(ElementName="agent", Namespace="http://www.w3.org/ns/ttml#metadata")] 
    public XmlAgent Agent { get; set; } 

    [XmlElement(ElementName="iTunesMetadata", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public XmlITunesMetadata ITunesMetadata { get; set; } 
}