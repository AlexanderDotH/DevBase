using System.Reflection;

namespace DevBase.Test.DevBaseApi.MusixMatch;

public sealed class MusixMatchPublicSurfaceTests
{
    private static readonly ExpectedSignature[] ExpectedClientMethods =
    [
        new("IssueTokenAsync", "MusixMatchApiContext", "CancellationToken"),
        new("IssueToken", "MusixMatchApiContext"),
        new("LoginWithMusixMatchAccountAsync", "String", "String", "CancellationToken"),
        new("LoginWithMusixMatchAccount", "String", "String"),
        new("AuthenticateGoogleAsync", "String", "CancellationToken"),
        new("AuthenticateGoogle", "String"),
        new("AuthenticateMusixMatchAsync", "String", "String", "CancellationToken"),
        new("AuthenticateMusixMatch", "String", "String"),
        new("DeauthenticateAsync", "CancellationToken"),
        new("Deauthenticate"),
        new("Login", "String", "String", "CancellationToken"),
        new("GetAuthUrl"),
        new("CreateMissionClient"),
        new("SongSearchAsync", "TrackSearchParameters", "PaginationParameters", "CancellationToken"),
        new("SongSearch", "TrackSearchParameters", "PaginationParameters"),
        new("SongSearchAsync", "String", "PaginationParameters", "CancellationToken"),
        new("SongSearch", "String", "PaginationParameters"),
        new("SongSearchAsync", "String", "String", "PaginationParameters", "CancellationToken"),
        new("SongSearch", "String", "String", "PaginationParameters"),
        new("SongSearchByLyricsAsync", "String", "CancellationToken"),
        new("SongSearchByLyrics", "String"),
        new("GetTrackByIdAsync", "Int32", "CancellationToken"),
        new("GetTrackById", "Int32"),
        new("GetTrackSnippetAsync", "Int32", "CancellationToken"),
        new("GetTrackSnippet", "Int32"),
        new("GetTrackLyricsAsync", "Int32", "CancellationToken"),
        new("GetTrackLyrics", "Int32"),
        new("GetTrackSubtitlesRawAsync", "Int32", "MusixMatchSubtitleFormat", "CancellationToken"),
        new("GetTrackSubtitlesRaw", "Int32", "MusixMatchSubtitleFormat"),
        new("GetTrackSubtitlesAsync", "Int32", "CancellationToken"),
        new("GetTrackSubtitles", "Int32"),
        new("SubmitTrackSubtitlesRawAsync", "Int32", "String", "CancellationToken"),
        new("SubmitTrackSubtitlesRaw", "Int32", "String"),
        new("SubmitTrackSubtitlesAsync", "Int32", "Subtitles", "CancellationToken"),
        new("SubmitTrackSubtitles", "Int32", "Subtitles"),
        new("GetTrackRichsyncRawAsync", "Int32", "CancellationToken"),
        new("GetTrackRichsyncRaw", "Int32"),
        new("GetTrackRichsyncAsync", "Int32", "CancellationToken"),
        new("GetTrackRichsync", "Int32"),
        new("SubmitTrackRichsyncRawAsync", "Int32", "String", "CancellationToken"),
        new("SubmitTrackRichsyncRaw", "Int32", "String"),
        new("SubmitTrackRichsyncAsync", "Int32", "Richsync", "CancellationToken"),
        new("SubmitTrackRichsync", "Int32", "Richsync"),
        new("GetLyricsTranslationAsync", "Int32", "String", "CancellationToken"),
        new("GetLyricsTranslation", "Int32", "String"),
        new("GetLyricsTranslationLinesAsync", "Int32", "String", "CancellationToken"),
        new("GetLyricsTranslationLines", "Int32", "String"),
        new("GetLyricsTranslationsRawAsync", "Int32", "String", "CancellationToken"),
        new("GetLyricsTranslationsRaw", "Int32", "String"),
        new("SubmitTrackTranslationsRawAsync", "Int32", "List<TranslationPost>", "Int32", "CancellationToken"),
        new("SubmitTrackTranslationsRaw", "Int32", "List<TranslationPost>", "Int32"),
        new("ChartTracksGetAsync", "PaginationParameters", "CancellationToken"),
        new("ChartTracksGet", "PaginationParameters"),
        new("GetChartArtistsAsync", "String", "PaginationParameters", "CancellationToken"),
        new("GetChartArtists", "String", "PaginationParameters"),
        new("ArtistSearchAsync", "String", "PaginationParameters", "CancellationToken"),
        new("ArtistSearch", "String", "PaginationParameters"),
        new("GetArtistByIdAsync", "Int32", "CancellationToken"),
        new("GetArtistById", "Int32"),
        new("GetArtistAlbumsAsync", "Int32", "PaginationParameters", "CancellationToken"),
        new("GetArtistAlbums", "Int32", "PaginationParameters"),
        new("GetAlbumByIdAsync", "Int32", "CancellationToken"),
        new("GetAlbumById", "Int32"),
        new("GetAlbumTracksAsync", "Int32", "PaginationParameters", "CancellationToken"),
        new("GetAlbumTracks", "Int32", "PaginationParameters"),
        new("GetMusicGenresAsync", "CancellationToken"),
        new("GetMusicGenres"),
        new("GetUserInfoAsync", "CancellationToken"),
        new("GetUserInfo"),
        new("GetSpotifyOauthTokenAsync", "CancellationToken"),
        new("GetSpotifyOauthToken"),
        new("GetCrowdUserFeedbackAsync", "CancellationToken"),
        new("GetCrowdUserFeedback"),
        new("GetCrowdFeedbackAsync", "CancellationToken"),
        new("GetCrowdFeedback"),
        new("GetPollTracksAsync", "CancellationToken"),
        new("GetPollTracks"),
        new("GetContributeSuggestionsTracksAsync", "MusixMatchContributionType", "String", "CancellationToken"),
        new("GetContributeSuggestionsTracks", "MusixMatchContributionType", "String"),
        new("GetUserScoreAsync", "CancellationToken"),
        new("GetUserScore"),
        new("GetUserWeeklyTopAsync", "String", "PaginationParameters", "CancellationToken"),
        new("GetUserWeeklyTop", "String", "PaginationParameters"),
        new("SubmitTrackLyricsAsync", "Int32", "String", "CancellationToken"),
        new("SubmitTrackLyrics", "Int32", "String"),
        new("SubmitTrackLanguageAsync", "Int32", "String", "CancellationToken"),
        new("SubmitTrackLanguage", "Int32", "String"),
        new("SubmitTrackMoodAsync", "Int32", "Int32", "Int32", "CancellationToken"),
        new("SubmitTrackMood", "Int32", "Int32", "Int32"),
        new("UpdateUserProfileCountryAsync", "String", "CancellationToken"),
        new("UpdateUserProfileCountry", "String"),
        new("UpdateUserProfileFavouriteArtistsAsync", "List<Int32>", "CancellationToken"),
        new("UpdateUserProfileFavouriteArtists", "List<Int32>"),
        new("UpdateUserProfileFavouriteGenresAsync", "List<Int32>", "CancellationToken"),
        new("UpdateUserProfileFavouriteGenres", "List<Int32>")
    ];

