using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleRichXmlFormat.Xml;

[XmlRoot(ElementName="p", Namespace="http://www.w3.org/ns/ttml")]
public class XmlLyric
{
    [XmlElement(ElementName="span", Namespace="http://www.w3.org/ns/ttml")] 
    public List<XmlLyricElement> LyricElements;

    [XmlAttribute(AttributeName="begin", Namespace="")] 
    public string Begin; 

    [XmlAttribute(AttributeName="end", Namespace="")] 
    public string End; 

    [XmlAttribute(AttributeName="key", Namespace="http://music.apple.com/lyric-ttml-internal")] 
    public string Key; 

    [XmlAttribute(AttributeName="agent", Namespace="http://www.w3.org/ns/ttml#metadata")] 
    public string Agent; 

    [XmlText] 
    public string Text; 
}