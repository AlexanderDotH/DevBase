using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserOptIns
{
    [JsonProperty("channel")]
    public List<object> channel { get; set; }

    [JsonProperty("optin")]
    public List<object> optin { get; set; }

    [JsonProperty("group")]
    public List<object> group { get; set; }
}