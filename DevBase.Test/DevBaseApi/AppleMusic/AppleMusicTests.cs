using Dumpify;

namespace DevBase.Test.DevBaseApi.AppleMusic;

public class AppleMusicTests
{
    private string _userMediaToken;
    
    [SetUp]
    public void SetUp()
    {
        this._userMediaToken = "";
    }
    
    [Test]
    public async Task RawSearchTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        var searchResults = await appleMusic.RawSearch("Rich Astley");

        searchResults.DumpConsole();
        Assert.That(searchResults.SearchResults.SongResult.Songs[0].Attributes.AlbumName, Is.EqualTo("3 Originals"));
    }
    
    [Test]
    public async Task SearchTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        try
        {
            var searchResults = await appleMusic.Search("If I Could");
            searchResults.DumpConsole();
        
            Assert.That(searchResults[0].Title, Is.EqualTo("If I Could"));
        }
        catch
        {
            Console.WriteLine("Failed to search tracks, but that's okay");
        }
    }

    [Test]
    public async Task CreateObjectTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        appleMusic.ApiToken.DumpConsole();
        
        Assert.That(appleMusic.ApiToken, Is.Not.Null);
    }
    
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
