using System.Text;
using DevBase.Format;
using DevBase.Format.Formats.RmmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.RmmlFormat;

public class RmmlTester : FormatTest
{
    private FileParser<RmmlParser, AList<RichTimeStampedLyric>> _rmmlParser;

    [SetUp]
    public void Setup()
    {
        this._rmmlParser = new FileParser<RmmlParser, AList<RichTimeStampedLyric>>();
    }

    [Test]
    public void TestFormatFromFile()
    {
        string content = File.ReadAllText(GetTestFile("RMML", "rick.rmml").FullName);
        
        AList<RichTimeStampedLyric> list = this._rmmlParser.ParseFromString(content);
        
        list.GetAsList().DumpConsole();
        
        Assert.IsTrue(content.Contains(list.Get(0).Text));
    }
}