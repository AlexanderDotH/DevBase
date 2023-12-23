using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleXmlFormat.Xml;

[XmlRoot(ElementName="metadata", Namespace="http://www.w3.org/ns/ttml")]
public class XmlMetadata { 

    [XmlElement(ElementName="iTunesMetadata", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public XmlITunesMetadata ITunesMetadata { get; set; } 
}