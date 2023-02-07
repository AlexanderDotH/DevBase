using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetails
{
    [JsonProperty("error")]
    public List<object> error { get; set; }

    [JsonProperty("results")]
    public JsonDeezerSongDetailsResults results { get; set; }
}