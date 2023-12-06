namespace DevBase.Test.DevBaseApi.NetEase;

public class NetEaseTest
{
    [Test]
    public async Task SearchTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();
        
        var result = await netEaseApi.Search("Rick Astley");
        
        Assert.NotNull(result.result);
    }
    
    [Test]
    public async Task RawLyricsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var result = await netEaseApi.RawLyrics("18520488");
        
        Assert.IsTrue(result.lrc.lyric.Contains("We're no strangers to love"));
    }
    
    [Test]
    public async Task LyricsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var result = await netEaseApi.Lyrics("18520488");
        
        Assert.IsTrue(result.Get(0).Line.Contains("We're no strangers to love"));
    }
    
    [Test]
    public async Task KaraokeLyricsTest()
    {
        Api.Apis.NetEase.NetEase netEaseApi = new Api.Apis.NetEase.NetEase();

        var result = await netEaseApi.KaraokeLyrics("18520488");
        
        Assert.IsTrue(result.Get(0).FullLine.Contains("Rick Astley"));
    }
}