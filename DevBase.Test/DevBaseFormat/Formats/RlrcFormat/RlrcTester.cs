using DevBase.Format.Formats.RlrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.RlrcFormat;

public class RlrcTester
{
    private RlrcParser _rlrcParser;

    [SetUp]
    public void Setup()
    {
        this._rlrcParser = new RlrcParser();
    }

    [Test]
    public void TestToRlrc()
    {
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\RLRC\\RickAstley.rlrc");
        
        string content = AFile.ReadFile(fileInfo).ToStringData();

        AList<RawLyric> list = this._rlrcParser.Parse(content);
        
        list.GetAsList().DumpConsole();
        Assert.AreEqual(list.Get(0).Text, "Never gonna, never gonna, never gonna, never gonna");
    }

    [Test]
    public void TestFromRlc()
    {
        FileInfo fileInfo =
            new FileInfo($"..\\..\\..\\DevBaseFormatData\\RLRC\\RickAstley.rlrc");

        string content = AFile.ReadFile(fileInfo).ToStringData();
        
        AList<RawLyric> list = this._rlrcParser.Parse(content);

        string formated = this._rlrcParser.Revert(list);

        // Just remove the \r\n at the end of the file
        formated = formated.Substring(0, formated.Length - 2);
        
        formated.DumpConsole();
        Assert.AreEqual(content, formated);
    }
}