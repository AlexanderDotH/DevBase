using System.Text;
using DevBase.Extensions;
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Formats.SrtFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.SrtFormat;

public class SrtTester
{
    private FileParser<SrtParser, AList<RichTimeStampedLyric>> _srtParser;

    [SetUp]
    public void Setup()
    {
        this._srtParser = new FileParser<SrtParser, AList<RichTimeStampedLyric>>();
    }

    [Test]
    public void TestFormatFromFile()
    {
        AList<AFileObject> files =
            AFile.GetFiles("..\\..\\..\\DevBaseFormatData\\SRT", true, "*.srt");

        AFileObject random = files.GetRandom();
        string file = random.ToStringData().Replace("\n", Environment.NewLine);

        StringBuilder sb = new StringBuilder();
        foreach (var s in file.Split('\n'))
        {
            sb.AppendLine(s);
        }
        
        AList<string> content = new AString(sb.ToString()).AsList();

        AList<RichTimeStampedLyric> list = this._srtParser.ParseFromDisk(random.FileInfo);

        list.GetAsList().DumpConsole();
        
        Assert.AreEqual(content.Get(6), list.Get(0).Text);
    }
}