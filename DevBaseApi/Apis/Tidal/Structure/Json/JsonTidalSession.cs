using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBaseApi.Apis.Tidal.Structure.Json
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
