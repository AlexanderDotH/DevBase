using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicLyricsResponseData
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("attributes")]
    public JsonAppleMusicLyricsResponseDataAttributes Attributes { get; set; }
}