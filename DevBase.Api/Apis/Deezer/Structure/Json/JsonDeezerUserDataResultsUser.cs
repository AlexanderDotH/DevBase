using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUser
{
     [JsonProperty("USER_ID")]
        public int USER_ID { get; set; }

        [JsonProperty("USER_PICTURE")]
        public string USER_PICTURE { get; set; }

        [JsonProperty("INSCRIPTION_DATE")]
        public string INSCRIPTION_DATE { get; set; }

        [JsonProperty("TRY_AND_BUY")]
        public JsonDeezerUserDataResultsUserTryAndBuy TRY_AND_BUY { get; set; }

        [JsonProperty("PARTNERS")]
        public string PARTNERS { get; set; }

        [JsonProperty("TOOLBAR")]
        public List<object> TOOLBAR { get; set; }

        [JsonProperty("OPTIONS")]
        public JsonDeezerUserDataResultsUserOptions OPTIONS { get; set; }

        [JsonProperty("AUDIO_SETTINGS")]
        public JsonDeezerUserDataResultsUserAudioSettings AUDIO_SETTINGS { get; set; }

        [JsonProperty("SETTING")]
        public JsonDeezerUserDataResultsUserSetting SETTING { get; set; }

        [JsonProperty("LASTFM")]
        public object LASTFM { get; set; }

        [JsonProperty("TWITTER")]
        public object TWITTER { get; set; }

        [JsonProperty("FACEBOOK")]
        public object FACEBOOK { get; set; }

        [JsonProperty("GOOGLEPLUS")]
        public object GOOGLEPLUS { get; set; }

        [JsonProperty("FAVORITE_TAG")]
        public int FAVORITE_TAG { get; set; }

        [JsonProperty("ABTEST")]
        public JsonDeezerUserDataResultsUserAbtest ABTEST { get; set; }

        [JsonProperty("MULTI_ACCOUNT")]
        public List<object> MULTI_ACCOUNT { get; set; }

        [JsonProperty("ONBOARDING_MODAL")]
        public bool ONBOARDING_MODAL { get; set; }

        [JsonProperty("ADS_OFFER")]
        public string ADS_OFFER { get; set; }

        [JsonProperty("ENTRYPOINTS")]
        public object ENTRYPOINTS { get; set; }

        [JsonProperty("ADS_TEST_FORMAT")]
        public string ADS_TEST_FORMAT { get; set; }

        [JsonProperty("NEW_USER")]
        public bool NEW_USER { get; set; }

        [JsonProperty("CONSENT_STRING")]
        public List<object> CONSENT_STRING { get; set; }

        [JsonProperty("RECOMMENDATION_COUNTRY")]
        public string RECOMMENDATION_COUNTRY { get; set; }

        [JsonProperty("CAN_BE_CONVERTED_TO_INDEPENDENT")]
        public bool CAN_BE_CONVERTED_TO_INDEPENDENT { get; set; }

        [JsonProperty("IS_FREEMIUM_COUNTRY")]
        public int IS_FREEMIUM_COUNTRY { get; set; }

        [JsonProperty("EXPLICIT_CONTENT_LEVEL")]
        public string EXPLICIT_CONTENT_LEVEL { get; set; }

        [JsonProperty("EXPLICIT_CONTENT_LEVELS_AVAILABLE")]
        public List<string> EXPLICIT_CONTENT_LEVELS_AVAILABLE { get; set; }

        [JsonProperty("CAN_EDIT_EXPLICIT_CONTENT_LEVEL")]
        public bool CAN_EDIT_EXPLICIT_CONTENT_LEVEL { get; set; }

        [JsonProperty("DEVICES_COUNT")]
        public int DEVICES_COUNT { get; set; }

        [JsonProperty("HAS_UPNEXT")]
        public bool HAS_UPNEXT { get; set; }

        [JsonProperty("LOVEDTRACKS_ID")]
        public string LOVEDTRACKS_ID { get; set; }

        [JsonProperty("OPTINS")]
        public JsonDeezerUserDataResultsUserOptIns OPTINS { get; set; }
}