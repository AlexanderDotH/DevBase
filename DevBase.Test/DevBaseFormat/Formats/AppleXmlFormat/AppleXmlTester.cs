using DevBase.Format;
using DevBase.Format.Formats.AppleLrcXmlFormat;
using DevBase.Format.Formats.AppleRichXmlFormat;
using DevBase.Format.Formats.AppleXmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.AppleXmlFormat;

public class AppleXmlTester : FormatTest
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
        AList<RichTimeStampedLyric> list = 
            this._richXmlParser.ParseFromDisk(this.GetTestFile("XML", "RickAstley.xml"));

        list.GetAsList().DumpConsole();
        Assert.That(list.Get(0).Text, Is.EqualTo("We're no strangers to love"));
    }
    
    [Test]
    public void TestTryParseRichXml()
    {
        AList<RichTimeStampedLyric> richTimeStampedLyrics = null;
        
        this._richXmlParser.TryParseFromDisk(
            this.GetTestFile("XML", "RickAstley.xml").FullName, 
            out richTimeStampedLyrics);

        richTimeStampedLyrics.DumpConsole();
        Assert.That(richTimeStampedLyrics.Get(0).Text, Is.EqualTo("We're no strangers to love"));
    }
    
    [Test]
    public void TestFormatFromFileLine()
    {
        AList<TimeStampedLyric> list = 
            this._lineXmlParser.ParseFromDisk(
                this.GetTestFile("XML", "Liebe.xml"));

        list.GetAsList().DumpConsole();
        Assert.That(list.Get(0).Text, Is.EqualTo("Die Sterne ziehen vorbei, Lichtgeschwindigkeit"));
    }

    [Test]
    public void TestTryParseTimeStampedXml()
    {
        AList<TimeStampedLyric> timeStampedLyrics = null;

        this._lineXmlParser.TryParseFromDisk(
            this.GetTestFile("XML", "Liebe.xml").FullName, 
            out timeStampedLyrics);

        timeStampedLyrics.DumpConsole();
        Assert.That(timeStampedLyrics.Get(0).Text, Is.EqualTo("Die Sterne ziehen vorbei, Lichtgeschwindigkeit"));
    }
    
    [Test]
    public void TestFormatFromNone()
    {
        AList<RawLyric> list = this._rawXmlParser.ParseFromDisk(
            this.GetTestFile("XML", "RickAstleyUnsynced.xml"));

        list.GetAsList().DumpConsole();
        Assert.That(list.Get(0).Text, Is.EqualTo("Move yourself"));
    }
    
    [Test]
    public void TestTryParseFromNone()
    {
        AList<RawLyric> rawLyrics = null;

        this._rawXmlParser.TryParseFromDisk(
            this.GetTestFile("XML", "RickAstleyUnsynced.xml").FullName, 
            out rawLyrics);

        rawLyrics.GetAsList().DumpConsole();
        Assert.That(rawLyrics.Get(0).Text, Is.EqualTo("Move yourself"));
    }
}
