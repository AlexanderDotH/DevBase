using Newtonsoft.Json;

namespace DevBase.Api.Apis.Tidal.Structure.Json
{
    public class JsonTidalSession
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("channelId")]
        public int ChannelId { get; set; }

        [JsonProperty("partnerId")]
        public int PartnerId { get; set; }

        [JsonProperty("client")]
        public JsonTidalSessionClient Client { get; set; }
    }
}
