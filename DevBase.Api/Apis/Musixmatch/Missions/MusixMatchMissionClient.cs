using DevBase.Api.Apis.Musixmatch.Internal;
using DevBase.Api.Apis.Musixmatch.Structure.Missions;
using DevBase.Api.Apis.Musixmatch.Structure.Requests;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using DevBase.Net.Core;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Missions;

public sealed class MusixMatchMissionClient
{
    private const string EndpointUrl = "https://missions-backend-new.musixmatch.com/graphql";
    private const string Origin = "https://curators.musixmatch.com";
    private const string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:101.0) Gecko/20100101 Firefox/101.0";

    private readonly MusixMatchMissionSession _session;

    internal MusixMatchMissionClient(MusixMatch client)
    {
        ArgumentNullException.ThrowIfNull(client);
        EnsureMissionContext(client.Context);

        this._session = CreateSessionAsync(client, CancellationToken.None).GetAwaiter().GetResult();
    }

    public List<Mission> GetMissions()
    {
        return this.GetMissionsAsync().GetAwaiter().GetResult();
    }

    public async Task<List<Mission>> GetMissionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string payload = BuildGraphQlPayload(
                MissionQueries.AvailableMissionsList,
                "AvailableMissionsList",
                new
                {
                    appId = this._session.AppId,
                    userId = this._session.UserId,
                    userToken = this._session.UserToken
                });
            Request request = new Request(EndpointUrl)
                .AsPost()
                .WithJsonBody(payload)
                .WithHeader("Authorization", this._session.JwtToken)
                .WithHeader("Origin", Origin)
                .WithHeader("User-Agent", UserAgent)
                .WithHeader("Accept", "application/json")
                .WithHeader("Content-Type", "application/json");
            Response response = await request.SendAsync(cancellationToken);
            AvailableMissionsData? data =
                await ParseGraphQlBodyAsync<AvailableMissionsData>(response, cancellationToken);

            return data?.AvailableMissions?.Items ?? [];
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.HandleFailure<List<Mission>>(exception) ?? [];
        }
    }

    public List<MissionTrack> GetMissionTracks(
        string missionId,
        string languages = "en",
        string destinationLanguage = "en",
        int limit = 25,
        bool sortAscending = true)
    {
        return this.GetMissionTracksAsync(
                missionId,
                languages,
                destinationLanguage,
                limit,
                sortAscending)
            .GetAwaiter()
            .GetResult();
    }

    public async Task<List<MissionTrack>> GetMissionTracksAsync(
        string missionId,
        string languages = "en",
        string destinationLanguage = "en",
        int limit = 25,
        bool sortAscending = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(missionId);

            string payload = BuildGraphQlPayload(
                MissionQueries.LyricsList,
                "LyricsList",
                new
                {
                    appId = this._session.AppId,
                    destinationLanguage,
                    languages,
                    limit,
                    missionId = NormalizeMissionId(missionId),
                    sortAscending,
                    sortBy = "language",
                    status = "AVAILABLE",
                    userToken = this._session.UserToken
                });
            Request request = new Request(EndpointUrl)
                .AsPost()
                .WithJsonBody(payload)
                .WithHeader("Authorization", this._session.JwtToken)
                .WithHeader("Origin", Origin)
                .WithHeader("User-Agent", UserAgent)
                .WithHeader("Accept", "application/json")
                .WithHeader("Content-Type", "application/json");
            Response response = await request.SendAsync(cancellationToken);
            MissionTracksData? data = await ParseGraphQlBodyAsync<MissionTracksData>(response, cancellationToken);

            return data?.SortedLyrics?.Items ?? [];
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.HandleFailure<List<MissionTrack>>(exception) ?? [];
        }
    }

    [Obsolete("Musixmatch mission reservation is not well tested and may change account state.")]
    public bool ReserveTask(string missionId, string resourceId, string type = "VERIFY")
    {
        return this.ReserveTaskAsync(missionId, resourceId, type).GetAwaiter().GetResult();
    }

    [Obsolete("Musixmatch mission reservation is not well tested and may change account state.")]
    public async Task<bool> ReserveTaskAsync(
        string missionId,
        string resourceId,
        string type = "VERIFY",
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(missionId);
            ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);
            ArgumentException.ThrowIfNullOrWhiteSpace(type);

            string payload = BuildGraphQlPayload(
                MissionQueries.CreateTask,
                "CreateTask",
                new
                {
                    appId = this._session.AppId,
                    missionId = NormalizeMissionId(missionId),
                    resourceId,
                    type,
                    userId = this._session.UserId,
                    userToken = this._session.UserToken
                });
            Request request = new Request(EndpointUrl)
                .AsPost()
                .WithJsonBody(payload)
                .WithHeader("Authorization", this._session.JwtToken)
                .WithHeader("Origin", Origin)
                .WithHeader("User-Agent", UserAgent)
                .WithHeader("Accept", "application/json")
                .WithHeader("Content-Type", "application/json");
            Response response = await request.SendAsync(cancellationToken);
            _ = await ParseGraphQlBodyAsync<object>(response, cancellationToken);

            return true;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.HandleFailure<bool>(exception);
        }
    }

    [Obsolete("Musixmatch mission release is not well tested and may change account state.")]
    public bool CancelTask(string missionId, string resourceId, string taskId, string type)
    {
        return this.CancelTaskAsync(missionId, resourceId, taskId, type).GetAwaiter().GetResult();
    }

    [Obsolete("Musixmatch mission release is not well tested and may change account state.")]
    public async Task<bool> CancelTaskAsync(
        string missionId,
        string resourceId,
        string taskId,
        string type,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(missionId);
            ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);
            ArgumentException.ThrowIfNullOrWhiteSpace(taskId);
            ArgumentException.ThrowIfNullOrWhiteSpace(type);

            string payload = BuildGraphQlPayload(
                MissionQueries.CancelTask,
                "CancelTask",
                new
                {
                    appId = this._session.AppId,
                    missionId = NormalizeMissionId(missionId),
                    resourceId,
                    taskId,
                    type,
                    userId = this._session.UserId,
                    userToken = this._session.UserToken
                });
            Request request = new Request(EndpointUrl)
                .AsPost()
                .WithJsonBody(payload)
                .WithHeader("Authorization", this._session.JwtToken)
                .WithHeader("Origin", Origin)
                .WithHeader("User-Agent", UserAgent)
                .WithHeader("Accept", "application/json")
                .WithHeader("Content-Type", "application/json");
            Response response = await request.SendAsync(cancellationToken);
            _ = await ParseGraphQlBodyAsync<object>(response, cancellationToken);

            return true;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.HandleFailure<bool>(exception);
        }
    }

    private static async Task<MusixMatchMissionSession> CreateSessionAsync(
        MusixMatch client,
        CancellationToken cancellationToken)
    {
        string jwtUrl = await client.BuildMusixMatchUrlAsync(
            MusixMatchEndpoint.JwtGet,
            cancellationToken: cancellationToken);
        Request jwtRequest = new Request(jwtUrl);
        Response jwtResponse = await jwtRequest.SendAsync(cancellationToken);
        JwtGet? jwt = await client.ParseMusixMatchApiBodyAsync<JwtGet>(jwtResponse, cancellationToken);

        string userUrl = await client.BuildMusixMatchUrlAsync(
            MusixMatchEndpoint.UserGet,
            cancellationToken: cancellationToken);
        Request userRequest = new Request(userUrl);
        Response userResponse = await userRequest.SendAsync(cancellationToken);
        MissionUserGet? user = await client.ParseMusixMatchApiBodyAsync<MissionUserGet>(
            userResponse,
            cancellationToken);

        string jwtToken = jwt?.JwtToken ?? string.Empty;
        string userId = user?.UserData?.UserId ?? user?.UserData?.Id ?? string.Empty;

        if (string.IsNullOrWhiteSpace(jwtToken)) {
            throw new InvalidOperationException("Musixmatch missions require a JWT token from jwt.get.");
        }

        if (string.IsNullOrWhiteSpace(userId)) {
            throw new InvalidOperationException("Musixmatch missions require a user id from user.get.");
        }

        string appId = MusixMatchApiContextDefinition.For(client.Context).AppId ?? string.Empty;

        return new MusixMatchMissionSession(
            jwtToken,
            userId,
            appId,
            client.Token.Token,
            client.StrictErrorHandling);
    }

    private static string BuildGraphQlPayload(
        string query,
        string operationName,
        object variables)
    {
        return JsonConvert.SerializeObject(
            new MusixMatchGraphQlRequest(query, operationName, variables),
            Formatting.None);
    }

    private static async Task<TData?> ParseGraphQlBodyAsync<TData>(
        Response response,
        CancellationToken cancellationToken)
        where TData : class
    {
        string content = await response.GetStringAsync(cancellationToken: cancellationToken);
        MusixMatchGraphQlResponse<TData>? graphQlResponse =
            JsonConvert.DeserializeObject<MusixMatchGraphQlResponse<TData>>(content);

        if (graphQlResponse?.Errors is { Count: > 0 }) {
            throw new InvalidOperationException(graphQlResponse.Errors[0].Message);
        }

        return graphQlResponse?.Data;
    }

    private T? HandleFailure<T>(System.Exception exception)
    {
        if (this._session.StrictErrorHandling) {
            throw exception;
        }

        return default;
    }

    private static void EnsureMissionContext(MusixMatchApiContext context)
    {
        if (context == MusixMatchApiContext.Web || context == MusixMatchApiContext.Community) {
            return;
        }

        throw new InvalidOperationException("Musixmatch missions require a Community or Web context token.");
    }

    private static string NormalizeMissionId(string missionId)
    {
        return missionId.Replace("MISSION.", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record MusixMatchMissionSession(
        string JwtToken,
        string UserId,
        string AppId,
        string UserToken,
        bool StrictErrorHandling);

    private sealed class AvailableMissionsData
    {
        [JsonProperty("getAvailableMissions")]
        public GraphQLAvailableMissionsListResponse? AvailableMissions { get; set; }
    }

    private sealed class MissionTracksData
    {
        [JsonProperty("getSortedLyrics")]
        public GraphQLTrackListResponse? SortedLyrics { get; set; }
    }

    private sealed class MissionUserGet
    {
        [JsonProperty("user")]
        public MissionUser? UserData { get; set; }
    }

    private sealed class MissionUser
    {
        [JsonProperty("user_id")]
        public string? UserId { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }
    }
}
