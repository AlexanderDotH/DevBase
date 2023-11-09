using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerRawLyricsResponseResultsSync
{
    [JsonProperty("lrc_timestamp")]
    public string lrc_timestamp { get; set; }

    [JsonProperty("milliseconds")]
    public string milliseconds { get; set; }

    [JsonProperty("duration")]
    public string duration { get; set; }

    [JsonProperty("line")]
    public string line { get; set; }
}