    private static readonly ExpectedSignature[] ExpectedMissionMethods =
    [
        new("GetMissionsAsync", "CancellationToken"),
        new("GetMissions"),
        new("GetMissionTracksAsync", "String", "String", "String", "Int32", "Boolean", "CancellationToken"),
        new("GetMissionTracks", "String", "String", "String", "Int32", "Boolean"),
        new("ReserveTaskAsync", "String", "String", "String", "CancellationToken"),
        new("ReserveTask", "String", "String", "String"),
        new("CancelTaskAsync", "String", "String", "String", "String", "CancellationToken"),
        new("CancelTask", "String", "String", "String", "String")
    ];

    public static IEnumerable<TestCaseData> ClientMethodCases()
    {
        return ExpectedClientMethods.Select(signature => new TestCaseData(signature).SetName(signature.ToString()));
    }

    public static IEnumerable<TestCaseData> MissionMethodCases()
    {
        return ExpectedMissionMethods.Select(signature => new TestCaseData(signature).SetName($"Mission_{signature}"));
    }

    [TestCaseSource(nameof(ClientMethodCases))]
    public void MusixMatch_ExposesThePlannedClientMethodSurface(ExpectedSignature expected)
    {
        Type clientType = typeof(Api.Apis.Musixmatch.MusixMatch);

        Assert.That(
            HasPublicInstanceMethod(clientType, expected),
            Is.True,
            $"Expected {clientType.Name} to expose {expected}.");
    }

