using DevBase.Api.Apis.Musixmatch;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using DevBase.IntegrationTests.Infrastructure;

namespace DevBase.IntegrationTests.Apis;

[Category("Integration")]
[Category("LiveApi")]
[NonParallelizable]
public sealed class MusixMatchLiveIntegrationTests
{
    [Test]
    public async Task SearchTrackAndFetchLyricsSyncAndMetadata()
    {
        LiveApiTestSettings.RequireLiveApiTests();

        try
        {
            using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout(TimeSpan.FromSeconds(90));
            MusixMatch client = CreateClient();

            List<Track> tracks = await client.SongSearchAsync(
                new TrackSearchParameters
                {
                    Artist = "Rick Astley",
                    Title = "Never Gonna Give You Up",
                    HasLyrics = true,
                    HasSubtitles = true,
                    HasRichSync = true
                },
                new PaginationParameters { PageSize = 5 },
                timeout.Token);

            Track track = tracks.FirstOrDefault(HasReadableSyncData)
                ?? tracks.FirstOrDefault(candidate => candidate.TrackId.HasValue)
                ?? throw new AssertionException("Musixmatch search did not return a track id.");

            int trackId = track.TrackId!.Value;
            Track details = await client.GetTrackByIdAsync(trackId, timeout.Token);
            Lyrics lyrics = await client.GetTrackLyricsAsync(trackId, timeout.Token);
            string snippet = await client.GetTrackSnippetAsync(trackId, timeout.Token);
            List<MusicGenre> genres = await client.GetMusicGenresAsync(timeout.Token);

            Assert.Multiple(() =>
            {
                Assert.That(client.Token.Token, Is.Not.Empty);
                Assert.That(tracks, Is.Not.Empty);
                Assert.That(details.TrackId, Is.EqualTo(trackId));
                Assert.That(details.TrackName, Is.Not.Empty);
                Assert.That(details.ArtistName, Is.Not.Empty);
                Assert.That(lyrics.LyricsBody, Is.Not.Empty);
                Assert.That(snippet, Is.Not.Null);
                Assert.That(genres, Is.Not.Empty);
            });

            if (track.HasSubtitles == 1) {
                Subtitles subtitles = await client.GetTrackSubtitlesAsync(trackId, timeout.Token);
                Assert.That(subtitles.Lines, Is.Not.Empty);
            }

            if (track.HasRichsync == 1) {
                Richsync richsync = await client.GetTrackRichsyncAsync(trackId, timeout.Token);
                Assert.That(richsync.Lines, Is.Not.Empty);
            }
        }
        catch (MusixMatchRequestException exception) when (CanSkipProviderStatus(exception))
        {
            LiveApiTestSettings.IgnoreProviderUnavailable("Musixmatch", exception);
        }
    }

    [Test]
    public async Task CatalogEndpointsReturnChartAndArtistData()
    {
        LiveApiTestSettings.RequireLiveApiTests();

        try
        {
            using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout(TimeSpan.FromSeconds(60));
            MusixMatch client = CreateClient();
            PaginationParameters page = new() { PageSize = 5 };

            List<string> chartTracks = await client.ChartTracksGetAsync(page, timeout.Token);
            List<Artist> chartArtists = await client.GetChartArtistsAsync("US", page, timeout.Token);
            List<Artist> artistSearch = await client.ArtistSearchAsync("Rick Astley", page, timeout.Token);

            if (chartTracks.Count == 0) {
                LiveApiTestSettings.IgnoreProviderUnavailable("Musixmatch", "chart tracks endpoint returned no tracks");
            }

            if (chartArtists.Count == 0) {
                LiveApiTestSettings.IgnoreProviderUnavailable("Musixmatch", "chart artists endpoint returned no artists");
            }

            if (artistSearch.Count == 0) {
                LiveApiTestSettings.IgnoreProviderUnavailable("Musixmatch", "artist search endpoint returned no artists");
            }

            Assert.Multiple(() =>
            {
                Assert.That(chartTracks, Is.Not.Empty);
                Assert.That(chartArtists, Is.Not.Empty);
                Assert.That(chartArtists.First().ArtistName, Is.Not.Empty);
                Assert.That(artistSearch, Is.Not.Empty);
                Assert.That(artistSearch.First().ArtistName, Does.Contain("Astley").IgnoreCase);
            });
        }
        catch (MusixMatchRequestException exception) when (CanSkipProviderStatus(exception))
        {
            LiveApiTestSettings.IgnoreProviderUnavailable("Musixmatch", exception);
        }
    }

    private static bool HasReadableSyncData(Track track)
    {
        return track.TrackId.HasValue &&
               track.HasLyrics == 1 &&
               track.HasSubtitles == 1 &&
               track.HasRichsync == 1;
    }

    private static MusixMatch CreateClient()
    {
        string? userToken = LiveApiTestSettings.GetEnvironmentValue("DEVBASE_MUSIXMATCH_USER_TOKEN");

        if (!string.IsNullOrWhiteSpace(userToken)) {
            return new MusixMatch(userToken) { StrictErrorHandling = true };
        }

        return new MusixMatch { StrictErrorHandling = true };
    }

    private static bool CanSkipProviderStatus(MusixMatchRequestException exception)
    {
        return exception.StatusCode is MusixMatchStatusCode.AuthFailed
            or MusixMatchStatusCode.UsageLimitReached
            or MusixMatchStatusCode.NotAuthorized
            or MusixMatchStatusCode.ServerBusy
            or MusixMatchStatusCode.ServerError;
    }
}
