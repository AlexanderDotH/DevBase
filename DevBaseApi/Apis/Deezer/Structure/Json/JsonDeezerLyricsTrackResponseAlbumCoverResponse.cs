using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerLyricsTrackResponseAlbumCoverResponse
{
    [JsonProperty("small")]
    public List<string> small { get; set; }

    [JsonProperty("medium")]
    public List<string> medium { get; set; }

    [JsonProperty("large")]
    public List<string> large { get; set; }

    [JsonProperty("explicitStatus")]
    public bool explicitStatus { get; set; }

    [JsonProperty("__typename")]
    public string __typename { get; set; }
}