using Newtonsoft.Json;

namespace DevBase.Api.Apis.Tidal.Structure.Json
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
