using System.Text;
using DevBase.Format;
using DevBase.Format.Extensions;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Formats.SrtFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;

namespace DevBase.IntegrationTests.Format;

[Category("Integration")]
[Category("Library")]
public sealed class FormatFileIntegrationTests
{
    [Test]
    public void LrcFileCanBeDiscoveredReadParsedAndConvertedToPlainText()
    {
        using TemporaryDirectory temporaryDirectory = TemporaryDirectory.Create();
        string filePath = Path.Combine(temporaryDirectory.Path, "sample.lrc");
        File.WriteAllText(
            filePath,
            "[00:01.00] First line\n[00:02.50] Second line\n",
            Encoding.UTF8);

        AList<AFileObject> files = AFile.GetFiles(temporaryDirectory.Path, readContent: true, filter: "*.lrc");
        AList<TimeStampedLyric> lyrics =
            new FileParser<LrcParser, AList<TimeStampedLyric>>().ParseFromDisk(files.Get(0).FileInfo);
        string plainText = lyrics.ToRawLyrics().ToPlainText();

        Assert.Multiple(() =>
        {
            Assert.That(files.Length, Is.EqualTo(1));
            Assert.That(files.Get(0).ToStringData(), Does.Contain("First line"));
            Assert.That(lyrics.Length, Is.EqualTo(2));
            Assert.That(lyrics.Get(0).Text, Is.EqualTo("First line"));
            Assert.That(lyrics.Get(1).StartTime, Is.EqualTo(TimeSpan.FromMilliseconds(2500)));
            Assert.That(plainText, Does.Contain("Second line"));
        });
    }

    [Test]
    public void SrtParserHandlesDiskContentWithCommaTimestamps()
    {
        using TemporaryDirectory temporaryDirectory = TemporaryDirectory.Create();
        string filePath = Path.Combine(temporaryDirectory.Path, "sample.srt");
        File.WriteAllText(
            filePath,
            "1\n00:00:01,000 --> 00:00:02,250\nFirst subtitle\n\n" +
            "2\n00:00:03,000 --> 00:00:05,000\nSecond subtitle\n\n",
            Encoding.UTF8);

        AList<RichTimeStampedLyric> subtitles =
            new FileParser<SrtParser, AList<RichTimeStampedLyric>>().ParseFromDisk(filePath);

        Assert.Multiple(() =>
        {
            Assert.That(subtitles.Length, Is.EqualTo(2));
            Assert.That(subtitles.Get(0).Text, Is.EqualTo("First subtitle"));
            Assert.That(subtitles.Get(0).StartTime, Is.EqualTo(TimeSpan.FromSeconds(1)));
            Assert.That(subtitles.Get(0).EndTime, Is.EqualTo(TimeSpan.FromMilliseconds(2250)));
            Assert.That(subtitles.ToPlainText(), Does.Contain("Second subtitle"));
        });
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        private TemporaryDirectory(string path)
        {
            this.Path = path;
        }

        public string Path { get; }

        public static TemporaryDirectory Create()
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"devbase-tests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(path);
            return new TemporaryDirectory(path);
        }

        public void Dispose()
        {
            if (Directory.Exists(this.Path)) {
                Directory.Delete(this.Path, recursive: true);
            }
        }
    }
}
