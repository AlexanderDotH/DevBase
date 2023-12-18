using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicLyricsResponseDataAttributesPlayParams
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("kind")]
    public string Kind { get; set; }

    [JsonProperty("catalogId")]
    public string CatalogId { get; set; }

    [JsonProperty("displayType")]
    public int DisplayType { get; set; }
}