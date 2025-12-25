using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleLrcXmlFormat.Xml;

/// <summary>
/// Represents the header of the Apple LRC XML.
/// </summary>
[XmlRoot(ElementName="head", Namespace="http://www.w3.org/ns/ttml")]
public class XmlHead
{
    [XmlElement(ElementName="metadata", Namespace="http://www.w3.org/ns/ttml")] 
    public XmlMetadata Metadata { get; set; } 
}