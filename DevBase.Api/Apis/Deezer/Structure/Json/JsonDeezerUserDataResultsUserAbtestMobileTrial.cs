using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserAbtestMobileTrial
{
    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("option")]
    public string option { get; set; }

    [JsonProperty("behaviour")]
    public string behaviour { get; set; }
}