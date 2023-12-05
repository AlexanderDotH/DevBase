using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseSearchResultEntry
{
    [JsonProperty("songs")]
    public List<JsonNetEaseSearchResultEntrySong> songs { get; set; }

    [JsonProperty("hasMore")]
    public bool hasMore { get; set; }

    [JsonProperty("songCount")]
    public int songCount { get; set; }
}