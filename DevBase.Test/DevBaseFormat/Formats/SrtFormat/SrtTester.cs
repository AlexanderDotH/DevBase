using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;

namespace DevBase.Test.DevBaseFormat.Formats.SrtFormat;

public class SrtTester
{
    private FileFormatParser<AList<PreciseLyricElement>> _srtParser;

    [SetUp]
    public void Setup()
    {
        this._srtParser = new FileFormatParser<AList<PreciseLyricElement>>(new Format.Formats.SrtFormat.SrtFormat());
    }

    [Test]
    public void TestFormatFromFile()
    {
        AList<AFileObject> files =
            AFile.GetFiles("C:\\Users\\alexa\\RiderProjects\\DevBase\\DevBase.Test\\DevBaseFormatData\\SRT", true, "*.srt");

        AFileObject random = files.GetRandom();
        AList<string> content = random.ToList();

        AList<PreciseLyricElement> list = this._srtParser.FormatFromFile(random.FileInfo.FullName);

        Assert.AreEqual(content.Get(2), list.Get(0).Text);
    }
}