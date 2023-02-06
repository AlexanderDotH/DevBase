using Newtonsoft.Json;

namespace DevBase.Format.Formats.MmlFormat.Json
{
    public class MmlElement
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("time")]
        public MmlElementTime Time { get; set; }
    }
}
