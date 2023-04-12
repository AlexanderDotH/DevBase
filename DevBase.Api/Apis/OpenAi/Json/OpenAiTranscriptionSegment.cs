using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenAi.Json;

public class OpenAiTranscriptionSegment
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public int id { get; set; }

    [JsonProperty("seek", NullValueHandling = NullValueHandling.Ignore)]
    public int seek { get; set; }

    [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
    public double start { get; set; }

    [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
    public double end { get; set; }

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string text { get; set; }

    [JsonProperty("tokens", NullValueHandling = NullValueHandling.Ignore)]
    public List<int> tokens { get; set; }

    [JsonProperty("temperature", NullValueHandling = NullValueHandling.Ignore)]
    public double temperature { get; set; }

    [JsonProperty("avg_logprob", NullValueHandling = NullValueHandling.Ignore)]
    public double avg_logprob { get; set; }

    [JsonProperty("compression_ratio", NullValueHandling = NullValueHandling.Ignore)]
    public double compression_ratio { get; set; }

    [JsonProperty("no_speech_prob", NullValueHandling = NullValueHandling.Ignore)]
    public double no_speech_prob { get; set; }

    [JsonProperty("transient", NullValueHandling = NullValueHandling.Ignore)]
    public bool transient { get; set; }
}