using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingWebviews
{
    [JsonProperty("domain")]
    public bool domain { get; set; }
}