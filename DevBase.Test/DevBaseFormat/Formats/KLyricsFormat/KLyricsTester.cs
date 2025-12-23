using DevBase.Format;
using DevBase.Format.Formats.KLyricsFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.KLyricsFormat;

public class KLyricsTester : FormatTest
{
    private FileParser<KLyricsParser, AList<RichTimeStampedLyric>> _klyricsParser;

    [SetUp]
    public void Setup()
    {
        this._klyricsParser = new FileParser<KLyricsParser, AList<RichTimeStampedLyric>>();
    }

    [Test]
    public void TestFormatFromFile()
    {
        AList<RichTimeStampedLyric> list = 
            this._klyricsParser.ParseFromDisk(GetTestFile("KLRC", "RickAstley.klyrics"));
        
        list.DumpConsole();
        Assert.That(list.Get(0).Text, Is.EqualTo("Rick Astley"));
    }
}
