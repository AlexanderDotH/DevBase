using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSearchResponse
{
    [JsonProperty("data")]
    public List<JsonDeezerSearchDataResponse> data { get; set; }

    [JsonProperty("total")]
    public int total { get; set; }
}