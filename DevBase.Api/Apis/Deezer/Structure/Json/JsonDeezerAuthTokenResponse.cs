using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerAuthTokenResponse
{
    [JsonProperty("data")]
    public JsonDeezerAuthTokenResponseData data { get; set; }
}