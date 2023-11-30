using System.Text;
using DevBase.Extensions;
using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;

namespace DevBase.Test.DevBaseFormat.Formats.SrtFormat;

public class SrtTester
{
    private FileFormatParser<AList<PreciseLyricElement>> _srtParser;

    [SetUp]
    public void Setup()
    {
        this._srtParser = new FileFormatParser<AList<PreciseLyricElement>>(new Format.Formats.SrtFormat.SrtParser());
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

        AList<PreciseLyricElement> list = this._srtParser.FormatFromFile(random.FileInfo.FullName);

        Assert.AreEqual(content.Get(6), list.Get(0).Text);
    }
}