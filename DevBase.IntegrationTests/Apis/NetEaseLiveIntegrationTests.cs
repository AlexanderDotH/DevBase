using DevBase.Api.Apis.NetEase;
using DevBase.Api.Apis.NetEase.Structure.Json;
using DevBase.IntegrationTests.Infrastructure;

namespace DevBase.IntegrationTests.Apis;

[Category("Integration")]
[Category("LiveApi")]
[NonParallelizable]
public sealed class NetEaseLiveIntegrationTests
{
    [Test]
    public async Task SearchDetailsAndLyricsEndpointsReturnSongData()
    {
        LiveApiTestSettings.RequireLiveApiTests();

        using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout(TimeSpan.FromSeconds(60));
        NetEase client = new() { StrictErrorHandling = true };

        JsonNetEaseSearchResult search = await client.Search("Rick Astley", limit: 5, cancellationToken: timeout.Token);
        JsonNetEaseSearchResultEntrySong song = search.result.songs.First();
        JsonNetEaseDetailResponse details = await client.TrackDetails(
            new[] { song.id.ToString() },
            timeout.Token);
        JsonNetEaseLyricResponse lyrics = await client.RawLyrics(song.id.ToString(), timeout.Token);

        if (details.code != 200) {
            LiveApiTestSettings.IgnoreProviderUnavailable("NetEase", $"details endpoint returned code {details.code}");
        }

        if (lyrics.code != 200) {
            LiveApiTestSettings.IgnoreProviderUnavailable("NetEase", $"lyrics endpoint returned code {lyrics.code}");
        }

        Assert.Multiple(() =>
        {
            Assert.That(search.code, Is.EqualTo(200));
            Assert.That(search.result.songs, Is.Not.Empty);
            Assert.That(song.id, Is.GreaterThan(0));
            Assert.That(song.name, Is.Not.Empty);
            Assert.That(details.code, Is.EqualTo(200));
            Assert.That(details.songs, Is.Not.Empty);
            Assert.That(details.songs.First().id, Is.EqualTo(song.id));
            Assert.That(lyrics.code, Is.EqualTo(200));
        });
    }
}
