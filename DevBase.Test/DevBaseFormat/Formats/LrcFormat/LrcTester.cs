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

        [Test]
        public void TestExampleCode()
        {
            // Parse from string
            string lrcContent = @"
                [00:12.00]First line
                [00:15.50]Second line
                [00:20.00]Third line
                ";

            var parser = new FileParser<LrcParser, AList<TimeStampedLyric>>();
            var lyrics = parser.ParseFromString(lrcContent);

            foreach (var line in lyrics)
            {
                Console.WriteLine($"[{line.StartTime}] {line.Text}");
            }

            // Pre-check if we can extract data
            Assert.That(lyrics, Is.Not.Empty);
            Assert.That(lyrics.Get(0), Is.Not.Null);

            TimeStampedLyric testCandidate = lyrics.Get(0);

            // Check if time data is correct
            Assert.That(testCandidate.StartTime.Milliseconds, Is.EqualTo(00));
            Assert.That(testCandidate.StartTime.Seconds, Is.EqualTo(12));
            Assert.That(testCandidate.StartTime.Minutes, Is.EqualTo(00));

            // Check if text data is correct
            Assert.That(testCandidate.Text, Is.EqualTo("First line"));

            lyrics.DumpConsole();
        }
    }
}
