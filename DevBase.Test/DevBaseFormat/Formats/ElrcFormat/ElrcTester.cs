using DevBase.Format;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.ElrcFormat;

public class ElrcTester
{
    private FileFormatParser<AList<RichTimeStampedLyric>> _elrcParser;

    [SetUp]
    public void Setup()
    {
        this._elrcParser = new FileFormatParser<AList<RichTimeStampedLyric>>(new Format.Formats.ElrcFormat.ElrcParser());
    }

    [Test]
    public void TestFormatFromFile()
    {
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\ELRC\\rick.elrc");
        
        AList<RichTimeStampedLyric> list = this._elrcParser.FormatFromFile(fileInfo.FullName);
        
        list.GetAsList().DumpConsole();
        Assert.AreEqual(list.Get(0).Text, "Never gonna give you up");
    }
    
    [Test]
    public void TestFormatToFile()
    {
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\ELRC\\rick.elrc");

        string content = AFile.ReadFile(fileInfo).ToStringData();
        
        AList<RichTimeStampedLyric> list = this._elrcParser.FormatFromString(content);

        string formated = this._elrcParser.FormatToString(list);
        
        formated.DumpConsole();
        Assert.AreEqual(content, formated);
    }
}