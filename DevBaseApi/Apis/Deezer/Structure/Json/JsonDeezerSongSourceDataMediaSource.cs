using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongSourceDataMediaSource
{
    [JsonProperty("provider")]
    public string provider { get; set; }

    [JsonProperty("url")]
    public string url { get; set; }
}