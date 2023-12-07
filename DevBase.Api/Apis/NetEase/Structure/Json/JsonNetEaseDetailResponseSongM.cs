using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseDetailResponseSongM
{
    [JsonProperty("br")]
    public int BitRate { get; set; }

    [JsonProperty("fid")]
    public int fid { get; set; }

    [JsonProperty("size")]
    public int size { get; set; }

    [JsonProperty("vd")]
    public int vd { get; set; }

    [JsonProperty("sr")]
    public int sr { get; set; }
}