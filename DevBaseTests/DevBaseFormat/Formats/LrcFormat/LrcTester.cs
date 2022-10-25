using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBaseFormat;
using DevBaseFormat.Formats.LrcFormat;
using DevBaseFormat.Structure;

namespace DevBaseTests.DevBaseFormat.Formats.LrcFormat
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
            LrcObject parsed = this._lrcParser.FormatFromFile("..\\..\\..\\DevBaseFormatData\\LRC\\BEST LIFE.lrc");
            Assert.AreEqual(parsed.Lyrics.Get(0).Line, "Best life");
        }
    }
}
