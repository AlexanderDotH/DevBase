using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenAi.Json;

public class OpenAiTranscription
{
    [JsonProperty("task", NullValueHandling = NullValueHandling.Ignore)]
    public string task { get; set; }

    [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
    public string language { get; set; }

    [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
    public double duration { get; set; }

    [JsonProperty("segments", NullValueHandling = NullValueHandling.Ignore)]
    public List<OpenAiTranscriptionSegment> segments { get; set; }

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string text { get; set; }
}