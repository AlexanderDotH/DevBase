using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBaseApi.Apis.Tidal.Structure.Json
{
    public class JsonTidalSessionClient
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("authorizedForOffline")]
        public bool AuthorizedForOffline { get; set; }

        [JsonProperty("authorizedForOfflineDate")]
        public object AuthorizedForOfflineDate { get; set; }
    }
}
