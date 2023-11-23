using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageBodyAuth
{
    [JsonProperty("user_id")]
    public string user_id { get; set; }

    [JsonProperty("authorizations")]
    public JsonMusixMatchAuthResponseMessageBodyAuthAuthorizations authorizations { get; set; }
}