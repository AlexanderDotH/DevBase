using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

[XmlRoot(ElementName="p", Namespace="http://www.w3.org/ns/ttml")]
public class XmlP
{
    [XmlAttribute(AttributeName="begin", Namespace="")] 
    public string Begin { get; set; } 

    [XmlAttribute(AttributeName="end", Namespace="")] 
    public string End { get; set; } 

    [XmlAttribute(AttributeName="key", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public string Key { get; set; } 

    [XmlAttribute(AttributeName="agent", Namespace="http://www.w3.org/ns/ttml#metadata")] 
    public string Agent { get; set; } 

    [XmlText] 
    public string Text { get; set; } 
}