using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class LyricsUser
{
    [JsonProperty("user")]
    public object? User { get; set; }
}

public sealed class LyricsVerifiedBy
{
    [JsonProperty("user")]
    public object? User { get; set; }
}

public sealed class Lyrics
{
    [JsonProperty("lyrics_id")]
    public int? LyricsId { get; set; }

    [JsonProperty("can_edit")]
    public int? CanEdit { get; set; }

    [JsonProperty("locked")]
    public int? Locked { get; set; }

    [JsonProperty("published_status")]
    public int? PublishedStatus { get; set; }

    [JsonProperty("action_requested")]
    public string? ActionRequested { get; set; }

    [JsonProperty("verified")]
    public int? Verified { get; set; }

    [JsonProperty("restricted")]
    public int? Restricted { get; set; }

    [JsonProperty("instrumental")]
    public int? Instrumental { get; set; }

    [JsonProperty("explicit")]
    public int? Explicit { get; set; }

    [JsonProperty("lyrics_body")]
    public string? LyricsBody { get; set; }

    [JsonProperty("lyrics_language")]
    public string? LyricsLanguage { get; set; }

    [JsonProperty("lyrics_language_description")]
    public string? LyricsLanguageDescription { get; set; }

    [JsonProperty("script_tracking_url")]
    public string? ScriptTrackingUrl { get; set; }

    [JsonProperty("pixel_tracking_url")]
    public string? PixelTrackingUrl { get; set; }

    [JsonProperty("html_tracking_url")]
    public string? HtmlTrackingUrl { get; set; }

    [JsonProperty("lyrics_copyright")]
    public string? LyricsCopyright { get; set; }

    [JsonProperty("writer_list")]
    public List<object>? WriterList { get; set; }

    [JsonProperty("publisher_list")]
    public List<object>? PublisherList { get; set; }

    [JsonProperty("backlink_url")]
    public string? BacklinkUrl { get; set; }

    [JsonProperty("updated_time")]
    public DateTime? UpdatedTime { get; set; }

    [JsonProperty("lyrics_user")]
    public LyricsUser? LyricsUser { get; set; }

    [JsonProperty("lyrics_verified_by")]
    public List<LyricsVerifiedBy>? LyricsVerifiedBy { get; set; }
}
