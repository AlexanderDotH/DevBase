using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponse
{
    [JsonProperty("message")]
    public JsonMusixMatchAuthResponseMessage message { get; set; }
}