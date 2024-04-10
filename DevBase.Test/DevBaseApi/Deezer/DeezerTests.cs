using System.Diagnostics;
using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.Api.Apis.Deezer.Structure.Objects;
using Dumpify;

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

        lyrics.DumpConsole();
        
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
        
        string trackID = "2185305857";

        DeezerTrack track = await deezerApi.GetSong(trackID);

        track.DumpConsole();
        
        Assert.NotNull(track);
    }

    [Test]
    public async Task SearchTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        var results = await deezerApi.Search(track:"Contact", artist:"Abide", album:"Contact", strict:false);
        stopwatch.Stop();
        
        Console.WriteLine($"Took {stopwatch.ElapsedMilliseconds}ms or {stopwatch.ElapsedTicks}ts to collect {results.data.Count} tracks");

        results.DumpConsole();
        
        Assert.NotNull(results);
    }

    [Test]
    public async Task SyncSearchTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        var results = await deezerApi.SearchSongData(track:"Never Gonna Give You Up", artist:"Rick Astley", strict:false, limit:10);
        stopwatch.Stop();
        
        Console.WriteLine($"Took {stopwatch.ElapsedMilliseconds}ms or {stopwatch.ElapsedTicks}ts to collect {results.Count} tracks");
        
        results.DumpConsole();
        
        Assert.NotNull(results);
    }
}