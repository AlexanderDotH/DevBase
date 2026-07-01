using System.Globalization;
using DevBase.Api.Apis.Musixmatch.Enums;
using DevBase.Api.Apis.Musixmatch.Structure.Requests;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using DevBase.Net.Core;
using Newtonsoft.Json;
using System.Text;

namespace DevBase.Api.Apis.Musixmatch;

public partial class MusixMatch
{
    private const string UserFeedbackTypes =
        "lyrics_missing,lyrics_ok,lyrics_ko,lyrics_generic_ko,lyrics_changed,lyrics_subtitle_added,lyrics_favourite_added,lyrics_music_id,track_annotat";

    public UserGet GetUserInfo()
    {
        return this.GetUserInfoAsync().GetAwaiter().GetResult();
    }

    public async Task<UserGet> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        UserGet? user;

        try
        {
            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.UserGet,
                cancellationToken: cancellationToken);

            Request request = new Request(url);

            Response response = await request.SendAsync(cancellationToken);

            user = await this.ParseMusixMatchApiBodyAsync<UserGet>(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<UserGet>(exception);
        }

        return RequireUserInfoResponse(user);
    }

    public OauthTokenGet.Oauthtoken GetSpotifyOauthToken()
    {
        return this.GetSpotifyOauthTokenAsync().GetAwaiter().GetResult();
    }

    public async Task<OauthTokenGet.Oauthtoken> GetSpotifyOauthTokenAsync(
        CancellationToken cancellationToken = default)
    {
        OauthTokenGet? token;

        try
        {
            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.SpotifyOauthTokenGet,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response response = await request.SendAsync(cancellationToken);

            token = await this.ParseMusixMatchApiBodyAsync<OauthTokenGet>(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<OauthTokenGet.Oauthtoken>(exception);
        }

        return RequireOauthTokenResponse(token);
    }

    public List<FeedbackList> GetCrowdUserFeedback()
    {
        return this.GetCrowdUserFeedbackAsync().GetAwaiter().GetResult();
    }

    public async Task<List<FeedbackList>> GetCrowdUserFeedbackAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, string?> parameters = new()
            {
                ["feedback_type"] = UserFeedbackTypes,
                ["part"] = "user,track"
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CrowdUserFeedbackGet,
                parameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response response = await request.SendAsync(cancellationToken);

            CrowdUserFeedbackGet? feedback =
                await this.ParseMusixMatchApiBodyAsync<CrowdUserFeedbackGet>(response, cancellationToken);

            return feedback?.Feedbacks ?? [];
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<FeedbackList>>(exception);
        }
    }

    public List<FeedbackList> GetCrowdFeedback()
    {
        return this.GetCrowdFeedbackAsync().GetAwaiter().GetResult();
    }

    public async Task<List<FeedbackList>> GetCrowdFeedbackAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, string?> parameters = new()
            {
                ["part"] = "user,track"
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CrowdFeedbackGet,
                parameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response response = await request.SendAsync(cancellationToken);

            CrowdFeedbackGet? feedback =
                await this.ParseMusixMatchApiBodyAsync<CrowdFeedbackGet>(response, cancellationToken);

            return feedback?.Feedbacks ?? [];
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<FeedbackList>>(exception);
        }
    }

    public List<Track> GetPollTracks()
    {
        return this.GetPollTracksAsync().GetAwaiter().GetResult();
    }

    public async Task<List<Track>> GetPollTracksAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CrowdPollsTracksSearch,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response response = await request.SendAsync(cancellationToken);

            CrowdPollsTracksSearch? tracks =
                await this.ParseMusixMatchApiBodyAsync<CrowdPollsTracksSearch>(response, cancellationToken);

            return ToTracks(tracks?.Results);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<Track>>(exception);
        }
    }

    public List<Track> GetContributeSuggestionsTracks(
        MusixMatchContributionType contributionType,
        string language = "")
    {
        return this.GetContributeSuggestionsTracksAsync(contributionType, language).GetAwaiter().GetResult();
    }

    public async Task<List<Track>> GetContributeSuggestionsTracksAsync(
        MusixMatchContributionType contributionType,
        string language = "",
        CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, string?> parameters = new()
            {
                ["language"] = language
            };

            string url = await this.BuildMusixMatchUrlAsync(
                ToSuggestionEndpoint(contributionType),
                parameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response response = await request.SendAsync(cancellationToken);

            CrowdSuggestionGet? suggestions =
                await this.ParseMusixMatchApiBodyAsync<CrowdSuggestionGet>(response, cancellationToken);

            return ToTracks(suggestions?.Results);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<Track>>(exception);
        }
    }

    /// <summary>
    /// Submits a track language answer to Musixmatch and may change account/community state.
    /// </summary>
    public void SubmitTrackLanguage(int id, string language)
    {
        this.SubmitTrackLanguageAsync(id, language).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Submits a track language answer to Musixmatch and may change account/community state.
    /// </summary>
    public async Task SubmitTrackLanguageAsync(
        int id,
        string language,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, string?> data = new()
            {
                ["answer_id"] = "lyrics_ai_ugc_language",
                ["track_id"] = id.ToString(CultureInfo.InvariantCulture),
                ["selected_language"] = language,
                ["question_id"] = "lyrics_ai_language_collect"
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.AiQuestionPost,
                cancellationToken: cancellationToken);

            string body = BuildMusixMatchBody(data);

            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response response = await request.SendAsync(cancellationToken);

            _ = await this.ParseMusixMatchApiBodyAsync<object>(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            _ = this.Throw<object>(exception);
        }
    }

    /// <summary>
    /// Submits a track mood answer to Musixmatch and may change account/community state.
    /// </summary>
    public void SubmitTrackMood(int id, int energy, int mood)
    {
        this.SubmitTrackMoodAsync(id, energy, mood).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Submits a track mood answer to Musixmatch and may change account/community state.
    /// </summary>
    public async Task SubmitTrackMoodAsync(
        int id,
        int energy,
        int mood,
        CancellationToken cancellationToken = default)
    {
        ValidatePercentage(energy, nameof(energy));
        ValidatePercentage(mood, nameof(mood));

        try
        {
            Dictionary<string, string?> data = new()
            {
                ["answer_id"] = "lyrics_ai_mood_analysis_v3_value",
                ["track_id"] = id.ToString(CultureInfo.InvariantCulture),
                ["energy_ratio"] = energy.ToString(CultureInfo.InvariantCulture),
                ["mood_ratio"] = mood.ToString(CultureInfo.InvariantCulture),
                ["question_id"] = "lyrics_ai_mood_analysis_v3"
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.AiQuestionPost,
                cancellationToken: cancellationToken);

            string body = BuildMusixMatchBody(data);

            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response response = await request.SendAsync(cancellationToken);

            _ = await this.ParseMusixMatchApiBodyAsync<object>(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            _ = this.Throw<object>(exception);
        }
    }

    public List<User> GetUserWeeklyTop(string country = "", PaginationParameters? pagination = null)
    {
        return this.GetUserWeeklyTopAsync(country, pagination).GetAwaiter().GetResult();
    }

    public async Task<List<User>> GetUserWeeklyTopAsync(
        string country = "",
        PaginationParameters? pagination = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            PaginationParameters page = pagination ?? new PaginationParameters();
            Dictionary<string, string?> parameters = new()
            {
                ["country"] = country,
                ["page"] = page.Page.ToString(CultureInfo.InvariantCulture),
                ["page_size"] = page.PageSize.ToString(CultureInfo.InvariantCulture)
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CrowdChartUsersGet,
                parameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response response = await request.SendAsync(cancellationToken);

            CrowdChartUsersGet? users =
                await this.ParseMusixMatchApiBodyAsync<CrowdChartUsersGet>(response, cancellationToken);

            return (users?.Results ?? [])
                .Select(item => item.User)
                .Where(user => user is not null)
                .ToList()!;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<User>>(exception);
        }
    }

    public User GetUserScore()
    {
        return this.GetUserScoreAsync().GetAwaiter().GetResult();
    }

    public async Task<User> GetUserScoreAsync(CancellationToken cancellationToken = default)
    {
        CrowdScoreGet? score;

        try
        {
            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CrowdScoreGet,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response response = await request.SendAsync(cancellationToken);

            score = await this.ParseMusixMatchApiBodyAsync<CrowdScoreGet>(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<User>(exception);
        }

        return RequireUserScoreResponse(score);
    }

    /// <summary>
    /// Updates the current Musixmatch user's profile country and may change account/community state.
    /// </summary>
    public void UpdateUserProfileCountry(string country)
    {
        this.UpdateUserProfileCountryAsync(country).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Updates the current Musixmatch user's profile country and may change account/community state.
    /// </summary>
    public async Task UpdateUserProfileCountryAsync(
        string country,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(country);

        try
        {
            Dictionary<string, string?> data = CreateUserProfilePayload(
                new Dictionary<string, object?>
                {
                    ["country"] = country.ToUpperInvariant()
                });

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CrowdUserProfilePost,
                cancellationToken: cancellationToken);

            string body = BuildMusixMatchBody(data);

            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response response = await request.SendAsync(cancellationToken);

            _ = await this.ParseMusixMatchApiBodyAsync<CrowdUserProfilePost>(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            _ = this.Throw<object>(exception);
        }
    }

    /// <summary>
    /// Updates the current Musixmatch user's favourite artists and may change account/community state.
    /// </summary>
    public void UpdateUserProfileFavouriteArtists(List<int> artistIds)
    {
        this.UpdateUserProfileFavouriteArtistsAsync(artistIds).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Updates the current Musixmatch user's favourite artists and may change account/community state.
    /// </summary>
    public async Task UpdateUserProfileFavouriteArtistsAsync(
        List<int> artistIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(artistIds);

        try
        {
            Dictionary<string, string?> data = CreateUserProfilePayload(
                new Dictionary<string, object?>
                {
                    ["favourite_artists"] = artistIds
                });

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CrowdUserProfilePost,
                cancellationToken: cancellationToken);

            string body = BuildMusixMatchBody(data);

            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response response = await request.SendAsync(cancellationToken);

            _ = await this.ParseMusixMatchApiBodyAsync<CrowdUserProfilePost>(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            _ = this.Throw<object>(exception);
        }
    }

    /// <summary>
    /// Updates the current Musixmatch user's favourite genres and may change account/community state.
    /// </summary>
    public void UpdateUserProfileFavouriteGenres(List<int> genreIds)
    {
        this.UpdateUserProfileFavouriteGenresAsync(genreIds).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Updates the current Musixmatch user's favourite genres and may change account/community state.
    /// </summary>
    public async Task UpdateUserProfileFavouriteGenresAsync(
        List<int> genreIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(genreIds);

        try
        {
            Dictionary<string, string?> data = CreateUserProfilePayload(
                new Dictionary<string, object?>
                {
                    ["favourite_genres"] = genreIds
                });

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CrowdUserProfilePost,
                cancellationToken: cancellationToken);

            string body = BuildMusixMatchBody(data);

            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));

            Response response = await request.SendAsync(cancellationToken);

            _ = await this.ParseMusixMatchApiBodyAsync<CrowdUserProfilePost>(response, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            _ = this.Throw<object>(exception);
        }
    }

    private static Dictionary<string, string?> CreateUserProfilePayload(Dictionary<string, object?> profile)
    {
        string profileJson = JsonConvert.SerializeObject(profile);

        return new Dictionary<string, string?>
        {
            ["profile"] = profileJson
        };
    }

    private static MusixMatchEndpoint ToSuggestionEndpoint(MusixMatchContributionType contributionType)
    {
        return contributionType switch
        {
            MusixMatchContributionType.Lyrics => MusixMatchEndpoint.CrowdUserSuggestionLyricsGet,
            MusixMatchContributionType.Sync => MusixMatchEndpoint.CrowdUserSuggestionSubtitlesGet,
            MusixMatchContributionType.Translation => MusixMatchEndpoint.CrowdUserSuggestionTranslationsGet,
            MusixMatchContributionType.Vote => MusixMatchEndpoint.CrowdUserSuggestionVotesGet,
            _ => throw new ArgumentOutOfRangeException(nameof(contributionType), contributionType, null)
        };
    }

    private static List<Track> ToTracks(IEnumerable<CrowdSuggestionTrackList>? source)
    {
        if (source is null) {
            return [];
        }

        return source
            .Select(item => item.Track)
            .Where(track => track is not null)
            .ToList()!;
    }

    private static void ValidatePercentage(int value, string parameterName)
    {
        if (value is < 0 or > 100) {
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be between 0 and 100.");
        }
    }

    private static UserGet RequireUserInfoResponse(UserGet? response)
    {
        return RequireResponseData(
            response,
            "user",
            null,
            HasUserInfoData);
    }

    private static OauthTokenGet.Oauthtoken RequireOauthTokenResponse(OauthTokenGet? response)
    {
        return RequireResponseData(
            response?.Token,
            "oauth token",
            null,
            HasOauthTokenData);
    }

    private static User RequireUserScoreResponse(CrowdScoreGet? response)
    {
        return RequireResponseData(
            response?.User,
            "user score",
            null,
            HasUserScoreData);
    }

    private static bool HasUserInfoData(UserGet user)
    {
        UserGet.UserAccount? account = user.UserData;

        return account is not null &&
               (!string.IsNullOrWhiteSpace(account.UserId)
                || !string.IsNullOrWhiteSpace(account.Id)
                || !string.IsNullOrWhiteSpace(account.Name)
                || !string.IsNullOrWhiteSpace(account.Profile?.UserId)
                || !string.IsNullOrWhiteSpace(account.Profile?.Email)
                || !string.IsNullOrWhiteSpace(account.Profile?.Name));
    }

    private static bool HasOauthTokenData(OauthTokenGet.Oauthtoken token)
    {
        return token.OauthRefreshtokenReply is not null
               || token.LastUpdated.HasValue
               || !string.IsNullOrWhiteSpace(token.Callback)
               || !string.IsNullOrWhiteSpace(token.Service)
               || !string.IsNullOrWhiteSpace(token.ServiceUserPrefix);
    }

    private static bool HasUserScoreData(User user)
    {
        return !string.IsNullOrWhiteSpace(user.Uaid)
               || !string.IsNullOrWhiteSpace(user.UserName)
               || !string.IsNullOrWhiteSpace(user.Level)
               || !string.IsNullOrWhiteSpace(user.RankName)
               || user.Score > 0
               || user.WeeklyScore > 0
               || user.RankLevel > 0
               || user.Position > 0;
    }
}
