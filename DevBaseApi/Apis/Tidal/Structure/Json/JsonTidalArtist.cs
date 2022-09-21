using Newtonsoft.Json;

namespace DevBaseApi.Apis.Tidal.Structure.Json
{
    public class JsonTidalArtist
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }
    }
}