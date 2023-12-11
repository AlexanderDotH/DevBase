using DevBase.Format;
using DevBase.Format.Formats.AppleRichXmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.AppleXmlFormat;

public class AppleXmlTester
{
    private FileFormatParser<AList<RichTimeStampedLyric>> _xmlParser;

    [SetUp]
    public void Setup()
    {
        this._xmlParser = new FileFormatParser<AList<RichTimeStampedLyric>>(new AppleRichXmlParser());
    }

    [Test]
    public void TestFormatFromFile()
    {
        FileInfo fileInfo =
            new FileInfo("..\\..\\..\\DevBaseFormatData\\XML\\RickAstley.xml");

        string content = File.ReadAllText(fileInfo.FullName);
        
        AList<RichTimeStampedLyric> list = this._xmlParser.FormatFromString(content);

        list.GetAsList().DumpConsole();
        Assert.AreEqual(list.Get(0).Text, "We're no strangers to love");
    }
}