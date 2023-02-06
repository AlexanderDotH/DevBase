using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenLyricsClient.Structure.Json;

public class JsonOpenLyricsClientAccess
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    [JsonProperty("scope")]
    public List<string> Scope { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
}