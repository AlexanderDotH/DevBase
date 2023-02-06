using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerSongDetailsResultsDataRights
{
    [JsonProperty("STREAM_ADS_AVAILABLE")]
    public bool STREAM_ADS_AVAILABLE { get; set; }

    [JsonProperty("STREAM_ADS")]
    public string STREAM_ADS { get; set; }

    [JsonProperty("STREAM_SUB_AVAILABLE")]
    public bool STREAM_SUB_AVAILABLE { get; set; }

    [JsonProperty("STREAM_SUB")]
    public string STREAM_SUB { get; set; }
}