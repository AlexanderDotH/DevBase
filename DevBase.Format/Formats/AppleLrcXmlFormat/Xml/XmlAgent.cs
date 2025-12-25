using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

/// <summary>
/// Represents metadata agent information in Apple LRC XML.
/// </summary>
[XmlRoot(ElementName="agent", Namespace="http://www.w3.org/ns/ttml#metadata")]
public class XmlAgent
{
    [XmlAttribute(AttributeName="type", Namespace="")] 
    public string Type { get; set; } 

    [XmlAttribute(AttributeName="id", Namespace="http://www.w3.org/XML/1998/namespace")] 
    public string Id { get; set; } 
}