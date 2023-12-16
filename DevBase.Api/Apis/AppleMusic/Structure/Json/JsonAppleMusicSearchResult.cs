using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResult
{
    [JsonProperty("results")]
    public JsonAppleMusicSearchResultResults SearchResults { get; set; }

    [JsonProperty("meta")]
    public JsonAppleMusicSearchResultMeta MetaData { get; set; }
}