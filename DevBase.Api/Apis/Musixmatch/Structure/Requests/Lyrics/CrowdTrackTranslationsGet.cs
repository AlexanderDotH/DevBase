using DevBase.Api.Apis.Musixmatch.Structure;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class CrowdTrackTranslationsGet : MusixmatchApiResponse
{
    [JsonProperty("translations_list")]
    public List<TranslationsListRoot> TranslationsList { get; set; } = [];

    public class PostActionParams
    {
        [JsonProperty("skip_stats_update")]
        public bool SkipStatsUpdate { get; set; }
    }

    public class Translation
    {
        [JsonProperty("type_id")]
        public string TypeId { get; set; } = string.Empty;

        [JsonProperty("artist_id")]
        public int ArtistId { get; set; }

        [JsonProperty("language_from")]
        public string LanguageFrom { get; set; } = string.Empty;

        [JsonProperty("app_id")]
        public string AppId { get; set; } = string.Empty;

        [JsonProperty("ids")]
        public string Ids { get; set; } = string.Empty;

        [JsonProperty("snippet")]
        public string Snippet { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("selected_language")]
        public string SelectedLanguage { get; set; } = string.Empty;

        [JsonProperty("num_keypressed")]
        public int NumKeypressed { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("do_not_increment_ranking")]
        public bool DoNotIncrementRanking { get; set; }

        [JsonProperty("do_not_detect_language")]
        public bool DoNotDetectLanguage { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; } = string.Empty;

        [JsonProperty("wantkey")]
        public bool Wantkey { get; set; }

        [JsonProperty("group_key")]
        public string GroupKey { get; set; } = string.Empty;

        [JsonProperty("_validated")]
        public bool Validated { get; set; }

        [JsonProperty("create_timestamp")]
        public int CreateTimestamp { get; set; }

        [JsonProperty("type_id_weight")]
        public int TypeIdWeight { get; set; }

        [JsonProperty("effectiveness")]
        public double Effectiveness { get; set; }

        [JsonProperty("days_in_chart")]
        public int DaysInChart { get; set; }

        [JsonProperty("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; } = string.Empty;

        [JsonProperty("matched_line")]
        public string MatchedLine { get; set; } = string.Empty;

        [JsonProperty("subtitle_matched_line")]
        public string SubtitleMatchedLine { get; set; } = string.Empty;

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("user_score")]
        public int UserScore { get; set; }

        [JsonProperty("published_status_macro")]
        public int PublishedStatusMacro { get; set; }

        [JsonProperty("image_id")]
        public int ImageId { get; set; }

        [JsonProperty("video_id")]
        public int VideoId { get; set; }

        [JsonProperty("lyrics_id")]
        public int LyricsId { get; set; }

        [JsonProperty("subtitle_id")]
        public int SubtitleId { get; set; }

        [JsonProperty("created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("commontrack_id")]
        public int CommontrackId { get; set; }

        [JsonProperty("is_expired")]
        public int IsExpired { get; set; }

        [JsonProperty("can_translate")]
        public int CanTranslate { get; set; }

        [JsonProperty("can_delete")]
        public int CanDelete { get; set; }

        [JsonProperty("is_mine")]
        public int IsMine { get; set; }

        [JsonProperty("can_approve")]
        public int CanApprove { get; set; }

        [JsonProperty("moderator")]
        public int? Moderator { get; set; }

        [JsonProperty("votes")]
        public Votes Votes { get; set; } = new();

        [JsonProperty("post_action_params")]
        public PostActionParams PostActionParams { get; set; } = new();
    }

    public class TranslationsListRoot
    {
        [JsonProperty("translation")]
        public Translation Translation { get; set; } = new();
    }

    public class Votes
    {
        [JsonProperty("translation_ok")]
        public int TranslationOk { get; set; }
    }
}
