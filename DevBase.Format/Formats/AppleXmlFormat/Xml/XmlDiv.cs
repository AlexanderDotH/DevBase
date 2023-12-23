using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleXmlFormat.Xml;

[XmlRoot(ElementName="div", Namespace="http://www.w3.org/ns/ttml")]
public class XmlDiv { 

    [XmlElement(ElementName="p", Namespace="http://www.w3.org/ns/ttml")] 
    public List<string> P { get; set; } 
}