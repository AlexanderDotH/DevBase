using Newtonsoft.Json;

namespace DevBase.Api.Apis.Tidal.Structure.Json
{
    public class JsonTidalSearchResult
    {
        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("totalNumberOfItems")]
        public int TotalNumberOfItems { get; set; }

        [JsonProperty("items")]
        public List<JsonTidalTrack> Items { get; set; }
    }
}
