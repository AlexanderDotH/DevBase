using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenLyricsClient.Structure.Json;

public class JsonOpenLyricsClientAiSyncItem
{
    [JsonProperty("startTimestamp")]
    public int startTimestamp { get; set; }

    [JsonProperty("endTimeStamp")]
    public int endTimeStamp { get; set; }

    [JsonProperty("text")]
    public string text { get; set; }
}