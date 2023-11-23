using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageBodyCredentialAccountExtraParams
{
    [JsonProperty("mode")]
    public string mode { get; set; }

    [JsonProperty("redirect_url")]
    public string redirect_url { get; set; }
}