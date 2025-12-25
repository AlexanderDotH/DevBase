using System.Text;
using DevBase.Format;
using DevBase.Format.Formats.RmmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Typography;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.RmmlFormat;

/// <summary>
/// Tests for RMML format parser.
/// </summary>
public class RmmlTester : FormatTest
{
    private FileParser<RmmlParser, AList<RichTimeStampedLyric>> _rmmlParser;

    /// <summary>
    /// Sets up the RMML parser.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._rmmlParser = new FileParser<RmmlParser, AList<RichTimeStampedLyric>>();
    }

    /// <summary>
    /// Tests parsing RMML format from file.
    /// </summary>
    [Test]
    public void TestFormatFromFile()
    {
        string content = File.ReadAllText(GetTestFile("RMML", "rick.rmml").FullName);
        
        AList<RichTimeStampedLyric> list = this._rmmlParser.ParseFromString(content);
        
        list.GetAsList().DumpConsole();
        
        Assert.That(content.Contains(list.Get(0).Text), Is.True);
    }
}
