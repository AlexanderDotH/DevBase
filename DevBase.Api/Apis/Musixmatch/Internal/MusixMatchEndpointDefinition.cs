namespace DevBase.Api.Apis.Musixmatch.Internal;

internal sealed record MusixMatchEndpointDefinition(
    string Resource,
    MusixMatchRequestMethod Method)
{
    private static readonly IReadOnlyDictionary<MusixMatchEndpoint, MusixMatchEndpointDefinition> Definitions =
        new Dictionary<MusixMatchEndpoint, MusixMatchEndpointDefinition>
        {
            [MusixMatchEndpoint.None] = new(string.Empty, MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TokenGet] = new("token.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackGet] = new("track.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackLyricsGet] = new("track.lyrics.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackSearch] = new("track.search", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackSnippetGet] = new("track.snippet.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackSubtitleGet] = new("track.subtitle.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackSubtitlePost] = new("track.subtitle.post", MusixMatchRequestMethod.Post),
            [MusixMatchEndpoint.RequestJwtToken] = new("jwt.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.UserGet] = new("user.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.SpotifyOauthTokenGet] = new("spotify.oauthtoken.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackLyricsTranslationGet] = new("track.lyrics.translation.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackLyricsTranslationPost] = new("track.lyrics.translation.post", MusixMatchRequestMethod.Post),
            [MusixMatchEndpoint.CrowdUserFeedbackGet] = new("crowd.user.feedback.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.CrowdFeedbackGet] = new("crowd.feedback.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackLyricsPost] = new("track.lyrics.post", MusixMatchRequestMethod.Post),
            [MusixMatchEndpoint.ChartTracksGet] = new("chart.tracks.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.CrowdPollsTracksSearch] = new("crowd.polls.tracks.search", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.CrowdChartUsersGet] = new("crowd.chart.users.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.CrowdUserSuggestionTranslationsGet] = new("crowd.user.suggestion.translations.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.CrowdUserSuggestionLyricsGet] = new("crowd.user.suggestion.lyrics.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.CrowdUserSuggestionSubtitlesGet] = new("crowd.user.suggestion.subtitles.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.CrowdUserSuggestionVotesGet] = new("crowd.user.suggestion.votes.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.AiQuestionPost] = new("crowd.ai.question.post", MusixMatchRequestMethod.Post),
            [MusixMatchEndpoint.CredentialPost] = new("credential.post", MusixMatchRequestMethod.Post),
            [MusixMatchEndpoint.AccountPost] = new("account.post", MusixMatchRequestMethod.Post),
            [MusixMatchEndpoint.TrackRichsyncGet] = new("track.richsync.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.TrackRichsyncPost] = new("track.richsync.post", MusixMatchRequestMethod.Post),
            [MusixMatchEndpoint.CrowdScoreGet] = new("crowd.score.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.ChartArtistsGet] = new("chart.artists.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.MusicGenresGet] = new("music.genres.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.AlbumGet] = new("album.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.AlbumTracksGet] = new("album.tracks.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.ArtistAlbumsGet] = new("artist.albums.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.ArtistGet] = new("artist.get", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.ArtistSearch] = new("artist.search", MusixMatchRequestMethod.Get),
            [MusixMatchEndpoint.CrowdUserProfilePost] = new("crowd.user.profile.post", MusixMatchRequestMethod.Post),
            [MusixMatchEndpoint.CrowdTrackTranslationsGet] = new("crowd.track.translations.get", MusixMatchRequestMethod.Get)
        };

    public static MusixMatchEndpointDefinition For(MusixMatchEndpoint endpoint)
    {
        if (Definitions.TryGetValue(endpoint, out MusixMatchEndpointDefinition? definition)) {
            return definition;
        }

        throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, "Unsupported Musixmatch endpoint.");
    }
}
