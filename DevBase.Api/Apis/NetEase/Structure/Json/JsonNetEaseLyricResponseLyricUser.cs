using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseLyricResponseLyricUser
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("status")]
    public int status { get; set; }

    [JsonProperty("demand")]
    public int demand { get; set; }

    [JsonProperty("userid")]
    public int userid { get; set; }

    [JsonProperty("nickname")]
    public string nickname { get; set; }

    [JsonProperty("uptime")]
    public long uptime { get; set; }
}