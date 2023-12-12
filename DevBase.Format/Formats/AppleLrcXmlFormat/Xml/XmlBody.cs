using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

[XmlRoot(ElementName="body", Namespace="http://www.w3.org/ns/ttml")]
public class XmlBody
{
    [XmlElement(ElementName="div", Namespace="http://www.w3.org/ns/ttml")] 
    public XmlDiv Block; 

    [XmlAttribute(AttributeName="dur", Namespace="")] 
    public DateTime Duration; 

    [XmlText] 
    public string Text; 
}