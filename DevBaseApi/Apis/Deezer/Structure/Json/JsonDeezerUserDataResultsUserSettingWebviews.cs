using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingWebviews
{
    [JsonProperty("domain")]
    public bool domain { get; set; }
}