using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageHeader
{
    [JsonProperty("status_code")]
    public int status_code { get; set; }

    [JsonProperty("execute_time")]
    public double execute_time { get; set; }

    [JsonProperty("pid")]
    public int pid { get; set; }

    [JsonProperty("surrogate_key_list")]
    public List<object> surrogate_key_list { get; set; }
}