using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsThirdParty
{
    [JsonProperty("facebook")]
    public JsonDeezerUserDataResultsThirdPartyFacebook facebook { get; set; }

    [JsonProperty("googleplus")]
    public object googleplus { get; set; }

    [JsonProperty("braze")]
    public JsonDeezerUserDataResultsThirdPartyBraze braze { get; set; }
}