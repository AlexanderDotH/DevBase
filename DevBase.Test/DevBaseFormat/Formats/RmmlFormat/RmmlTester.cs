using System.Text;
using DevBase.Format;
using DevBase.Format.Formats.RmmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.RmmlFormat;

public class RmmlTester
{
    private FileFormatParser<AList<RichLyrics>> _srtParser;

    [SetUp]
    public void Setup()
    {
        this._srtParser = new FileFormatParser<AList<RichLyrics>>(new RmmlParser());
    }

    [Test]
    public void TestFormatFromFile()
    {
        FileInfo fileInfo =
            new FileInfo("..\\..\\..\\DevBaseFormatData\\RMML\\rick.rmml");

        string content = File.ReadAllText(fileInfo.FullName);
        
        AList<RichLyrics> list = this._srtParser.FormatFromString(content);
        
        list.GetAsList().DumpConsole();
        Assert.IsTrue(content.Contains(list.Get(0).FullLine));
    }
}