using System.Diagnostics;
using DevBaseApi.Apis.OpenLyricsClient.Structure.Json;

namespace DevBaseTests.DevBaseApi.OpenLyricsClient;

public class RefreshTokenTest
{
    [Test]
    public void GetNewAccessToken()
    {
        Task.Factory.StartNew(async () =>
        {
            global::DevBaseApi.Apis.OpenLyricsClient.OpenLyricsClient api =
                new global::DevBaseApi.Apis.OpenLyricsClient.OpenLyricsClient();
            JsonOpenLyricsClientAccess access = await api.GetAccessToken("not today");
            Assert.AreEqual(access.TokenType, "Bearer");
        });
    }
}