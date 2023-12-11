using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

[XmlRoot(ElementName="p", Namespace="http://www.w3.org/ns/ttml")]
public class XmlP
{
    [XmlAttribute(AttributeName="begin", Namespace="")] 
    public DateTime Begin; 

    [XmlAttribute(AttributeName="end", Namespace="")] 
    public DateTime End; 

    [XmlText] 
    public string Text; 
}