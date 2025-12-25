using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

/// <summary>
/// Represents the body of the Apple LRC XML.
/// </summary>
[XmlRoot(ElementName="body", Namespace="http://www.w3.org/ns/ttml")]
public class XmlBody
{
    [XmlElement(ElementName="div", Namespace="http://www.w3.org/ns/ttml")] 
    public List<XmlDiv> Div { get; set; } 

    [XmlAttribute(AttributeName="dur", Namespace="")] 
    public string Dur { get; set; } 

    [XmlText] 
    public string Text { get; set; } 
}