using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DevBaseFormat.Formats.MmlFormat.Json
{
    public class MmlElement
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("time")]
        public MmlElementTime Time { get; set; }
    }
}
