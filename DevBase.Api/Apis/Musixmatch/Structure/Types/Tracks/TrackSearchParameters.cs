namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class TrackSearchParameters
{
    private static readonly IReadOnlyDictionary<SortStrategy, KeyValuePair<string, string>> StrategyParameters =
        new Dictionary<SortStrategy, KeyValuePair<string, string>>
        {
            [SortStrategy.TrackRatingAsc] = new KeyValuePair<string, string>("s_track_rating", "asc"),
            [SortStrategy.TrackRatingDesc] = new KeyValuePair<string, string>("s_track_rating", "desc"),
            [SortStrategy.ArtistRatingAsc] = new KeyValuePair<string, string>("s_artist_rating", "asc"),
            [SortStrategy.ArtistRatingDesc] = new KeyValuePair<string, string>("s_artist_rating", "desc"),
            [SortStrategy.ReleaseDateAsc] = new KeyValuePair<string, string>("s_track_release_date", "asc"),
            [SortStrategy.ReleaseDateDesc] = new KeyValuePair<string, string>("s_track_release_date", "desc")
        };

    public enum SortStrategy
    {
        TrackRatingAsc,
        TrackRatingDesc,
        ArtistRatingAsc,
        ArtistRatingDesc,
        ReleaseDateAsc,
        ReleaseDateDesc
    }

    public string Query { get; set; } = string.Empty;

    public string LyricsQuery { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;

    public string Album { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public bool HasLyrics { get; set; } = true;

    public bool HasSubtitles { get; set; }

    public bool HasRichSync { get; set; }

    public SortStrategy Sort { get; set; } = SortStrategy.TrackRatingDesc;

    public KeyValuePair<string, string> GetSortParameter()
    {
        return StrategyParameters[this.Sort];
    }
}
