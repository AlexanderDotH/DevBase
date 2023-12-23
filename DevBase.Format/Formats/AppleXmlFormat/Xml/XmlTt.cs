using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleXmlFormat.Xml;

[XmlRoot(ElementName="tt", Namespace="http://www.w3.org/ns/ttml")]
public class XmlTt { 

    [XmlElement(ElementName="head", Namespace="http://www.w3.org/ns/ttml")] 
    public XmlHead Head { get; set; } 

    [XmlElement(ElementName="body", Namespace="http://www.w3.org/ns/ttml")] 
    public XmlBody Body { get; set; } 

    [XmlAttribute(AttributeName="xmlns", Namespace="")] 
    public string Xmlns { get; set; } 

    [XmlAttribute(AttributeName="itunes", Namespace="http://www.w3.org/2000/xmlns/")] 
    public string Itunes { get; set; } 

    [XmlAttribute(AttributeName="timing", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public string Timing { get; set; } 

    [XmlAttribute(AttributeName="lang", Namespace="http://www.w3.org/XML/1998/namespace")] 
    public string Lang { get; set; } 

    [XmlText] 
    public string Text { get; set; } 
}