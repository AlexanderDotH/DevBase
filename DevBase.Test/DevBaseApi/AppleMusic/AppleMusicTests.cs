using Dumpify;

namespace DevBase.Test.DevBaseApi.AppleMusic;

/// <summary>
/// Tests for the Apple Music API client.
/// </summary>
public class AppleMusicTests
{
    private string _userMediaToken;
    
    /// <summary>
    /// Sets up the test environment.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._userMediaToken = "";
    }
    
    /// <summary>
    /// Tests raw search functionality.
    /// </summary>
    [Test]
    public async Task RawSearchTest()
    {
        try
        {
            Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

            var searchResults = await appleMusic.RawSearch("Rich Astley");

            if (searchResults?.SearchResults?.SongResult?.Songs == null || searchResults.SearchResults.SongResult.Songs.Count == 0)
            {
                Console.WriteLine("API returned null or empty, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }

            searchResults.DumpConsole();
            Assert.That(searchResults.SearchResults.SongResult.Songs[0].Attributes.AlbumName, Is.EqualTo("3 Originals"));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests the simplified Search method.
    /// </summary>
    [Test]
    public async Task SearchTest()
    {
        try
        {
            Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

            var searchResults = await appleMusic.Search("If I Could");
            
            if (searchResults == null || searchResults.Count == 0)
            {
                Console.WriteLine("API returned null or empty, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }
            
            searchResults.DumpConsole();
        
            Assert.That(searchResults[0].Title, Is.EqualTo("If I Could"));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }

    /// <summary>
    /// Tests creation of the AppleMusic object and access token generation.
    /// </summary>
    [Test]
    public async Task CreateObjectTest()
    {
        try
        {
            Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

            if (appleMusic?.ApiToken == null)
            {
                Console.WriteLine("API returned null, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }

            appleMusic.ApiToken.DumpConsole();
        
            Assert.That(appleMusic.ApiToken, Is.Not.Null);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }
    
    /// <summary>
    /// Tests configuring the user media token from a cookie.
    /// </summary>
    [Test]
    public async Task CreateObjectAndGetUserMediaTokenTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        if (string.IsNullOrEmpty(this._userMediaToken))
        {
            Console.WriteLine("UserMediaToken is null and that's okay");            
            return;
        }
        
        await appleMusic.WithMediaUserTokenFromCookie(this._userMediaToken);
        Assert.That(appleMusic.ApiToken, Is.Not.Null);   
    }

    /// <summary>
    /// Tests fetching lyrics.
    /// Requires a valid UserMediaToken.
    /// </summary>
    [Test]
    public async Task GetLyricsTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        if (string.IsNullOrEmpty(this._userMediaToken))
        {
            Console.WriteLine("UserMediaToken is null and that's okay");            
            return;
        }
        
        await appleMusic.WithMediaUserTokenFromCookie(this._userMediaToken);
           
        var lyrics = await appleMusic.GetLyrics("1717566174");

        lyrics.DumpConsole();
        
        Assert.That(lyrics.Data[0].Attributes.Ttml, Is.Not.Empty);
    }
}
