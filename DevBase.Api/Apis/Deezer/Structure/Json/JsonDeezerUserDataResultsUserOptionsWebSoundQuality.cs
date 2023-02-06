using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserOptionsWebSoundQuality
{
    [JsonProperty("low")]
    public bool low { get; set; }

    [JsonProperty("standard")]
    public bool standard { get; set; }

    [JsonProperty("high")]
    public bool high { get; set; }

    [JsonProperty("lossless")]
    public bool lossless { get; set; }

    [JsonProperty("reality")]
    public bool reality { get; set; }
}