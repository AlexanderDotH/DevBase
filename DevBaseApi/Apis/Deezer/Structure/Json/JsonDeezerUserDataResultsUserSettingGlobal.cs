using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingGlobal
{
    [JsonProperty("language")]
    public string language { get; set; }

    [JsonProperty("social")]
    public bool social { get; set; }

    [JsonProperty("popup_unload")]
    public bool popup_unload { get; set; }

    [JsonProperty("filter_explicit_lyrics")]
    public bool filter_explicit_lyrics { get; set; }

    [JsonProperty("is_kid")]
    public bool is_kid { get; set; }

    [JsonProperty("has_up_next")]
    public bool has_up_next { get; set; }

    [JsonProperty("dark_mode")]
    public string dark_mode { get; set; }

    [JsonProperty("onboarding")]
    public bool onboarding { get; set; }

    [JsonProperty("onboarding_progress")]
    public int onboarding_progress { get; set; }

    [JsonProperty("cookie_consent_string")]
    public string cookie_consent_string { get; set; }

    [JsonProperty("has_root_consent")]
    public int has_root_consent { get; set; }

    [JsonProperty("happy_hour")]
    public string happy_hour { get; set; }

    [JsonProperty("recommendation_country")]
    public string recommendation_country { get; set; }

    [JsonProperty("has_joined_family")]
    public bool has_joined_family { get; set; }

    [JsonProperty("has_already_tried_premium")]
    public bool has_already_tried_premium { get; set; }

    [JsonProperty("explicit_level_forced")]
    public bool explicit_level_forced { get; set; }
}