using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class CrowdSuggestionGet : MusixMatchApiResponse
{
    [JsonProperty("track_list")]
    public List<CrowdSuggestionTrackList> Results { get; set; } = [];
}

public sealed class CrowdSuggestionTrackList
{
    [JsonProperty("track")]
    public Track? Track { get; set; }
}
