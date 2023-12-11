using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleRichXmlFormat.Xml;

[XmlRoot(ElementName="tt", Namespace="http://www.w3.org/ns/ttml")]
public class XmlTt { 

    [XmlElement(ElementName="head", Namespace="http://www.w3.org/ns/ttml")] 
    public XmlHead Head; 

    [XmlElement(ElementName="body", Namespace="http://www.w3.org/ns/ttml")]
    public XmlBody Body; 

    [XmlAttribute(AttributeName="xmlns")] 
    public string Xmlns; 

    [XmlAttribute(AttributeName="itunes", Namespace="http://www.w3.org/2000/xmlns/")] 
    public string Itunes; 

    [XmlAttribute(AttributeName="ttm", Namespace="http://www.w3.org/2000/xmlns/")] 
    public string Ttm; 

    [XmlAttribute(AttributeName="timing", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public string Timing; 

    [XmlAttribute(AttributeName="lang", Namespace="http://www.w3.org/XML/1998/namespace")] 
    public string Lang; 

    [XmlText] 
    public string Text; 
}

