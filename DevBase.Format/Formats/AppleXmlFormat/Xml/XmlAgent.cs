using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleXmlFormat.Xml;

[XmlRoot(ElementName="agent", Namespace="http://www.w3.org/ns/ttml#metadata")]
public class XmlAgent
{
    [XmlElement(ElementName="name", Namespace="http://www.w3.org/ns/ttml#metadata")] 
    public XmlName Name; 

    [XmlAttribute(AttributeName="type", Namespace="")] 
    public string Type; 

    [XmlAttribute(AttributeName="id", Namespace="http://www.w3.org/XML/1998/namespace")] 
    public string Id; 

    [XmlText] 
    public string Text; 
}