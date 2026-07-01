using DevBase.Api.Apis.Tidal;
using DevBase.Api.Apis.Tidal.Structure.Json;
using DevBase.IntegrationTests.Infrastructure;

namespace DevBase.IntegrationTests.Apis;

[Category("Integration")]
[Category("LiveApi")]
[NonParallelizable]
public sealed class TidalLiveIntegrationTests
{
    [Test]
    public async Task PublicTrackSearchReturnsSongData()
    {
        LiveApiTestSettings.RequireLiveApiTests();

        using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout();
        Tidal client = new() { StrictErrorHandling = true };

        try
        {
            JsonTidalSearchResult response = await client.Search("Rick Astley", "US", 5, timeout.Token);
            JsonTidalTrack first = response.Items.First();

            Assert.Multiple(() =>
            {
                Assert.That(response.Items, Is.Not.Empty);
                Assert.That(response.TotalNumberOfItems, Is.GreaterThan(0));
                Assert.That(first.Id, Is.GreaterThan(0));
                Assert.That(first.Title, Is.Not.Empty);
                Assert.That(first.Artist.Name, Is.Not.Empty);
            });
        }
        catch (DevBase.Api.Exceptions.TidalException exception)
        {
            LiveApiTestSettings.IgnoreProviderUnavailable("Tidal", exception);
        }
    }

    [Test]
    public async Task LyricsEndpointReturnsTextWhenAccessTokenIsConfigured()
    {
        LiveApiTestSettings.RequireLiveApiTests();
        string accessToken = LiveApiTestSettings.RequireEnvironmentValue("DEVBASE_TIDAL_ACCESS_TOKEN");

        using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout();
        Tidal client = new() { StrictErrorHandling = true };

        JsonTidalSearchResult search = await client.Search("Rick Astley", "US", 5, timeout.Token);
        JsonTidalTrack track = search.Items.First(candidate => candidate.Id > 0);
        JsonTidalLyricsResult lyrics = await client.GetLyrics(accessToken, track.Id.ToString(), "US", timeout.Token);

        Assert.Multiple(() =>
        {
            Assert.That(lyrics.trackId, Is.EqualTo(track.Id));
            Assert.That(lyrics.lyrics ?? lyrics.subtitles, Is.Not.Empty);
        });
    }
}
