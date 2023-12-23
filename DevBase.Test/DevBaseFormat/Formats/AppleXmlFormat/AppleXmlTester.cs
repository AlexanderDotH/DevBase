using DevBase.Format;
using DevBase.Format.Formats.AppleLrcXmlFormat;
using DevBase.Format.Formats.AppleRichXmlFormat;
using DevBase.Format.Formats.AppleXmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.AppleXmlFormat;

public class AppleXmlTester
{
    private FileParser<AppleRichXmlParser, AList<RichTimeStampedLyric>> _richXmlParser;
    private FileParser<AppleLrcXmlParser, AList<TimeStampedLyric>> _lineXmlParser;
    private FileParser<AppleXmlParser, AList<RawLyric>> _rawXmlParser;


    [SetUp]
    public void Setup()
    {
        this._richXmlParser = new FileParser<AppleRichXmlParser, AList<RichTimeStampedLyric>>();
        this._lineXmlParser = new FileParser<AppleLrcXmlParser, AList<TimeStampedLyric>>();
        this._rawXmlParser = new FileParser<AppleXmlParser, AList<RawLyric>>();
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
    public void TestTryParseRichXml()
    {
        AList<RichTimeStampedLyric> richTimeStampedLyrics = null;
        
        FileInfo fileInfo =
            new FileInfo("..\\..\\..\\DevBaseFormatData\\XML\\RickAstley.xml");

        this._richXmlParser.TryParseFromDisk(fileInfo.FullName, out richTimeStampedLyrics);

        richTimeStampedLyrics.DumpConsole();
        
        Assert.AreEqual(richTimeStampedLyrics.Get(0).Text, "We're no strangers to love");
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

    [Test]
    public void TestTryParseTimeStampedXml()
    {
        AList<TimeStampedLyric> timeStampedLyrics = null;
        
        FileInfo fileInfo =
            new FileInfo("..\\..\\..\\DevBaseFormatData\\XML\\Liebe.xml");

        this._lineXmlParser.TryParseFromDisk(fileInfo.FullName, out timeStampedLyrics);

        timeStampedLyrics.DumpConsole();
        
        Assert.AreEqual(timeStampedLyrics.Get(0).Text, "Die Sterne ziehen vorbei, Lichtgeschwindigkeit");
    }
    
    [Test]
    public void TestFormatFromNone()
    {
        FileInfo fileInfo =
            new FileInfo("..\\..\\..\\DevBaseFormatData\\XML\\RickAstleyUnsynced.xml");

        AList<RawLyric> list = this._rawXmlParser.ParseFromDisk(fileInfo);

        list.GetAsList().DumpConsole();
        Assert.AreEqual(list.Get(0).Text, "Move yourself");
    }
    
    [Test]
    public void TestTryParseFromNone()
    {
        AList<RawLyric> rawLyrics = null;
        
        FileInfo fileInfo =
            new FileInfo("..\\..\\..\\DevBaseFormatData\\XML\\RickAstleyUnsynced.xml");

        this._rawXmlParser.TryParseFromDisk(fileInfo.FullName, out rawLyrics);

        rawLyrics.GetAsList().DumpConsole();
        Assert.AreEqual(rawLyrics.Get(0).Text, "Move yourself");
    }
}