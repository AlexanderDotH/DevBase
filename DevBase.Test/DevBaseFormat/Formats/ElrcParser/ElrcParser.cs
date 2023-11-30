using DevBase.Format;
using DevBase.Format.Formats.RmmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.ElrcParser;

public class ElrcParser
{
    private FileFormatParser<AList<RichLyrics>> _elrcParser;

    [SetUp]
    public void Setup()
    {
        this._elrcParser = new FileFormatParser<AList<RichLyrics>>(new Format.Formats.ElrcFormat.ElrcParser());
    }

    [Test]
    public void TestFormatFromFile()
    {
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\ELRC\\rich.elrc");
        
        AList<RichLyrics> list = this._elrcParser.FormatFromFile(fileInfo.FullName);
        
        list.GetAsList().DumpConsole();
        Assert.AreEqual(list.Get(0).FullLine, "Never gonna give you up");
    }
    
    [Test]
    public void TestFormatToFile()
    {
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\ELRC\\rich.elrc");

        string content = AFile.ReadFile(fileInfo).ToStringData();
        
        AList<RichLyrics> list = this._elrcParser.FormatFromString(content);

        string formated = this._elrcParser.FormatToString(list);
        
        formated.DumpConsole();
        Assert.AreEqual(content, formated);
    }
}