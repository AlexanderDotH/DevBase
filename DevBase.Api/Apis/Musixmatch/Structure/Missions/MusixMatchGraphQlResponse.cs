using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Missions;

internal sealed class MusixMatchGraphQlResponse<TData>
    where TData : class
{
    [JsonProperty("data")]
    public TData? Data { get; set; }

    [JsonProperty("errors")]
    public List<MusixMatchGraphQlError>? Errors { get; set; }
}

internal sealed class MusixMatchGraphQlError
{
    [JsonProperty("message")]
    public string Message { get; set; } = "Musixmatch missions GraphQL request failed.";
}
