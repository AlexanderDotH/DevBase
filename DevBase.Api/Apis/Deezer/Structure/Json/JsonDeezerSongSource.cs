using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSongSource
{
    [JsonProperty("data")]
    public List<JsonDeezerSongSourceData> data { get; set; }
}