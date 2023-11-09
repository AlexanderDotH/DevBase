using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerAuthTokenResponseAttributes
{
    [JsonProperty("userId")]
    public int userId { get; set; }

    [JsonProperty("token")]
    public string token { get; set; }
}