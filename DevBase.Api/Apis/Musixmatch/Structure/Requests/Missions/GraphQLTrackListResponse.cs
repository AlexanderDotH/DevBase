using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class GraphQLTrackListResponse
{
    [JsonProperty("items")]
    public List<MissionTrack> Items { get; set; } = [];
}
