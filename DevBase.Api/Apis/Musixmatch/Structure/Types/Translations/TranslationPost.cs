using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public class TranslationPost
{
    [JsonProperty("originalIndex")]
    public string OriginalIndex { get; set; } = string.Empty;

    [JsonProperty("selected_language")]
    public string Language { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Translation { get; set; } = string.Empty;

    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("snippet")]
    public string SourceLine { get; set; } = string.Empty;
}
