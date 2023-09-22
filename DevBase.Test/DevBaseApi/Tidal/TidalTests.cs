using DevBase.Api.Apis.Tidal;

namespace DevBase.Test.DevBaseApi.Tidal;

public class TidalTests
{
    [Test]
    public async Task AuthTokenToAccess()
    {
        TidalClient client = new TidalClient();
        var token = await client.AuthTokenToAccess("");
        
        Assert.AreEqual(token.clientName, "Android Automotive");
    }
}