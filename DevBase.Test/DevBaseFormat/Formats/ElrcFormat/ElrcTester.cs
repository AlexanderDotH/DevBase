using DevBase.Format;
using DevBase.Format.Formats.ElrcFormat;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.ElrcFormat;

/// <summary>
/// Tests for ELRC format parser.
/// </summary>
public class ElrcTester : FormatTest
{
    private ElrcParser _elrcParser;

    /// <summary>
    /// Sets up the ELRC parser.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._elrcParser = new ElrcParser();
    }

    /// <summary>
    /// Tests parsing ELRC format from file.
    /// </summary>
    [Test]
    public void TestFormatFromFile()
    {
        string content = AFile.ReadFileToObject(GetTestFile("ELRC", "rick.elrc")).ToStringData();

        AList<RichTimeStampedLyric> list = this._elrcParser.Parse(content);
        
        list.GetAsList().DumpConsole();
        Assert.That(list.Get(0).Text, Is.EqualTo("Never gonna give you up"));
    }
    
    /// <summary>
    /// Tests formatting back to ELRC format.
    /// </summary>
    [Test]
    public void TestFormatToFile()
    {
        string content = AFile.ReadFileToObject(GetTestFile("ELRC", "rick.elrc")).ToStringData();
        
        AList<RichTimeStampedLyric> list = this._elrcParser.Parse(content);

        string formatted = this._elrcParser.Revert(list);
        
        formatted.DumpConsole();
        Assert.That(formatted, Is.EqualTo(content));
    }

    [Test]
    public void TestParseIndentedContentTrimsLineWhitespace()
    {
        string content = @"
            [00:00:12.000] - [00:00:13.000] First line
            [
                [00:00:12.000] - [00:00:12.500] First
                [00:00:12.500] - [00:00:13.000] line
            ]
            ";

        AList<RichTimeStampedLyric> lyrics = this._elrcParser.Parse(content);

        Assert.That(lyrics.Get(0).Text, Is.EqualTo("First line"));
        Assert.That(lyrics.Get(0).Words.Length, Is.EqualTo(2));
    }
}
