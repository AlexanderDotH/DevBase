using DevBase.Format;
using DevBase.Format.Formats.KLyricsFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.KLyricsFormat;

public class KLyricsTester
{
    private FileFormatParser<AList<RichTimeStampedLyric>> _klyricsParser;

    [SetUp]
    public void Setup()
    {
        this._klyricsParser = new FileFormatParser<AList<RichTimeStampedLyric>>(new KLyricsParser());
    }

    [Test]
    public void TestFormatFromFile()
    {
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\KLRC\\RickAstley.klyrics");
        
        AList<RichTimeStampedLyric> list = this._klyricsParser.FormatFromFile(fileInfo.FullName);
        
        list.GetAsList().DumpConsole();
        Assert.AreEqual(list.Get(0).Text, "Rick Astley");
    }
}