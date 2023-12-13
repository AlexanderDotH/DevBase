using DevBase.Format;
using DevBase.Format.Formats.AppleLrcXmlFormat;
using DevBase.Format.Formats.AppleRichXmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.AppleXmlFormat;

public class AppleXmlTester
{
    private FileParser<AppleRichXmlParser, AList<RichTimeStampedLyric>> _richXmlParser;
    private FileParser<AppleLrcXmlParser, AList<TimeStampedLyric>> _lineXmlParser;

    [SetUp]
    public void Setup()
    {
        this._richXmlParser = new FileParser<AppleRichXmlParser, AList<RichTimeStampedLyric>>();
        this._lineXmlParser = new FileParser<AppleLrcXmlParser, AList<TimeStampedLyric>>();
    }

    [Test]
    public void TestFormatFromFileRich()
    {
        FileInfo fileInfo =
            new FileInfo("..\\..\\..\\DevBaseFormatData\\XML\\RickAstley.xml");

        
        
        AList<RichTimeStampedLyric> list = this._richXmlParser.ParseFromDisk(fileInfo);

        list.GetAsList().DumpConsole();
        Assert.AreEqual(list.Get(0).Text, "We're no strangers to love");
    }
    
    [Test]
    public void TestFormatFromFileLine()
    {
        FileInfo fileInfo =
            new FileInfo("..\\..\\..\\DevBaseFormatData\\XML\\Liebe.xml");

        AList<TimeStampedLyric> list = this._lineXmlParser.ParseFromDisk(fileInfo);

        list.GetAsList().DumpConsole();
        Assert.AreEqual(list.Get(0).Text, "Die Sterne ziehen vorbei, Lichtgeschwindigkeit");
    }
}