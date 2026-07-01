using DevBase.Api.Apis.BeautifulLyrics;
using DevBase.IntegrationTests.Infrastructure;

namespace DevBase.IntegrationTests.Apis;

[Category("Integration")]
[Category("LiveApi")]
[NonParallelizable]
public sealed class BeautifulLyricsLiveIntegrationTests
{
    private const string KnownRickAstleyIsrc = "GBARL9300135";

    [Test]
    public async Task RawLyricsReturnTimedVocalGroupsForKnownIsrc()
    {
        LiveApiTestSettings.RequireLiveApiTests();

        using CancellationTokenSource timeout = LiveApiTestSettings.CreateTimeout();
        BeautifulLyrics client = new() { StrictErrorHandling = true };

        try
        {
            (string RawLyrics, bool IsRichSync) lyrics = await client.GetRawLyrics(
                KnownRickAstleyIsrc,
                timeout.Token);

            Assert.Multiple(() =>
            {
                Assert.That(lyrics.RawLyrics, Is.Not.Empty);
                Assert.That(lyrics.RawLyrics, Does.Contain("VocalGroups"));
            });
        }
        catch (System.Exception exception) when (exception is not AssertionException)
        {
            LiveApiTestSettings.IgnoreProviderUnavailable("BeautifulLyrics", exception);
        }
    }
}
