using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicLyricsResponse
{
    [JsonProperty("data")]
    public List<JsonAppleMusicLyricsResponseData> Data { get; set; }
}