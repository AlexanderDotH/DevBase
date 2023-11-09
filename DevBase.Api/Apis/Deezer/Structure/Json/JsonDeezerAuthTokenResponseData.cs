using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerAuthTokenResponseData
{
    [JsonProperty("type")]
    public string type { get; set; }

    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("attributes")]
    public JsonDeezerAuthTokenResponseAttributes attributes { get; set; }
}