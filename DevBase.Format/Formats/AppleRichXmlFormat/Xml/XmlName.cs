using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleRichXmlFormat.Xml;

[XmlRoot(ElementName="name", Namespace="http://www.w3.org/ns/ttml#metadata")]
public class XmlName
{
    [XmlAttribute(AttributeName="type", Namespace="")] 
    public string Type; 

    [XmlText] 
    public string Text; 
}