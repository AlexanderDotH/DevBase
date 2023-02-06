using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSetting
{
    [JsonProperty("global")]
    public JsonDeezerUserDataResultsUserSettingGlobal global { get; set; }

    [JsonProperty("site")]
    public JsonDeezerUserDataResultsUserSettingSite site { get; set; }

    [JsonProperty("twitter")]
    public JsonDeezerUserDataResultsUserSettingTwitter twitter { get; set; }

    [JsonProperty("facebook")]
    public JsonDeezerUserDataResultsUserSettingFacebook facebook { get; set; }

    [JsonProperty("google")]
    public JsonDeezerUserDataResultsUserSettingGoogle google { get; set; }

    [JsonProperty("notification_mail")]
    public JsonDeezerUserDataResultsUserSettingNotificationMail notification_mail { get; set; }

    [JsonProperty("notification_mobile")]
    public JsonDeezerUserDataResultsUserSettingNotificationMobile notification_mobile { get; set; }

    [JsonProperty("newsletter")]
    public JsonDeezerUserDataResultsUserSettingNewsletter newsletter { get; set; }

    [JsonProperty("optin_mail")]
    public List<object> optin_mail { get; set; }

    [JsonProperty("optin_push")]
    public List<object> optin_push { get; set; }

    [JsonProperty("optin_inapp")]
    public List<object> optin_inapp { get; set; }

    [JsonProperty("optin_sms")]
    public List<object> optin_sms { get; set; }

    [JsonProperty("beta_user")]
    public JsonDeezerUserDataResultsUserSettingBetaUser beta_user { get; set; }

    [JsonProperty("tips")]
    public JsonDeezerUserDataResultsUserSettingTips tips { get; set; }

    [JsonProperty("audio_quality_settings")]
    public JsonDeezerUserDataResultsUserSettingAudioQualitySettings audio_quality_settings { get; set; }

    [JsonProperty("customer_message")]
    public List<object> customer_message { get; set; }

    [JsonProperty("mobile_message")]
    public List<object> mobile_message { get; set; }

    [JsonProperty("adjust")]
    public List<object> adjust { get; set; }

    [JsonProperty("abtest")]
    public List<object> abtest { get; set; }

    [JsonProperty("ads")]
    public JsonDeezerUserDataResultsUserSettingAds ads { get; set; }

    [JsonProperty("webviews")]
    public JsonDeezerUserDataResultsUserSettingWebviews webviews { get; set; }

    [JsonProperty("tracking")]
    public List<object> tracking { get; set; }
}