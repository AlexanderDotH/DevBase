using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleXmlFormat.Xml;

[XmlRoot(ElementName="head", Namespace="http://www.w3.org/ns/ttml")]
public class XmlHead
{
    [XmlElement(ElementName="metadata", Namespace="http://www.w3.org/ns/ttml")] 
    public XmlMetadata Metadata; 
}