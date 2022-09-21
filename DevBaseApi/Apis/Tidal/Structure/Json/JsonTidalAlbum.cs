using Newtonsoft.Json;

namespace DevBaseApi.Apis.Tidal.Structure.Json
{
    public class JsonTidalAlbum
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("vibrantColor")]
        public string VibrantColor { get; set; }

        [JsonProperty("videoCover")]
        public object VideoCover { get; set; }
    }
}