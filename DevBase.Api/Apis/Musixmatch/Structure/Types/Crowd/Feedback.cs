using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class Feedback
{
    [JsonProperty("type_id")]
    public string TypeId { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("app_id")]
    public string AppId { get; set; } = string.Empty;

    [JsonProperty("prev_lyrics_id")]
    public object? PrevLyricsId { get; set; }

    [JsonProperty("artist_id")]
    public int ArtistId { get; set; }

    [JsonProperty("wantkey")]
    public bool Wantkey { get; set; }

    [JsonProperty("lyrics_id")]
    public int LyricsId { get; set; }

    [JsonProperty("type_id_weight")]
    public int TypeIdWeight { get; set; }

    [JsonProperty("moderator")]
    public int Moderator { get; set; }

    [JsonProperty("effectiveness")]
    public double Effectiveness { get; set; }

    [JsonProperty("days_in_chart")]
    public int DaysInChart { get; set; }

    [JsonProperty("last_updated")]
    public DateTime? LastUpdated { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [JsonProperty("create_timestamp")]
    public int CreateTimestamp { get; set; }

    [JsonProperty("group_key")]
    public string GroupKey { get; set; } = string.Empty;

    [JsonProperty("image_id")]
    public int ImageId { get; set; }

    [JsonProperty("video_id")]
    public int VideoId { get; set; }

    [JsonProperty("subtitle_id")]
    public int SubtitleId { get; set; }

    [JsonProperty("updated_time")]
    public DateTime? UpdatedTime { get; set; }

    [JsonProperty("created_date")]
    public DateTime? CreatedDate { get; set; }

    [JsonProperty("commontrack_id")]
    public int CommontrackId { get; set; }

    [JsonProperty("friendly_type_id")]
    public string FriendlyTypeId { get; set; } = string.Empty;

    [JsonProperty("is_expired")]
    public int IsExpired { get; set; }

    [JsonProperty("track")]
    public Track? Track { get; set; }

    [JsonProperty("user")]
    public User? User { get; set; }

    [JsonProperty("time_spent")]
    public string TimeSpent { get; set; } = string.Empty;

    [JsonProperty("num_keypressed")]
    public object? NumKeypressed { get; set; }
}

public sealed class FeedbackList
{
    [JsonProperty("feedback")]
    public Feedback? Feedback { get; set; }
}
