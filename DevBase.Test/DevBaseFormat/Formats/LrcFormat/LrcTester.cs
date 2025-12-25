using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.LrcFormat
{
/// <summary>
/// Tests for LRC format parser.
/// </summary>
public class LrcTester : FormatTest
{
    private FileParser<LrcParser, AList<TimeStampedLyric>> _lrcParser;

    /// <summary>
    /// Sets up the LRC parser.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._lrcParser = new FileParser<LrcParser, AList<TimeStampedLyric>>();
    }

    /// <summary>
    /// Tests parsing LRC format from file.
    /// </summary>
    [Test]
    public void TestFormatFromFile()
    {
        AList<TimeStampedLyric> parsed = this._lrcParser.ParseFromDisk(GetTestFile("LRC", "Circles.lrc"));

        parsed.DumpConsole();
        Assert.That(parsed.Get(0).Text, Is.EqualTo("Lets make circles"));
    }
}
}
