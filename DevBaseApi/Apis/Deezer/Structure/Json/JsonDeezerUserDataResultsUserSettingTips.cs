using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingTips
{
    [JsonProperty("player")]
    public bool player { get; set; }

    [JsonProperty("flow")]
    public bool flow { get; set; }

    [JsonProperty("add_to_library")]
    public bool add_to_library { get; set; }

    [JsonProperty("lyrics")]
    public int lyrics { get; set; }

    [JsonProperty("up_next")]
    public bool up_next { get; set; }
}