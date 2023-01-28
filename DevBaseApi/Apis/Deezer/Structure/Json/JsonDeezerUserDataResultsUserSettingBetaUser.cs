using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingBetaUser
{
    [JsonProperty("ios")]
    public bool ios { get; set; }

    [JsonProperty("android")]
    public bool android { get; set; }

    [JsonProperty("windowsphone")]
    public bool windowsphone { get; set; }

    [JsonProperty("windows")]
    public bool windows { get; set; }
}