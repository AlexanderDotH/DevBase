using Newtonsoft.Json;

namespace DevBase.Api.Apis.Replicate.Structure;

public class ReplicatePredictionResultOutputSegment
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("end")]
    public int end { get; set; }

    [JsonProperty("seek")]
    public int seek { get; set; }

    [JsonProperty("text")]
    public string text { get; set; }

    [JsonProperty("start")]
    public int start { get; set; }

    [JsonProperty("tokens")]
    public List<int> tokens { get; set; }

    [JsonProperty("avg_logprob")]
    public double avg_logprob { get; set; }

    [JsonProperty("temperature")]
    public int temperature { get; set; }

    [JsonProperty("no_speech_prob")]
    public double no_speech_prob { get; set; }

    [JsonProperty("compression_ratio")]
    public double compression_ratio { get; set; }
}