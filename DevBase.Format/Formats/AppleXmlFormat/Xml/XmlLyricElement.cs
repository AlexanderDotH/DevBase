using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleXmlFormat.Xml;

[XmlRoot(ElementName="span", Namespace="http://www.w3.org/ns/ttml")]
public class XmlLyricElement
{
    [XmlAttribute(AttributeName="begin", Namespace="")] 
    public string Begin; 

    [XmlAttribute(AttributeName="end", Namespace="")] 
    public string End; 

    [XmlText] 
    public string Text; 

    [XmlElement(ElementName="span", Namespace="http://www.w3.org/ns/ttml")] 
    public List<XmlLyricElement> LyricElements { get; set; } 
    
    [XmlAttribute(AttributeName="role", Namespace="http://www.w3.org/ns/ttml#metadata")] 
    public string Role; 
}