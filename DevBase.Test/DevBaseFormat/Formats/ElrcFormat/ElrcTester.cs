using DevBase.Format;
using DevBase.Format.Formats.ElrcFormat;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.ElrcFormat;

public class ElrcTester : FormatTest
{
    private ElrcParser _elrcParser;

    [SetUp]
    public void Setup()
    {
        this._elrcParser = new ElrcParser();
    }

    [Test]
    public void TestFormatFromFile()
    {
        string content = AFile.ReadFileToObject(GetTestFile("ELRC", "rick.elrc")).ToStringData();

        AList<RichTimeStampedLyric> list = this._elrcParser.Parse(content);
        
        list.GetAsList().DumpConsole();
        Assert.That(list.Get(0).Text, Is.EqualTo("Never gonna give you up"));
    }
    
    [Test]
    public void TestFormatToFile()
    {
        string content = AFile.ReadFileToObject(GetTestFile("ELRC", "rick.elrc")).ToStringData();
        
        AList<RichTimeStampedLyric> list = this._elrcParser.Parse(content);

        string formatted = this._elrcParser.Revert(list);
        
        formatted.DumpConsole();
        Assert.That(formatted, Is.EqualTo(content));
    }
}
