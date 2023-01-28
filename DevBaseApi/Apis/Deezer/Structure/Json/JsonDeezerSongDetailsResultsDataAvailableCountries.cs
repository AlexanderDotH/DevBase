using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsDataAvailableCountries
{
    [JsonProperty("STREAM_ADS")]
    public List<string> STREAM_ADS { get; set; }

    [JsonProperty("STREAM_SUB_ONLY")]
    public List<object> STREAM_SUB_ONLY { get; set; }
}