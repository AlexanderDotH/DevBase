using System.Runtime.InteropServices;
using DevBase.Format.Formats.RlrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.RlrcFormat;

/// <summary>
/// Tests for RLRC format parser.
/// </summary>
public class RlrcTester : FormatTest
{
    private RlrcParser _rlrcParser;

    /// <summary>
    /// Sets up the RLRC parser.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._rlrcParser = new RlrcParser();
    }

    /// <summary>
    /// Tests parsing RLRC format.
    /// </summary>
    [Test]
    public void TestToRlrc()
    {
        string content = AFile.ReadFileToObject(GetTestFile("RLRC", "RickAstley.rlrc")).ToStringData();

        AList<RawLyric> list = this._rlrcParser.Parse(content);
        
        list.GetAsList().DumpConsole();
        Assert.That(list.Get(0).Text, Is.EqualTo("Never gonna, never gonna, never gonna, never gonna"));
    }

    /// <summary>
    /// Tests formatting back to RLRC format.
    /// </summary>
    [Test]
    public void TestFromRlc()
    {
        string content = AFile.ReadFileToObject(GetTestFile("RLRC", "RickAstley.rlrc")).ToStringData();
        
        AList<RawLyric> list = this._rlrcParser.Parse(content);

        string formated = this._rlrcParser.Revert(list);

        // Just remove the \r\n at the end of the file
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            formated = formated.Substring(0, formated.Length - 2);
        }
        else
        {
            formated = formated.Substring(0, formated.Length - 1);
        }
        
        formated.DumpConsole();
        Assert.That(formated, Is.EqualTo(content));
    }
}
