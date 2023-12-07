using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseDetailResponseSongAr
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("tns")]
    public List<object> tns { get; set; }

    [JsonProperty("alias")]
    public List<object> alias { get; set; }
}