using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

[XmlRoot(ElementName="p", Namespace="http://www.w3.org/ns/ttml")]
public class XmlP
{
    [XmlAttribute(AttributeName="begin", Namespace="")] 
    public string Begin; 

    [XmlAttribute(AttributeName="end", Namespace="")] 
    public string End; 

    [XmlText] 
    public string Text; 
}