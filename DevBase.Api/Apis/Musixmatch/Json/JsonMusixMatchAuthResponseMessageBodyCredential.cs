using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageBodyCredential
{
    [JsonProperty("action")]
    public string action { get; set; }

    [JsonProperty("email")]
    public string email { get; set; }

    [JsonProperty("type")]
    public string type { get; set; }

    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("account")]
    public JsonMusixMatchAuthResponseMessageBodyCredentialAccount account { get; set; }
}