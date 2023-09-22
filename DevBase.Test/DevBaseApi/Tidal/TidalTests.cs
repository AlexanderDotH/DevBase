using DevBase.Api.Apis.Tidal;

namespace DevBase.Test.DevBaseApi.Tidal;

public class TidalTests
{
    [Test]
    public async Task AuthTokenToAccess()
    {
        TidalClient client = new TidalClient();
        var token = await client.AuthTokenToAccess("MzZhZTYxNzMtN2RjYy00YzRmLWExMWEtN2IzYmUyMWE3Mjhl");
        
        Assert.AreEqual(token.clientName, "Android Automotive");
    }
}