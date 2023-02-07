using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSongSourceData
{
    [JsonProperty("media")]
    public List<JsonDeezerSongSourceDataMedia> media { get; set; }
}