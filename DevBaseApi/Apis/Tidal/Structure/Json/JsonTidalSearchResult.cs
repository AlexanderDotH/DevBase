using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBaseApi.Apis.Tidal.Structure.Json
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
