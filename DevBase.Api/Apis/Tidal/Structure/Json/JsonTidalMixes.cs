using Newtonsoft.Json;

namespace DevBase.Api.Apis.Tidal.Structure.Json
{
    public class JsonTidalMixes
    {
        [JsonProperty("MASTER_TRACK_MIX")]
        public string MASTERTRACKMIX { get; set; }

        [JsonProperty("TRACK_MIX")]
        public string TRACKMIX { get; set; }
    }
}