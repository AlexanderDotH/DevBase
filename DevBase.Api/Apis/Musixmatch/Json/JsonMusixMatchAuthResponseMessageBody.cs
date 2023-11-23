using System.Text.Json.Serialization;
using DevBase.Web.RequestData.Data;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageBody
{
    [JsonProperty("tokens")]
    public JsonMusixMatchAuthResponseMessageBodyTokens tokens { get; set; }

    [JsonProperty("credential")]
    public JsonMusixMatchAuthResponseMessageBodyCredential credential { get; set; }

    [JsonProperty("auth")]
    public JsonMusixMatchAuthResponseMessageBodyAuth auth { get; set; }
}