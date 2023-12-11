using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleRichXmlFormat.Xml;

[XmlRoot(ElementName="div", Namespace="http://www.w3.org/ns/ttml")]
public class XmlLyricsBlock
{
    [XmlElement(ElementName="p", Namespace="http://www.w3.org/ns/ttml")] 
    public List<XmlLyric> Lyrics; 

    [XmlAttribute(AttributeName="begin", Namespace="")] 
    public string Begin; 

    [XmlAttribute(AttributeName="end", Namespace="")] 
    public string End; 

    [XmlText] 
    public string Text; 
}