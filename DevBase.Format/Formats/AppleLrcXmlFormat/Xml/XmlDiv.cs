using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

[XmlRoot(ElementName="div", Namespace="http://www.w3.org/ns/ttml")]
public class XmlDiv
{
    [XmlElement(ElementName="p", Namespace="http://www.w3.org/ns/ttml")] 
    public List<XmlP> P { get; set; } 

    [XmlAttribute(AttributeName="begin", Namespace="")] 
    public string Begin { get; set; } 

    [XmlAttribute(AttributeName="end", Namespace="")] 
    public string End { get; set; } 

    [XmlAttribute(AttributeName="songPart", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public string SongPart { get; set; } 

    [XmlText] 
    public string Text { get; set; } 
}