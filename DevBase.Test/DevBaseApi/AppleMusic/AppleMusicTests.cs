using Dumpify;

namespace DevBase.Test.DevBaseApi.AppleMusic;

public class AppleMusicTests
{
    [Test]
    public async Task SearchTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        var searchResults = await appleMusic.Search("Rich Astley");

        searchResults.DumpConsole();
        
        Assert.AreEqual("Whenever You Need Somebody", searchResults.SearchResults.SongResult.Songs[0].Attributes.AlbumName);
    }

    [Test]
    public async Task CreateObjectTest()
    {
        Api.Apis.AppleMusic.AppleMusic appleMusic = await Api.Apis.AppleMusic.AppleMusic.WithAccessToken();

        appleMusic.ApiToken.DumpConsole();
        
        Assert.NotNull(appleMusic.ApiToken);
    }
}