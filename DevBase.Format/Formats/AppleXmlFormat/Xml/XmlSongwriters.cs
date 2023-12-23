using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleXmlFormat.Xml;

[XmlRoot(ElementName="songwriters", Namespace="http://music.apple.com/lyric-ttml-internal")]
public class XmlSongwriters { 

    [XmlElement(ElementName="songwriter", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public List<string> Songwriter { get; set; } 
}