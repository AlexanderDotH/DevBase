using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessage
{
    [JsonProperty("header")]
    public JsonMusixMatchAuthResponseMessageHeader header { get; set; }

    [JsonProperty("body")]
    public JsonMusixMatchAuthResponseMessageBody body { get; set; }
}