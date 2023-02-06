using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerLyricsResponse
{
    [JsonProperty("data")]
    public JsonDeezerDataResponse data { get; set; }

    [JsonProperty("extensions")]
    public JsonDeezerExtensionsResponse extensions { get; set; }
}