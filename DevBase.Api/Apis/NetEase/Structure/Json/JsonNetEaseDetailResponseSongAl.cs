using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseDetailResponseSongAl
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("picUrl")]
    public string picUrl { get; set; }

    [JsonProperty("tns")]
    public List<string> tns { get; set; }

    [JsonProperty("pic_str")]
    public string pic_str { get; set; }

    [JsonProperty("pic")]
    public long pic { get; set; }
}