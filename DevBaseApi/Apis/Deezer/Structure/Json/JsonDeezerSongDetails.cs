using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetails
{
    [JsonProperty("error")]
    public List<object> error { get; set; }

    [JsonProperty("results")]
    public JsonDeezerSongDetailsResults results { get; set; }
}