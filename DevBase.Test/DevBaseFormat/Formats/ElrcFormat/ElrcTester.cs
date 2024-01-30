using DevBase.Format;
using DevBase.Format.Formats.ElrcFormat;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.ElrcFormat;

public class ElrcTester
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
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\ELRC\\rick.elrc");
        
        string content = AFile.ReadFileToObject(fileInfo).ToStringData();

        AList<RichTimeStampedLyric> list = this._elrcParser.Parse(content);
        
        list.GetAsList().DumpConsole();
        Assert.AreEqual("Never gonna give you up", list.Get(0).Text);
    }
    
    [Test]
    public void TestFormatToFile()
    {
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\ELRC\\rick.elrc");

        string content = AFile.ReadFileToObject(fileInfo).ToStringData();
        
        AList<RichTimeStampedLyric> list = this._elrcParser.Parse(content);

        string formated = this._elrcParser.Revert(list);
        
        formated.DumpConsole();
        Assert.AreEqual(content, formated);
    }
}