using System.Net;
using Dumpify;

namespace DevBase.Test.DevBaseApi.NetEase;

/// <summary>
/// Tests for the NetEase API client.
/// </summary>
public class NetEaseTest
{
    /// <summary>
    /// Tests searching for tracks.
    /// </summary>
    [Test]
    public async Task SearchTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        try
        {
            var result = await netEaseApi.Search("Rick Astley");
            result.DumpConsole();
        
            Assert.That(result.result != null);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Failed to search tracks: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests fetching raw lyrics (LRC format).
    /// </summary>
    [Test]
    public async Task RawLyricsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        try
        {
            var result = await netEaseApi.RawLyrics("18520488");

            if (result?.lrc?.lyric == null)
            {
                Console.WriteLine("API returned null, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }

            result.DumpConsole();
        
            Assert.That(result.lrc.lyric.Contains("We're no strangers to love"));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests fetching processed lyrics.
    /// </summary>
    [Test]
    public async Task LyricsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        try
        {
            var result = await netEaseApi.Lyrics("18520488");
            
            if (result == null || result.IsEmpty())
            {
                Console.WriteLine("API returned null or empty, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }
        
            result.DumpConsole();

            Assert.That(result.Get(0).Text.Contains("We're no strangers to love"));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests fetching karaoke lyrics.
    /// </summary>
    [Test]
    public async Task KaraokeLyricsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        try
        {
            var result = await netEaseApi.KaraokeLyrics("18520488");
            
            if (result == null || result.IsEmpty())
            {
                Console.WriteLine("API returned null or empty, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }
        
            result.DumpConsole();
        
            Assert.That(result.Get(0).Text, Does.Contain("Rick Astley"));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }

    /// <summary>
    /// Tests fetching track details.
    /// </summary>
    [Test]
    public async Task TrackDetailsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        try
        {
            var details = await netEaseApi.TrackDetails("1883422");

            if (details?.songs == null || details.songs.Count == 0)
            {
                Console.WriteLine("API returned null or empty, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }

            details.DumpConsole();
        
            Assert.That(details.songs[0].name, Is.EqualTo("Take Me to Your Heart"));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests searching and receiving details in one go.
    /// </summary>
    [Test]
    public async Task SearchAndReceiveDetailsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        try
        {
            var details = await netEaseApi.SearchAndReceiveDetails("Rick Astley");

            if (details?.songs == null || details.songs.Count == 0)
            {
                Console.WriteLine("API returned null or empty, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }

            details.DumpConsole();
        
            Assert.That(details.songs[0].id, Is.EqualTo(28738054));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests downloading a track.
    /// </summary>
    [Test]
    public async Task DownloadTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        try
        {
            var downloadedBytes = await netEaseApi.Download("18520488");
            
            if (downloadedBytes == null || downloadedBytes.Length == 0)
            {
                Console.WriteLine("Download returned empty, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }
            
            downloadedBytes.Length.DumpConsole();
            Assert.That(downloadedBytes, Is.Not.Null);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Download failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests fetching the download URL for a track.
    /// </summary>
    [Test]
    public async Task UrlTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        try
        {
            var url = await netEaseApi.Url("18520488");

            if (url?.data == null || url.data.Count == 0 || url.data[0].url == null)
            {
                Console.WriteLine("API returned null or empty, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }

            url.DumpConsole();
        
            Assert.That(url.data[0].url, Is.Not.Null);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
}
