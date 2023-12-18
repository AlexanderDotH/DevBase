using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicLyricsResponseDataAttributes
{
    [JsonProperty("ttml")]
    public string Ttml { get; set; }

    [JsonProperty("playParams")]
    public JsonAppleMusicLyricsResponseDataAttributesPlayParams PlayParams { get; set; }
}