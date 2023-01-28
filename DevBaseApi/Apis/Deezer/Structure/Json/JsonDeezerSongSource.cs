using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongSource
{
    [JsonProperty("data")]
    public List<JsonDeezerSongSourceData> data { get; set; }
}