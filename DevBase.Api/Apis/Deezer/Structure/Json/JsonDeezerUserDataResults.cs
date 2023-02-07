using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResults
{
        [JsonProperty("USER")]
        public JsonDeezerUserDataResultsUser USER { get; set; }

        [JsonProperty("SETTING_LANG")]
        public string SETTING_LANG { get; set; }

        [JsonProperty("SETTING_LOCALE")]
        public string SETTING_LOCALE { get; set; }

        [JsonProperty("DIRECTION")]
        public string DIRECTION { get; set; }

        [JsonProperty("SESSION_ID")]
        public string SESSION_ID { get; set; }

        [JsonProperty("USER_TOKEN")]
        public string USER_TOKEN { get; set; }

        [JsonProperty("PLAYLIST_WELCOME_ID")]
        public string PLAYLIST_WELCOME_ID { get; set; }

        [JsonProperty("OFFER_ID")]
        public int OFFER_ID { get; set; }

        [JsonProperty("OFFER_NAME")]
        public string OFFER_NAME { get; set; }

        [JsonProperty("OFFER_ELIGIBILITIES")]
        public List<object> OFFER_ELIGIBILITIES { get; set; }

        [JsonProperty("COUNTRY")]
        public string COUNTRY { get; set; }

        [JsonProperty("COUNTRY_CATEGORY")]
        public string COUNTRY_CATEGORY { get; set; }

        [JsonProperty("MIN_LEGAL_AGE")]
        public int MIN_LEGAL_AGE { get; set; }

        [JsonProperty("FAMILY_KIDS_AGE")]
        public int FAMILY_KIDS_AGE { get; set; }

        [JsonProperty("SERVER_TIMESTAMP")]
        public int SERVER_TIMESTAMP { get; set; }

        [JsonProperty("PLAYER_TOKEN")]
        public string PLAYER_TOKEN { get; set; }

        [JsonProperty("checkForm")]
        public string checkForm { get; set; }

        [JsonProperty("FROM_ONBOARDING")]
        public string FROM_ONBOARDING { get; set; }

        [JsonProperty("CUSTO")]
        public string CUSTO { get; set; }

        [JsonProperty("SETTING_REFERER_UPLOAD")]
        public string SETTING_REFERER_UPLOAD { get; set; }

        [JsonProperty("REG_FLOW")]
        public List<string> REG_FLOW { get; set; }

        [JsonProperty("LOGIN_FLOW")]
        public List<string> LOGIN_FLOW { get; set; }

        [JsonProperty("__DZR_GATEKEEPS__")]
        public JsonDeezerUserDataResultsDZRGATEKEEPS __DZR_GATEKEEPS__ { get; set; }

        [JsonProperty("thirdParty")]
        public JsonDeezerUserDataResultsThirdParty thirdParty { get; set; }

        [JsonProperty("URL_MEDIA")]
        public string URL_MEDIA { get; set; }

        [JsonProperty("GAIN")]
        public JsonDeezerUserDataResultsGain GAIN { get; set; }

        [JsonProperty("checkFormLogin")]
        public string checkFormLogin { get; set; }
}