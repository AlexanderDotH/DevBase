namespace DevBase.Test.DevBaseApi.MusixMatch;

public sealed class MusixMatchFixtureParserTests
{
    [Test]
    public void TrackSearchFixture_MapsTrackList()
    {
        object response = MusixMatchTestSupport.DeserializeFixtureBody(
            "track-search.json",
            "Structure.Requests.TrackSearch");

        object firstTrackList = MusixMatchTestSupport.GetList(response, "Results").Single();
        object track = MusixMatchTestSupport.GetMember(firstTrackList, "Track")!;

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetInt(track, "TrackId"), Is.EqualTo(101));
            Assert.That(MusixMatchTestSupport.GetString(track, "TrackName"), Is.EqualTo("Fixture Search Track"));
            Assert.That(MusixMatchTestSupport.GetString(track, "ArtistName"), Is.EqualTo("Fixture Artist"));
        });
    }

    [Test]
    public void TrackGetFixture_MapsTrack()
    {
        object response = MusixMatchTestSupport.DeserializeFixtureBody(
            "track-get.json",
            "Structure.Requests.TrackGet");

        object track = MusixMatchTestSupport.GetMember(response, "Track")!;

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetInt(track, "TrackId"), Is.EqualTo(202));
            Assert.That(MusixMatchTestSupport.GetString(track, "TrackName"), Is.EqualTo("Fixture Track"));
            Assert.That(MusixMatchTestSupport.GetString(track, "AlbumName"), Is.EqualTo("Fixture Album"));
        });
    }

    [Test]
    public void LyricsFixture_MapsLyricsBody()
    {
        object response = MusixMatchTestSupport.DeserializeFixtureBody(
            "lyrics-get.json",
            "Structure.Requests.TrackLyricsGet");

        object lyrics = MusixMatchTestSupport.GetMember(response, "Lyrics")!;

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetInt(lyrics, "LyricsId"), Is.EqualTo(303));
            Assert.That(MusixMatchTestSupport.GetString(lyrics, "LyricsBody"), Does.Contain("offline fixtures"));
            Assert.That(MusixMatchTestSupport.GetString(lyrics, "LyricsLanguage"), Is.EqualTo("en"));
        });
    }

    [Test]
    public void SubtitleFixture_MapsRawAndFormattedLines()
    {
        object response = MusixMatchTestSupport.DeserializeFixtureBody(
            "subtitle-get.json",
            "Structure.Requests.TrackSubtitleGet");

        object subtitle = MusixMatchTestSupport.GetMember(response, "Subtitle")!;
        string subtitleBody = MusixMatchTestSupport.GetString(subtitle, "SubtitleBody")!;
        Type subtitlesType = MusixMatchTestSupport.ApiType("Structure.Types.Subtitles");
        object subtitles = Activator.CreateInstance(subtitlesType, [subtitleBody])!;

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetInt(subtitle, "SubtitleId"), Is.EqualTo(404));
            Assert.That(MusixMatchTestSupport.GetList(subtitles, "Lines"), Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void RichsyncFixture_MapsRawAndFormattedLines()
    {
        object response = MusixMatchTestSupport.DeserializeFixtureBody(
            "richsync-get.json",
            "Structure.Requests.TrackRichsyncGet");

        object raw = MusixMatchTestSupport.GetMember(response, "Richsync")!;
        string richsyncBody = MusixMatchTestSupport.GetString(raw, "RichsyncBody")!;
        Type richsyncType = MusixMatchTestSupport.ApiType("Structure.Types.Richsync");
        object richsync = Activator.CreateInstance(richsyncType, [richsyncBody])!;

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetInt(raw, "RichsyncId"), Is.EqualTo(505));
            Assert.That(MusixMatchTestSupport.GetList(richsync, "Lines"), Has.Count.EqualTo(1));
            Assert.That(MusixMatchTestSupport.GetList(richsync, "Characters"), Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void CatalogFixtures_MapRepresentativeBodies()
    {
        object chartArtists = MusixMatchTestSupport.DeserializeFixtureBody(
            "chart-artists-get.json",
            "Structure.Requests.ChartArtistsGet");
        object albumGet = MusixMatchTestSupport.DeserializeFixtureBody(
            "album-get.json",
            "Structure.Requests.AlbumGet");
        object genresGet = MusixMatchTestSupport.DeserializeFixtureBody(
            "music-genres-get.json",
            "Structure.Requests.MusicGenresGet");

        object artist = MusixMatchTestSupport.GetMember(MusixMatchTestSupport.GetList(chartArtists, "Results").Single(), "Artist")!;
        object album = MusixMatchTestSupport.GetMember(albumGet, "Album")!;
        object genre = MusixMatchTestSupport.GetMember(MusixMatchTestSupport.GetList(genresGet, "Results").Single(), "MusicGenre")!;

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetString(artist, "ArtistName"), Is.EqualTo("Fixture Artist"));
            Assert.That(MusixMatchTestSupport.GetString(album, "AlbumName"), Is.EqualTo("Fixture Album"));
            Assert.That(MusixMatchTestSupport.GetString(genre, "MusicGenreName"), Is.EqualTo("Fixture Genre"));
        });
    }

    [Test]
    public void UserAndCrowdFixtures_MapRepresentativeBodies()
    {
        object userGet = MusixMatchTestSupport.DeserializeFixtureBody(
            "user-get.json",
            "Structure.Requests.UserGet");
        object crowdFeedback = MusixMatchTestSupport.DeserializeFixtureBody(
            "crowd-user-feedback-get.json",
            "Structure.Requests.CrowdUserFeedbackGet");

        object user = MusixMatchTestSupport.GetFirstExistingMember(userGet, "UserData", "User")!;
        object feedbackRoot = MusixMatchTestSupport.GetList(crowdFeedback, "Feedbacks").Single();
        object feedback = MusixMatchTestSupport.GetMember(feedbackRoot, "Feedback")!;

        Assert.Multiple(() =>
        {
            Assert.That(MusixMatchTestSupport.GetFirstExistingMember(user, "UserId", "Id")?.ToString(), Is.EqualTo("fixture-user"));
            Assert.That(MusixMatchTestSupport.GetString(feedback, "FriendlyTypeId"), Is.EqualTo("lyrics_ok"));
        });
    }
}
