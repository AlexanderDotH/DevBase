using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsThirdPartyBraze
{
    [JsonProperty("apiKey")]
    public string apiKey { get; set; }

    [JsonProperty("isAvailable")]
    public bool isAvailable { get; set; }
}