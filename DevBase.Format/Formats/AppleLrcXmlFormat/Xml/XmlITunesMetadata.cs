using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

[XmlRoot(ElementName="iTunesMetadata", Namespace="http://music.apple.com/lyric-ttml-internal")]
public class XmlITunesMetadata
{
    [XmlElement(ElementName="songwriters", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public XmlSongwriters Songwriters; 

    [XmlAttribute(AttributeName="xmlns", Namespace="")] 
    public string Xmlns; 

    [XmlText] 
    public string Text; 
}