using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseFormat.Formats.LrcFormat
{
    public class LrcTester
    {
        private FileParser<LrcParser, AList<TimeStampedLyric>> _lrcParser;

        [SetUp]
        public void Setup()
        {
            this._lrcParser = new FileParser<LrcParser, AList<TimeStampedLyric>>();
        }

        [Test]
        public void TestFormatFromFile()
        {
            AList<TimeStampedLyric> parsed = this._lrcParser.ParseFromDisk("..\\..\\..\\DevBaseFormatData\\LRC\\Circles.lrc");

            parsed.DumpConsole();
            
            Assert.AreEqual(parsed.Get(0).Text, "Lets make circles");
        }
    }
}
