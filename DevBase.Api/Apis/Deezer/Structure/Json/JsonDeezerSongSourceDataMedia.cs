using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSongSourceDataMedia
{
    [JsonProperty("cipher")]
    public JsonDeezerSongSourceDataMediaCipher cipher { get; set; }

    [JsonProperty("exp")]
    public int exp { get; set; }

    [JsonProperty("format")]
    public string format { get; set; }

    [JsonProperty("media_type")]
    public string media_type { get; set; }

    [JsonProperty("nbf")]
    public int nbf { get; set; }

    [JsonProperty("sources")]
    public List<JsonDeezerSongSourceDataMediaSource> sources { get; set; }
}