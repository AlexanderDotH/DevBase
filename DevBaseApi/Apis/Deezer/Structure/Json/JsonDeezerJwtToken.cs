using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerJwtToken
{
    [JsonProperty("jwt")]
    public string jwt { get; set; }

    [JsonProperty("refresh_token")]
    public string refresh_token { get; set; }
}