    [Test]
    public void MusixMatch_ExposesThePlannedConstructors()
    {
        Type clientType = typeof(Api.Apis.Musixmatch.MusixMatch);
        IReadOnlySet<string> constructorSignatures = clientType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .Select(constructor => string.Join(", ", constructor.GetParameters().Select(parameter => FriendlyTypeName(parameter.ParameterType))))
            .ToHashSet(StringComparer.Ordinal);

        Assert.Multiple(() =>
        {
            Assert.That(constructorSignatures, Does.Contain(string.Empty));
            Assert.That(constructorSignatures, Does.Contain("String, MusixMatchApiContext"));
            Assert.That(constructorSignatures, Does.Contain("MusixMatchToken"));
        });
    }

    [TestCaseSource(nameof(MissionMethodCases))]
    public void MusixMatchMissionClient_ExposesThePlannedMissionSurface(ExpectedSignature expected)
    {
        Type missionClientType = MusixMatchTestSupport.ApiType("Missions.MusixMatchMissionClient");

        Assert.That(
            HasPublicInstanceMethod(missionClientType, expected),
            Is.True,
            $"Expected {missionClientType.Name} to expose {expected}.");
    }

    [Test]
    public void DefaultConstructor_IsSourceDocumentedAsAutoTokenAndIsNotInvokedByOfflineTests()
    {
        string source = MusixMatchTestSupport.ReadProductionSource("MusixMatch.cs");

        Assert.Multiple(() =>
        {
            Assert.That(source, Does.Match(@"public\s+MusixMatch\s*\(\s*\)"));
            Assert.That(source, Does.Contain("IssueToken"));
        });
    }

    [Test]
    public void AccountChangingMethods_AreDocumentedAsUnsafeOrObsolete()
    {
        string source = MusixMatchTestSupport.ReadAllMusixMatchSources();

        foreach (string methodName in new[]
        {
            "SubmitTrackLyrics",
            "SubmitTrackSubtitles",
            "SubmitTrackRichsync",
            "SubmitTrackTranslationsRaw",
            "SubmitTrackLanguage",
            "SubmitTrackMood",
            "UpdateUserProfileCountry",
            "UpdateUserProfileFavouriteArtists",
            "UpdateUserProfileFavouriteGenres",
            "ReserveTask",
            "CancelTask"
        })
        {
            int index = source.IndexOf(methodName, StringComparison.Ordinal);
            Assert.That(index, Is.GreaterThanOrEqualTo(0), $"Expected {methodName} to exist.");

            int start = Math.Max(0, index - 500);
            string context = source[start..index];

            Assert.That(
                context.Contains("may change account/community state", StringComparison.OrdinalIgnoreCase)
                || context.Contains("[Obsolete", StringComparison.Ordinal),
                Is.True,
                $"{methodName} should be documented as account/community-state changing or marked obsolete.");
        }
    }

    private static bool HasPublicInstanceMethod(Type type, ExpectedSignature expected)
    {
        return type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => !method.IsSpecialName)
            .Where(method => method.Name == expected.Name)
            .Any(method => method.GetParameters()
                .Select(parameter => FriendlyTypeName(parameter.ParameterType))
                .SequenceEqual(expected.ParameterTypeNames, StringComparer.Ordinal));
    }

    private static string FriendlyTypeName(Type type)
    {
        if (!type.IsGenericType) {
            return type.Name;
        }

        string genericName = type.Name[..type.Name.IndexOf('`')];
        string arguments = string.Join(", ", type.GetGenericArguments().Select(FriendlyTypeName));
        return $"{genericName}<{arguments}>";
    }

    public sealed record ExpectedSignature(string Name, params string[] ParameterTypeNames)
    {
        public override string ToString()
        {
            return $"{this.Name}({string.Join(", ", this.ParameterTypeNames)})";
        }
    }
}
