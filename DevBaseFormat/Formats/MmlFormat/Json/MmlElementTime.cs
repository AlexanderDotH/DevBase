using Newtonsoft.Json;

namespace DevBaseFormat.Formats.MmlFormat.Json
{
    public class MmlElementTime
    {
        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("minutes")]
        public long Minutes { get; set; }

        [JsonProperty("seconds")]
        public long Seconds { get; set; }

        [JsonProperty("hundredths")]
        public long Hundredths { get; set; }
    }
}