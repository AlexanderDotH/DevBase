using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public class Album
{
    [JsonProperty("album_id")]
    public int AlbumId { get; set; }

    [JsonProperty("album_mbid")]
    public string AlbumMbid { get; set; } = string.Empty;

    [JsonProperty("album_name")]
    public string AlbumName { get; set; } = string.Empty;

    [JsonProperty("album_rating")]
    public int AlbumRating { get; set; }

    [JsonProperty("album_track_count")]
    public int AlbumTrackCount { get; set; }

    [JsonProperty("album_release_date")]
    public string AlbumReleaseDate { get; set; } = string.Empty;

    [JsonProperty("album_release_type")]
    public string AlbumReleaseType { get; set; } = string.Empty;

    [JsonProperty("artist_id")]
    public int ArtistId { get; set; }

    [JsonProperty("artist_name")]
    public string ArtistName { get; set; } = string.Empty;

    [JsonProperty("primary_genres")]
    public MusicGenreLayout PrimaryGenres { get; set; } = new();

    [JsonProperty("secondary_genres")]
    public MusicGenreLayout SecondaryGenres { get; set; } = new();

    [JsonProperty("album_pline")]
    public string AlbumPline { get; set; } = string.Empty;

    [JsonProperty("album_copyright")]
    public string AlbumCopyright { get; set; } = string.Empty;

    [JsonProperty("album_label")]
    public string AlbumLabel { get; set; } = string.Empty;

    [JsonProperty("updated_time")]
    public DateTime UpdatedTime { get; set; }

    [JsonProperty("album_coverart_100x100")]
    public string AlbumCoverart100x100 { get; set; } = string.Empty;
}
