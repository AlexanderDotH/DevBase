using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseDetailResponse
{
    [JsonProperty("songs")]
    public List<JsonNetEaseDetailResponseSong> songs { get; set; }

    [JsonProperty("privileges")]
    public List<JsonNetEaseDetailResponsePrivilege> privileges { get; set; }

    [JsonProperty("code")]
    public int code { get; set; }
}