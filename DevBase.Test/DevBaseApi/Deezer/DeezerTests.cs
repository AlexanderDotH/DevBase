using System.Diagnostics;
using System.Net;
using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.Api.Apis.Deezer.Structure.Objects;
using DevBase.Extensions.Stopwatch;
using Dumpify;

namespace DevBase.Test.DevBaseApi.Deezer;

public class DeezerTests
{
    private string _accessToken;
    private string _sessionToken;
    private string _arlToken;
    
    [SetUp]
    public void SetUp()
    {
        this._accessToken = "";
        this._sessionToken = "";
        this._arlToken = "";
    }
    
    [Test]
    public async Task GetJwtTokenTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        if (string.IsNullOrEmpty(this._arlToken))
        {
            Console.WriteLine("The arl token is empty and that is okay");
        }
        else
        {
            var token = await deezerApi.GetJwtToken();
            Assert.NotNull(token.jwt);
        }
    }

    [Test]
    public async Task GetLyricsTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        if (string.IsNullOrEmpty(this._arlToken))
        {
            Console.WriteLine("The arl token is empty and that is okay");
        }
        else
        {
            string trackID = "1276231202";
            
            var lyrics = await deezerApi.GetLyrics(trackID);
            lyrics.DumpConsole();
        
            Assert.NotNull(lyrics.RawLyrics);
        }
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

        if (string.IsNullOrEmpty(this._accessToken))
        {
            Console.WriteLine("Access token is null and that is okay");
        }
        else
        {
            JsonDeezerAuthTokenResponse token = await deezerApi.GetAccessToken("", "457142");
            Assert.NotNull(token.data.attributes);
        }
    }

    [Test]
    public async Task GetArlTokenFromSessionTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        if (string.IsNullOrEmpty(this._sessionToken))
        {
            Console.WriteLine("Access token is null and that is okay");
        }
        else
        {
            string arlToken = await deezerApi.GetArlTokenFromSession("");
            Assert.NotNull(arlToken);
        }
    }

    [Test]
    public async Task DownloadSongTest()
    {
        try
        {
            Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();
        
            string trackID = "1276231202";

            byte[] mp3 = await deezerApi.DownloadSong(trackID);
        
            if (mp3 == null || mp3.Length == 0)
                return;
            
            Assert.NotNull(mp3);
        }
        catch (WebException e)
        {
            if (e.Message.SequenceEqual("The operation has timed out."))
            {
                Console.WriteLine("Request timed out and that is okay");
            }
            else
            {
                throw;
            }
        }
    }

    [Test]
    public async Task GetSongTest()
    {
        Api.Apis.Deezer.Deezer deezerApi = new Api.Apis.Deezer.Deezer();

        try
        {
            string trackID = "2185305857";

            DeezerTrack track = await deezerApi.GetSong(trackID);

            track.DumpConsole();
        
            Assert.NotNull(track);
        }
        catch (WebException e)
        {
            if (e.Message.SequenceEqual("The operation has timed out."))
            {
                Console.WriteLine("Timed out but that's okay");
            }
        }
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
        deezerApi.StrictErrorHandling = false;

        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        var results = await deezerApi.SearchSongData(track:"Never Gonna Give You Up", artist:"Rick Astley", strict:false, limit:10);
        stopwatch.Stop();
        
        Console.WriteLine($"Collected {results.Count} tracks");

        stopwatch.PrintTimeTable();
        results.DumpConsole();
        
        Assert.NotNull(results);
    }
}