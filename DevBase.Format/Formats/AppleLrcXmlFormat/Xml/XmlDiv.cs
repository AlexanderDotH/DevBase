using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

[XmlRoot(ElementName="div", Namespace="http://www.w3.org/ns/ttml")]
public class XmlDiv
{
    [XmlElement(ElementName="p", Namespace="http://www.w3.org/ns/ttml")] 
    public List<XmlP> Parts; 

    [XmlAttribute(AttributeName="begin", Namespace="")] 
    public DateTime Begin; 

    [XmlAttribute(AttributeName="end", Namespace="")] 
    public DateTime End; 

    [XmlText] 
    public string Text; 
}