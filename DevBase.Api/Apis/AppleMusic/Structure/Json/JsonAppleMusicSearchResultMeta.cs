using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultMeta
{
    [JsonProperty("formerIds")]
    public List<string> FormerIds { get; set; }

    [JsonProperty("results")]
    public JsonAppleMusicSearchResultMetaResults Results { get; set; }

    [JsonProperty("metrics")]
    public JsonAppleMusicSearchResultMetaMetrics Metrics { get; set; }
}