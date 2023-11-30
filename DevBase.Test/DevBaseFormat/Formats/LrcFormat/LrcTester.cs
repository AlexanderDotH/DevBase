using DevBase.Format;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;

namespace DevBase.Test.DevBaseFormat.Formats.LrcFormat
{
    public class LrcTester
    {
        private FileFormatParser<LrcObject> _lrcParser;

        [SetUp]
        public void Setup()
        {
            this._lrcParser = new FileFormatParser<LrcObject>(new LrcParser<LrcObject>());
        }

        [Test]
        public void TestFormatFromFile()
        {
            LrcObject parsed = this._lrcParser.FormatFromFile("..\\..\\..\\DevBaseFormatData\\LRC\\Circles.lrc");
            Assert.AreEqual(parsed.Lyrics.Get(0).Line, "Lets make circles");
        }
    }
}
