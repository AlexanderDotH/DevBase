using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class SubtitleRawResponse
{
    [JsonProperty("subtitle_id")]
    public int SubtitleId { get; set; }

    [JsonProperty("restricted")]
    public int Restricted { get; set; }

    [JsonProperty("subtitle_body")]
    public string SubtitleBody { get; set; } = string.Empty;

    [JsonProperty("subtitle_avg_count")]
    public int SubtitleAvgCount { get; set; }

    [JsonProperty("lyrics_copyright")]
    public string LyricsCopyright { get; set; } = string.Empty;

    [JsonProperty("subtitle_length")]
    public int SubtitleLength { get; set; }

    [JsonProperty("subtitle_language")]
    public string SubtitleLanguage { get; set; } = string.Empty;

    [JsonProperty("subtitle_language_description")]
    public string SubtitleLanguageDescription { get; set; } = string.Empty;

    [JsonProperty("script_tracking_url")]
    public string ScriptTrackingUrl { get; set; } = string.Empty;

    [JsonProperty("pixel_tracking_url")]
    public string PixelTrackingUrl { get; set; } = string.Empty;

    [JsonProperty("html_tracking_url")]
    public string HtmlTrackingUrl { get; set; } = string.Empty;

    [JsonProperty("writer_list")]
    public List<object> WriterList { get; set; } = [];

    [JsonProperty("publisher_list")]
    public List<object> PublisherList { get; set; } = [];

    [JsonProperty("updated_time")]
    public DateTime? UpdatedTime { get; set; }
}
