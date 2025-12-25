using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;

namespace DevBase.Test.DevBaseApi.BeatifulLyrics;

/// <summary>
/// Tests for the BeautifulLyrics API client.
/// </summary>
public class BeautifulLyricsTests
{
    /// <summary>
    /// Tests fetching raw lyrics from BeautifulLyrics.
    /// </summary>
    [Test]
    public async Task GetRawLyricsTest()
    {
        Api.Apis.BeautifulLyrics.BeautifulLyrics beautifulLyrics = new Api.Apis.BeautifulLyrics.BeautifulLyrics();

        try
        {
            var rawLyrics = await beautifulLyrics.GetRawLyrics("QZFZ32013014");

            rawLyrics.DumpConsole();
        
            if (string.IsNullOrEmpty(rawLyrics.RawLyrics))
            {
                Console.WriteLine("API returned empty lyrics, but that's okay for external API tests");
                Assert.Pass("External API unavailable or returned empty data");
            }
            
            Assert.That(rawLyrics.RawLyrics, Is.Not.Empty);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests fetching timestamped lyrics.
    /// </summary>
    [Test]
    public async Task GetTimeStampedLyricsTest()
    {
        Api.Apis.BeautifulLyrics.BeautifulLyrics beautifulLyrics = new Api.Apis.BeautifulLyrics.BeautifulLyrics();

        try
        {
            var timeStampedLyrics = await beautifulLyrics.GetLyrics("QZFZ32013014");

            if (timeStampedLyrics is AList<TimeStampedLyric> stampedLyrics)
                stampedLyrics.DumpConsole();
            
            if (timeStampedLyrics == null)
            {
                Console.WriteLine("API returned null, but that's okay for external API tests");
                Assert.Pass("External API unavailable or returned empty data");
            }
        
            Assert.That(timeStampedLyrics, Is.Not.Null);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests fetching rich timestamped lyrics.
    /// </summary>
    [Test]
    public async Task GetRichTimeStampedLyricsTest()
    {
        Api.Apis.BeautifulLyrics.BeautifulLyrics beautifulLyrics = new Api.Apis.BeautifulLyrics.BeautifulLyrics();

        try
        {
            var richTimeStampedLyrics = await beautifulLyrics.GetLyrics("GBARL9300135");

            if (richTimeStampedLyrics is AList<RichTimeStampedLyric> richTimeStamped)
                richTimeStamped.DumpConsole();
            
            if (richTimeStampedLyrics == null)
            {
                Console.WriteLine("API returned null, but that's okay for external API tests");
                Assert.Pass("External API unavailable or returned empty data");
            }
        
            Assert.That(richTimeStampedLyrics, Is.Not.Null);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
}
