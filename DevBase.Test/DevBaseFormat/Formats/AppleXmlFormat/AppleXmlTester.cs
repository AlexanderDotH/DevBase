using DevBase.Format;
using DevBase.Format.Formats.AppleXmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.AppleXmlFormat;

public class AppleXmlTester
{
    private FileFormatParser<AList<RichLyrics>> _xmlParser;

    [SetUp]
    public void Setup()
    {
        this._xmlParser = new FileFormatParser<AList<RichLyrics>>(new AppleXmlParser());
    }

    [Test]
    public void TestFormatFromFile()
    {
        FileInfo fileInfo =
            new FileInfo("..\\..\\..\\DevBaseFormatData\\XML\\RickAstley.xml");

        string content = File.ReadAllText(fileInfo.FullName);
        
        AList<RichLyrics> list = this._xmlParser.FormatFromString(content);

        list.GetAsList().DumpConsole();
        Assert.AreEqual(list.Get(0).FullLine, "We're no strangers to love");
    }
}