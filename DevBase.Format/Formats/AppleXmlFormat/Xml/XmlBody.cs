﻿using System.Xml.Serialization;

namespace DevBase.Format.Formats.AppleXmlFormat.Xml;

[XmlRoot(ElementName="body", Namespace="http://www.w3.org/ns/ttml")]
public class XmlBody { 

    [XmlElement(ElementName="div", Namespace="http://www.w3.org/ns/ttml")] 
    public List<XmlDiv> Div { get; set; } 
}