
namespace DevBase.Test.DevBaseApi.OpenLyricsClient;

/// <summary>
/// Tests for OpenLyricsClient token refreshing.
/// </summary>
public class RefreshTokenTest
{
    /// <summary>
    /// Tests the retrieval of a new access token.
    /// Currently commented out as it seems to be manual/debug code.
    /// </summary>
    [Test]
    public void GetNewAccessToken()
    {
        /*Task.Factory.StartNew(async () =>
        {
            global::DevBase.Api.Apis.OpenLyricsClient.OpenLyricsClient api =
                new global::DevBase.Api.Apis.OpenLyricsClient.OpenLyricsClient();
            JsonOpenLyricsClientAccess access = await api.GetAccessToken("not today");
            Assert.That("Bearer", Is.EqualTo(access.TokenType));
        });*/
    }
}
