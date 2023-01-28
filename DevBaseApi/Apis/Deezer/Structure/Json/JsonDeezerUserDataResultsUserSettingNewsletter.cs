using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingNewsletter
{
    [JsonProperty("editor")]
    public bool editor { get; set; }

    [JsonProperty("talk")]
    public bool talk { get; set; }

    [JsonProperty("event")]
    public bool @event { get; set; }

    [JsonProperty("game")]
    public bool game { get; set; }

    [JsonProperty("special_offer")]
    public bool special_offer { get; set; }

    [JsonProperty("panel")]
    public bool panel { get; set; }
}