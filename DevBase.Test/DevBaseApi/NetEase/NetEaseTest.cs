using Dumpify;

namespace DevBase.Test.DevBaseApi.NetEase;

public class NetEaseTest
{
    [Test]
    public async Task SearchTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();
        
        var result = await netEaseApi.Search("Rick Astley");

        result.DumpConsole();
        
        Assert.That(result.result != null);
    }
    
    [Test]
    public async Task RawLyricsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var result = await netEaseApi.RawLyrics("18520488");

        result.DumpConsole();
        
        Assert.That(result.lrc.lyric.Contains("We're no strangers to love"));
    }
    
    [Test]
    public async Task LyricsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var result = await netEaseApi.Lyrics("18520488");
        
        result.DumpConsole();

        Assert.That(result.Get(0).Text.Contains("We're no strangers to love"));
    }
    
    [Test]
    public async Task KaraokeLyricsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var result = await netEaseApi.KaraokeLyrics("18520488");
        
        result.DumpConsole();
        
        Assert.IsTrue(result.Get(0).Text.Contains("Rick Astley"));
    }

    [Test]
    public async Task TrackDetailsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var details = await netEaseApi.TrackDetails("1883422");

        details.DumpConsole();
        
        Assert.AreEqual("Take Me to Your Heart", details.songs[0].name);
    }
    
    [Test]
    public async Task SearchAndReceiveDetailsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var details = await netEaseApi.SearchAndReceiveDetails("Rick Astley");

        details.DumpConsole();
        
        Assert.AreEqual(28738054, details.songs[0].id);
    }
    
    [Test]
    public async Task DownloadTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var downloadedBytes = await netEaseApi.Download("18520488");

        downloadedBytes.Length.DumpConsole();
        
        Assert.NotNull(downloadedBytes);
    }
    
    [Test]
    public async Task UrlTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var url = await netEaseApi.Url("18520488");

        url.DumpConsole();
        
        Assert.NotNull(url.data[0].url);
    }
}