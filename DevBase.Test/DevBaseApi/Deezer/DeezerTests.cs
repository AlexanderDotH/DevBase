using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.Api.Apis.Deezer.Structure.Objects;

namespace DevBase.Test.DevBaseApi.Deezer;

public class DeezerTests
{
    [Test]
    public async Task GetJwtTokenTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();
        
        var token = await deezerApi.GetJwtToken();
        Assert.NotNull(token.jwt);
    }

    [Test]
    public async Task GetLyricsTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        string trackID = "1276231202";

        var lyrics = await deezerApi.GetLyrics(trackID);
        
        Assert.NotNull(lyrics.RawLyrics);
    }

    [Test]
    public async Task GetAccessTokenTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        JsonDeezerAuthTokenResponse token = await deezerApi.GetAccessToken();
        
        Assert.NotNull(token.data.attributes);
    } 
    
    [Test]
    public async Task GetAccessTokenFromSessionTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        JsonDeezerAuthTokenResponse token = await deezerApi.GetAccessToken("", "457142");
        
        Assert.NotNull(token.data.attributes);
    }

    [Test]
    public async Task GetArlTokenFromSessionTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        string arlToken = await deezerApi.GetArlTokenFromSession("");
        
        Assert.NotNull(arlToken);
    }

    [Test]
    public async Task DownloadSong()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();
        
        string trackID = "1276231202";

        byte[] mp3 = await deezerApi.DownloadSong(trackID);
        
        Assert.NotNull(mp3);
    }

    [Test]
    public async Task GetSongTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();
        
        string trackID = "1276231202";

        DeezerTrack track = await deezerApi.GetSong(trackID);
        
        Assert.NotNull(track);
    }

    [Test]
    public async Task SearchTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        var results = await deezerApi.Search(track:"Never Gonna Give You Up", artist:"Rick Astley", strict:true);
        
        Assert.NotNull(results);
    }
}