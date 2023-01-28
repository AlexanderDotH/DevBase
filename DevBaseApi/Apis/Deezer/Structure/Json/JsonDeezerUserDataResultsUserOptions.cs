using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserOptions
{
    [JsonProperty("mobile_preview")]
        public bool mobile_preview { get; set; }

        [JsonProperty("mobile_radio")]
        public bool mobile_radio { get; set; }

        [JsonProperty("mobile_streaming")]
        public bool mobile_streaming { get; set; }

        [JsonProperty("mobile_streaming_duration")]
        public int mobile_streaming_duration { get; set; }

        [JsonProperty("mobile_offline")]
        public bool mobile_offline { get; set; }

        [JsonProperty("mobile_sound_quality")]
        public JsonDeezerUserDataResultsUserOptionsMobileSoundQuality mobile_sound_quality { get; set; }

        [JsonProperty("default_download_on_mobile_network")]
        public bool default_download_on_mobile_network { get; set; }

        [JsonProperty("mobile_hq")]
        public bool mobile_hq { get; set; }

        [JsonProperty("mobile_lossless")]
        public bool mobile_lossless { get; set; }

        [JsonProperty("tablet_sound_quality")]
        public JsonDeezerUserDataResultsUserOptionsTabletSoundQuality tablet_sound_quality { get; set; }

        [JsonProperty("audio_quality_default_preset")]
        public string audio_quality_default_preset { get; set; }

        [JsonProperty("web_preview")]
        public bool web_preview { get; set; }

        [JsonProperty("web_radio")]
        public bool web_radio { get; set; }

        [JsonProperty("web_streaming")]
        public bool web_streaming { get; set; }

        [JsonProperty("web_streaming_duration")]
        public int web_streaming_duration { get; set; }

        [JsonProperty("web_offline")]
        public bool web_offline { get; set; }

        [JsonProperty("web_hq")]
        public bool web_hq { get; set; }

        [JsonProperty("web_lossless")]
        public bool web_lossless { get; set; }

        [JsonProperty("web_sound_quality")]
        public JsonDeezerUserDataResultsUserOptionsWebSoundQuality web_sound_quality { get; set; }

        [JsonProperty("license_token")]
        public string license_token { get; set; }

        [JsonProperty("expiration_timestamp")]
        public int expiration_timestamp { get; set; }

        [JsonProperty("license_country")]
        public string license_country { get; set; }

        [JsonProperty("ads_display")]
        public bool ads_display { get; set; }

        [JsonProperty("ads_audio")]
        public bool ads_audio { get; set; }

        [JsonProperty("dj")]
        public bool dj { get; set; }

        [JsonProperty("nb_devices")]
        public int nb_devices { get; set; }

        [JsonProperty("multi_account")]
        public bool multi_account { get; set; }

        [JsonProperty("multi_account_max_allowed")]
        public int multi_account_max_allowed { get; set; }

        [JsonProperty("radio_skips")]
        public int radio_skips { get; set; }

        [JsonProperty("too_many_devices")]
        public bool too_many_devices { get; set; }

        [JsonProperty("business")]
        public bool business { get; set; }

        [JsonProperty("business_mod")]
        public bool business_mod { get; set; }

        [JsonProperty("business_box_owner")]
        public bool business_box_owner { get; set; }

        [JsonProperty("business_box_manager")]
        public bool business_box_manager { get; set; }

        [JsonProperty("business_box")]
        public bool business_box { get; set; }

        [JsonProperty("business_no_right")]
        public bool business_no_right { get; set; }

        [JsonProperty("allow_subscription")]
        public bool allow_subscription { get; set; }

        [JsonProperty("allow_trial_mobile")]
        public bool allow_trial_mobile { get; set; }

        [JsonProperty("timestamp")]
        public int timestamp { get; set; }

        [JsonProperty("can_subscribe")]
        public bool can_subscribe { get; set; }

        [JsonProperty("can_subscribe_family")]
        public bool can_subscribe_family { get; set; }

        [JsonProperty("show_subscription_section")]
        public bool show_subscription_section { get; set; }

        [JsonProperty("streaming_group")]
        public string streaming_group { get; set; }

        [JsonProperty("queuelist_edition")]
        public bool queuelist_edition { get; set; }

        [JsonProperty("web_streaming_used")]
        public int web_streaming_used { get; set; }

        [JsonProperty("ads")]
        public bool ads { get; set; }
}