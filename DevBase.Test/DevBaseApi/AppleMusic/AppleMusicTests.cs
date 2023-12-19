using DevBase.Api.Objects.Token;
using Dumpify;

namespace DevBase.Test.DevBaseApi.AppleMusic;

public class AppleMusicTests
{
    [Test]
    public async Task RawSearchTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        var searchResults = await appleMusic.RawSearch("Rich Astley");

        searchResults.DumpConsole();
        
        Assert.AreEqual("Whenever You Need Somebody", searchResults.SearchResults.SongResult.Songs[0].Attributes.AlbumName);
    }
    
    [Test]
    public async Task SearchTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        var searchResults = await appleMusic.Search("If I Could");

        searchResults.DumpConsole();
        
       // Assert.AreEqual("Never Gonna Give You Up", searchResults[0].Title);
    }

    [Test]
    public async Task CreateObjectTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        appleMusic.ApiToken.DumpConsole();
        
        Assert.NotNull(appleMusic.ApiToken);
    }
    
    [Test]
    public async Task CreateObjectAndGetUserMediaTokenTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        await appleMusic.WithMediaUserTokenFromCookie("");

        Assert.NotNull(appleMusic.ApiToken);
    }

    [Test]
    public async Task GetLyricsTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();
        
        var lyrics = await appleMusic.GetLyrics("1717566174");

        lyrics.DumpConsole();
        
        Assert.IsNotEmpty(lyrics.Data[0].Attributes.Ttml);
    }
}