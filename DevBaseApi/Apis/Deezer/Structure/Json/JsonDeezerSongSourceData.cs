using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongSourceData
{
    [JsonProperty("media")]
    public List<JsonDeezerSongSourceDataMedia> media { get; set; }
}