using Newtonsoft.Json;

namespace DevBase.Api.Apis.Tidal.Structure.Json;

public class JsonTidalAuthAccess
{
    [JsonProperty("clientName")]
    public string clientName { get; set; }

    [JsonProperty("token_type")]
    public string token_type { get; set; }

    [JsonProperty("access_token")]
    public string access_token { get; set; }

    [JsonProperty("refresh_token")]
    public string refresh_token { get; set; }

    [JsonProperty("expires_in")]
    public int expires_in { get; set; }
}