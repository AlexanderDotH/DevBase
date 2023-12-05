using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseLyricResponseKlyric
{
    [JsonProperty("version")]
    public int version { get; set; }

    [JsonProperty("lyric")]
    public string lyric { get; set; }
}