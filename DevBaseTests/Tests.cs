using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DevBaseFormat;
using DevBaseFormat.Formats.LrcFormat;
using DevBaseFormat.Structure;

namespace DevBaseTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestFileFormat()
        {
            FileFormatParser<LyricElement> fileFormatParser =
                new FileFormatParser<LyricElement>(new LrcParser<LyricElement>());
            fileFormatParser.FormatFromFile("C:\\Users\\Alex\\source\\repos\\EinfachEinAlex\\LyricsWPF\\LyricsWPF\\bin\\Debug\\lyrics-1651564640539.lrc").ForEach(t =>
            {
                Console.WriteLine(t.Line);
            });
        }
    }
}
