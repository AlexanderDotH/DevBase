using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseLyricResponse
{
    [JsonProperty("sgc")]
    public bool sgc { get; set; }

    [JsonProperty("sfy")]
    public bool sfy { get; set; }

    [JsonProperty("qfy")]
    public bool qfy { get; set; }

    [JsonProperty("transUser")]
    public JsonNetEaseLyricResponseTransUser transUser { get; set; }

    [JsonProperty("lyricUser")]
    public JsonNetEaseLyricResponseLyricUser lyricUser { get; set; }

    [JsonProperty("lrc")]
    public JsonNetEaseLyricResponseLrc lrc { get; set; }

    [JsonProperty("klyric")]
    public JsonNetEaseLyricResponseKlyric klyric { get; set; }

    [JsonProperty("tlyric")]
    public JsonNetEaseLyricResponseTlyric tlyric { get; set; }

    [JsonProperty("romalrc")]
    public JsonNetEaseLyricResponseRomalrc romalrc { get; set; }

    [JsonProperty("code")]
    public int code { get; set; }
}