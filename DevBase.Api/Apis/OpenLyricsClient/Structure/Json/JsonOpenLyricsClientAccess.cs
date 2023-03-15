using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenLyricsClient.Structure.Json;

public class JsonOpenLyricsClientAccess
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }

    [JsonProperty("tokenType")]
    public string TokenType { get; set; }

    [JsonProperty("scope")]
    public List<string> Scope { get; set; }

    [JsonProperty("expiresIn")]
    public int ExpiresIn { get; set; }
}