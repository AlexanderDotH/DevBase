using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsISRC
{
    [JsonProperty("data")]
    public List<JsonDeezerSongDetailsResultsISRCData> data { get; set; }

    [JsonProperty("count")]
    public int count { get; set; }

    [JsonProperty("total")]
    public int total { get; set; }
}