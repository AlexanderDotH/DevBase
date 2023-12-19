using DevBase.Format;
using DevBase.Format.Formats.KLyricsFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.KLyricsFormat;

public class KLyricsTester
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
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\KLRC\\RickAstley.klyrics");
        
        AList<RichTimeStampedLyric> list = this._klyricsParser.ParseFromDisk(fileInfo);
        
        list.DumpConsole();
        Assert.AreEqual("Rick Astley", list.Get(0).Text);
    }
}