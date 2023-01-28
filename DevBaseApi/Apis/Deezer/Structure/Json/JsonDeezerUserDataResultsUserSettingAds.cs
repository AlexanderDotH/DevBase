using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingAds
{
    [JsonProperty("test_format")]
    public bool test_format { get; set; }

    [JsonProperty("force_adsource")]
    public string force_adsource { get; set; }

    [JsonProperty("force_mediation")]
    public string force_mediation { get; set; }
}