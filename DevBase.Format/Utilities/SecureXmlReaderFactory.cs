using System.Xml;

namespace DevBase.Format.Utilities;

internal static class SecureXmlReaderFactory
{
    public static XmlReader Create(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        return XmlReader.Create(reader, CreateSettings());
    }

    public static XmlReaderSettings CreateSettings()
    {
        return new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = 10_000_000,
            MaxCharactersFromEntities = 1024
        };
    }
}
