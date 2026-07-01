using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class CrowdPollsTracksSearch : MusixMatchApiResponse
{
    [JsonProperty("track_list")]
    public List<CrowdSuggestionTrackList> Results { get; set; } = [];
}
