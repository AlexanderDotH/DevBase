using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleRichXmlFormat.Xml;

[XmlRoot(ElementName="iTunesMetadata")]
public class XmlITunesMetadata
{
    [XmlElement(ElementName="songwriters")] 
    public XmlSongwriters Songwriters; 

    [XmlAttribute(AttributeName="xmlns", Namespace="")] 
    public string Xmlns; 

    [XmlAttribute(AttributeName="leadingSilence", Namespace="")] 
    public string LeadingSilence; 

    [XmlText] 
    public string Text; 
}