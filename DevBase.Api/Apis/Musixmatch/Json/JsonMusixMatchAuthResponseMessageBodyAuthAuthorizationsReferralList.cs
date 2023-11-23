using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageBodyAuthAuthorizationsReferralList
{
    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("active")]
    public bool active { get; set; }

    [JsonProperty("code")]
    public string code { get; set; }

    [JsonProperty("invitation")]
    public string invitation { get; set; }
}