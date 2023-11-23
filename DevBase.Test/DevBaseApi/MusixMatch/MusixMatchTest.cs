using Dumpify;

namespace DevBase.Test.DevBaseApi.MusixMatch;

public class MusixMatchTest
{
    [Test]
    public async Task LoginTest()
    {
        Api.Apis.Musixmatch.MusixMatch musixMatch = new Api.Apis.Musixmatch.MusixMatch();
        var auth = await musixMatch.Login("", "");

        Assert.NotNull(auth.message.body.tokens.mxmprowebv10);
    }
}