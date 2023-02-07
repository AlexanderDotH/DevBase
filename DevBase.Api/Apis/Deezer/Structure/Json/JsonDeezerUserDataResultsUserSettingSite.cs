using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingSite
{
    [JsonProperty("livebar_state")]
    public string livebar_state { get; set; }

    [JsonProperty("livebar_tab")]
    public string livebar_tab { get; set; }

    [JsonProperty("push_mobile")]
    public int push_mobile { get; set; }

    [JsonProperty("howto_step")]
    public int howto_step { get; set; }

    [JsonProperty("edito_tag")]
    public int edito_tag { get; set; }

    [JsonProperty("display_confirm_discovery")]
    public int display_confirm_discovery { get; set; }

    [JsonProperty("cast_audio_quality")]
    public string cast_audio_quality { get; set; }
}