using DevBase.Format;
using DevBase.Format.Formats.SrtFormat;
using DevBase.Format.Structure;
using DevBase.Generics;

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
        AList<RichTimeStampedLyric> list = this._srtParser.ParseFromDisk(GetTestFile("SRT", "Rick.srt"));
        
        Assert.That(list, Is.Not.Null);
        Assert.That(list.Length, Is.GreaterThan(0));
        Assert.Multiple(() =>
        {
            Assert.That(list.Get(0).Text, Is.EqualTo("🎵"));
            Assert.That(list.Get(0).StartTime, Is.EqualTo(TimeSpan.Zero));
            Assert.That(list.Get(0).EndTime, Is.EqualTo(TimeSpan.FromMilliseconds(18340)));
        });
    }
}
