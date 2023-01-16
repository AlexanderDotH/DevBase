using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerLyricsTrackResponseLyricsSynchronizedLineResponse
{
    [JsonProperty("lrcTimestamp")]
    public string lrcTimestamp { get; set; }

    [JsonProperty("line")]
    public string line { get; set; }

    [JsonProperty("lineTranslated")]
    public object lineTranslated { get; set; }

    [JsonProperty("milliseconds")]
    public int milliseconds { get; set; }

    [JsonProperty("duration")]
    public int duration { get; set; }

    [JsonProperty("__typename")]
    public string __typename { get; set; }
}