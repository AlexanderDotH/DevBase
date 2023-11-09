using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerRawLyricsResponse
{
    [JsonProperty("error")]
    public List<object> error { get; set; }

    [JsonProperty("results")]
    public JsonDeezerRawLyricsResponseResults results { get; set; }
}