using DevBase.Api.Apis.Deezer;
using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.IntegrationTests.Infrastructure;

namespace DevBase.IntegrationTests.Apis;

[Category("Integration")]
[Category("LiveApi")]
[NonParallelizable]
public sealed class DeezerLiveIntegrationTests
{
    [Test]
    public async Task PublicSearchReturnsSongData()
    {
        LiveApiTestSettings.RequireLiveApiTests();

        using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout();
        Deezer client = new() { StrictErrorHandling = true };

        JsonDeezerSearchResponse response = await client.Search("Rick%20Astley", timeout.Token);
        JsonDeezerSearchDataResponse first = response.data.First();

        Assert.Multiple(() =>
        {
            Assert.That(response.data, Is.Not.Empty);
            Assert.That(response.total, Is.GreaterThan(0));
            Assert.That(first.id, Is.GreaterThan(0));
            Assert.That(first.title, Is.Not.Empty);
            Assert.That(first.artist.name, Is.Not.Empty);
        });
    }

    [Test]
    public async Task PublicAccessTokenEndpointReturnsUnloggedToken()
    {
        LiveApiTestSettings.RequireLiveApiTests();

        using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout();
        Deezer client = new() { StrictErrorHandling = true };

        JsonDeezerAuthTokenResponse token = await client.GetAccessToken(cancellationToken: timeout.Token);
        JsonDeezerAuthTokenResponseData? tokenData = token.data;
        JsonDeezerAuthTokenResponseAttributes? tokenAttributes = tokenData?.attributes;

        if (tokenData is null || tokenAttributes is null) {
            LiveApiTestSettings.IgnoreProviderUnavailable("Deezer", "unlogged token response did not include token data");
        }

        Assert.Multiple(() =>
        {
            Assert.That(tokenData!.id, Is.Not.Empty);
            Assert.That(tokenAttributes!.token, Is.Not.Empty);
        });
    }

    [Test]
    public async Task JwtEndpointReturnsTokenWhenArlIsConfigured()
    {
        LiveApiTestSettings.RequireLiveApiTests();
        string arl = LiveApiTestSettings.RequireEnvironmentValue("DEVBASE_DEEZER_ARL");

        using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout();
        Deezer client = new(arl) { StrictErrorHandling = true };

        JsonDeezerJwtToken token = await client.GetJwtToken(timeout.Token);

        Assert.Multiple(() =>
        {
            Assert.That(token.jwt, Is.Not.Empty);
            Assert.That(token.refresh_token, Is.Not.Empty);
        });
    }
}
