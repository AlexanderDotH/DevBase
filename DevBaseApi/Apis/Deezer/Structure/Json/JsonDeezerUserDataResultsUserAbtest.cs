using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserAbtest
{
    [JsonProperty("discovery_algorithms")]
    public JsonDeezerUserDataResultsUserAbtestDiscoveryAlgorithms discovery_algorithms { get; set; }

    [JsonProperty("premium_tab_native")]
    public JsonDeezerUserDataResultsUserAbtestPremiumTabNative premium_tab_native { get; set; }

    [JsonProperty("share_android_image_preview")]
    public JsonDeezerUserDataResultsUserAbtestShareAndroidImagePreview share_android_image_preview { get; set; }

    [JsonProperty("screenshot_sharing")]
    public JsonDeezerUserDataResultsUserAbtestScreenshotSharing screenshot_sharing { get; set; }

    [JsonProperty("botmanager_0721")]
    public JsonDeezerUserDataResultsUserAbtestBotmanager0721 botmanager_0721 { get; set; }

    [JsonProperty("restriction_boxes_vs_paywall")]
    public JsonDeezerUserDataResultsUserAbtestRestrictionBoxesVsPaywall restriction_boxes_vs_paywall { get; set; }

    [JsonProperty("mobile_trial")]
    public JsonDeezerUserDataResultsUserAbtestMobileTrial mobile_trial { get; set; }

    [JsonProperty("tnb_reminder")]
    public JsonDeezerUserDataResultsUserAbtestTnbReminder tnb_reminder { get; set; }
}