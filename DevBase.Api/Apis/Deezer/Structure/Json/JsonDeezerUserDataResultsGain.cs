using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsGain
{
    [JsonProperty("TARGET")]
    public string TARGET { get; set; }

    [JsonProperty("ADS")]
    public string ADS { get; set; }
}