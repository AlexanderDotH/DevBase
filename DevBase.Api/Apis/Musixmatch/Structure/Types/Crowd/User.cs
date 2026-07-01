using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class RankColors
{
    [JsonProperty("rank_color_10")]
    public string RankColor10 { get; set; } = string.Empty;

    [JsonProperty("rank_color_50")]
    public string RankColor50 { get; set; } = string.Empty;

    [JsonProperty("rank_color_100")]
    public string RankColor100 { get; set; } = string.Empty;

    [JsonProperty("rank_color_200")]
    public string RankColor200 { get; set; } = string.Empty;
}

public sealed class NextRankColors
{
    [JsonProperty("rank_color_10")]
    public string RankColor10 { get; set; } = string.Empty;

    [JsonProperty("rank_color_50")]
    public string RankColor50 { get; set; } = string.Empty;

    [JsonProperty("rank_color_100")]
    public string RankColor100 { get; set; } = string.Empty;

    [JsonProperty("rank_color_200")]
    public string RankColor200 { get; set; } = string.Empty;
}

public sealed class Counters
{
    [JsonProperty("track_translation")]
    public int TrackTranslation { get; set; }

    [JsonProperty("lyrics_missing")]
    public int LyricsMissing { get; set; }

    [JsonProperty("lyrics_ok")]
    public int LyricsOk { get; set; }

    [JsonProperty("lyrics_ko")]
    public int LyricsKo { get; set; }

    [JsonProperty("lyrics_changed")]
    public int LyricsChanged { get; set; }

    [JsonProperty("vote_bonuses")]
    public int VoteBonuses { get; set; }

    [JsonProperty("translation_ok")]
    public int TranslationOk { get; set; }

    [JsonProperty("track_influencer_bonus_moderator_vote")]
    public int TrackInfluencerBonusModeratorVote { get; set; }

    [JsonProperty("lyrics_favourite_added")]
    public int LyricsFavouriteAdded { get; set; }

    [JsonProperty("lyrics_ai_phrases_not_related_no")]
    public int LyricsAiPhrasesNotRelatedNo { get; set; }

    [JsonProperty("lyrics_report_contain_mistakes")]
    public int LyricsReportContainMistakes { get; set; }

    [JsonProperty("lyrics_subtitle_added")]
    public int LyricsSubtitleAdded { get; set; }

    [JsonProperty("lyrics_music_id")]
    public int LyricsMusicId { get; set; }

    [JsonProperty("lyrics_ai_phrases_not_related_yes")]
    public int LyricsAiPhrasesNotRelatedYes { get; set; }

    [JsonProperty("lyrics_report_incomplete_lyrics")]
    public int LyricsReportIncompleteLyrics { get; set; }

    [JsonProperty("lyrics_ai_phrases_not_related_skip")]
    public int LyricsAiPhrasesNotRelatedSkip { get; set; }

    [JsonProperty("lyrics_report_completely_wrong")]
    public int LyricsReportCompletelyWrong { get; set; }

    [JsonProperty("lyrics_implicitly_ok")]
    public int LyricsImplicitlyOk { get; set; }

    [JsonProperty("vote_maluses")]
    public int VoteMaluses { get; set; }

    [JsonProperty("lyrics_richsync_added")]
    public int LyricsRichsyncAdded { get; set; }

    [JsonProperty("lyrics_ranking_change")]
    public int LyricsRankingChange { get; set; }

    [JsonProperty("lyrics_ai_mood_analysis_v3_value")]
    public int LyricsAiMoodAnalysisV3Value { get; set; }

    [JsonProperty("lyrics_ai_ugc_language")]
    public int LyricsAiUgcLanguage { get; set; }

    [JsonProperty("track_structure")]
    public int TrackStructure { get; set; }

    [JsonProperty("track_complete_metadata")]
    public int TrackCompleteMetadata { get; set; }
}

public sealed class User
{
    [JsonProperty("uaid")]
    public string Uaid { get; set; } = string.Empty;

    [JsonProperty("is_mine")]
    public int IsMine { get; set; }

    [JsonProperty("user_name")]
    public string UserName { get; set; } = string.Empty;

    [JsonProperty("user_profile_photo")]
    public string UserProfilePhoto { get; set; } = string.Empty;

    [JsonProperty("has_private_profile")]
    public int HasPrivateProfile { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }

    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("weekly_score")]
    public int WeeklyScore { get; set; }

    [JsonProperty("level")]
    public string Level { get; set; } = string.Empty;

    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [JsonProperty("rank_level")]
    public int RankLevel { get; set; }

    [JsonProperty("points_to_next_level")]
    public int PointsToNextLevel { get; set; }

    [JsonProperty("ratio_to_next_level")]
    public double RatioToNextLevel { get; set; }

    [JsonProperty("rank_name")]
    public string RankName { get; set; } = string.Empty;

    [JsonProperty("next_rank_name")]
    public string NextRankName { get; set; } = string.Empty;

    [JsonProperty("ratio_to_next_rank")]
    public double RatioToNextRank { get; set; }

    [JsonProperty("rank_color")]
    public string RankColor { get; set; } = string.Empty;

    [JsonProperty("rank_colors")]
    public RankColors? RankColors { get; set; }

    [JsonProperty("rank_image_url")]
    public string RankImageUrl { get; set; } = string.Empty;

    [JsonProperty("next_rank_color")]
    public string NextRankColor { get; set; } = string.Empty;

    [JsonProperty("next_rank_colors")]
    public NextRankColors? NextRankColors { get; set; }

    [JsonProperty("next_rank_image_url")]
    public string NextRankImageUrl { get; set; } = string.Empty;

    [JsonProperty("counters")]
    public Counters? Counters { get; set; }

    [JsonProperty("academy_completed")]
    public bool AcademyCompleted { get; set; }

    [JsonProperty("moderator")]
    public bool Moderator { get; set; }
}
