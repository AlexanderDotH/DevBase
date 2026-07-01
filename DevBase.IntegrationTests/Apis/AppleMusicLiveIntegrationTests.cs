using DevBase.Api.Apis.AppleMusic;
using DevBase.Api.Apis.AppleMusic.Structure.Objects;
using DevBase.Api.Exceptions;
using DevBase.IntegrationTests.Infrastructure;
using DevBase.Net.Security.Token;

namespace DevBase.IntegrationTests.Apis;

[Category("Integration")]
[Category("LiveApi")]
[NonParallelizable]
public sealed class AppleMusicLiveIntegrationTests
{
    [Test]
    public async Task SearchUsesCurrentWebAccessToken()
    {
        LiveApiTestSettings.RequireLiveApiTests();

        using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout(TimeSpan.FromSeconds(60));
        AppleMusic client = await CreateClientOrIgnoreAsync(timeout.Token);

        List<AppleMusicTrack> tracks = await client.Search("Rick%20Astley", 5, timeout.Token);
        AppleMusicTrack first = tracks.First();

        Assert.Multiple(() =>
        {
            Assert.That(tracks, Is.Not.Empty);
            Assert.That(first.Title, Is.Not.Empty);
            Assert.That(first.Album, Is.Not.Empty);
            Assert.That(first.ServiceInternalId, Is.Not.Empty);
            Assert.That(first.Isrc, Is.Not.Empty);
        });
    }

    [Test]
    public async Task LyricsReturnTtmlWhenMediaUserTokenIsConfigured()
    {
        LiveApiTestSettings.RequireLiveApiTests();
        string mediaUserToken = LiveApiTestSettings.RequireEnvironmentValue("DEVBASE_APPLE_MUSIC_MEDIA_USER_TOKEN");

        using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout(TimeSpan.FromSeconds(60));
        AppleMusic client = await CreateClientOrIgnoreAsync(timeout.Token);

        List<AppleMusicTrack> tracks = await client.Search("Rick%20Astley", 5, timeout.Token);
        AppleMusicTrack track = tracks.First(candidate => !string.IsNullOrWhiteSpace(candidate.ServiceInternalId));

        client.WithMediaUserToken(new GenericAuthenticationToken
        {
            Token = mediaUserToken,
            ExpiresAt = DateTime.MaxValue
        });

        var lyrics = await client.GetLyrics(track.ServiceInternalId, timeout.Token);

        Assert.Multiple(() =>
        {
            Assert.That(lyrics.Data, Is.Not.Empty);
            Assert.That(lyrics.Data.First().Attributes.Ttml, Is.Not.Empty);
        });
    }

    private static async Task<AppleMusic> CreateClientOrIgnoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await AppleMusic.WithAccessToken(cancellationToken);
        }
        catch (AppleMusicException exception)
        {
            LiveApiTestSettings.IgnoreProviderUnavailable("Apple Music", exception);
            throw;
        }
    }
}
