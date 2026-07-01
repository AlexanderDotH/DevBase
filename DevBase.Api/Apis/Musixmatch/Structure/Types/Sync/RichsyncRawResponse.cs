using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class RichsyncRawResponse
{
    [JsonProperty("richsync_id")]
    public int RichsyncId { get; set; }

    [JsonProperty("restricted")]
    public int Restricted { get; set; }

    [JsonProperty("richsync_body")]
    public string RichsyncBody { get; set; } = string.Empty;

    [JsonProperty("lyrics_copyright")]
    public string LyricsCopyright { get; set; } = string.Empty;

    [JsonProperty("richsync_length")]
    public int RichsyncLength { get; set; }

    [JsonProperty("richsync_language")]
    public string RichsyncLanguage { get; set; } = string.Empty;

    [JsonProperty("richsync_language_description")]
    public string RichsyncLanguageDescription { get; set; } = string.Empty;

    [JsonProperty("script_tracking_url")]
    public string ScriptTrackingUrl { get; set; } = string.Empty;

    [JsonProperty("updated_time")]
    public DateTime? UpdatedTime { get; set; }
}
