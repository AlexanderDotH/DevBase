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

/// <summary>
/// Tests for SRT format parser.
/// </summary>
public class SrtTester : FormatTest
{
    private FileParser<SrtParser, AList<RichTimeStampedLyric>> _srtParser;

    /// <summary>
    /// Sets up the SRT parser.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._srtParser = new FileParser<SrtParser, AList<RichTimeStampedLyric>>();
    }

    /// <summary>
    /// Tests parsing SRT format from a random file.
    /// </summary>
    [Test]
    public void TestFormatFromFile()
    {
        AList<AFileObject> files =
            AFile.GetFiles(GetTestFile("SRT", "").DirectoryName!, true, "*.srt");

        AFileObject random = files.GetRandom();

        AList<RichTimeStampedLyric> list = this._srtParser.ParseFromDisk(random.FileInfo);

        list.GetAsList().DumpConsole();
        
        Assert.That(list, Is.Not.Null);
        Assert.That(list.Length, Is.GreaterThan(0));
        Assert.That(list.Get(0).Text, Is.Not.Empty);
    }
}
