using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public class Artist
{
    [JsonProperty("artist_id")]
    public int ArtistId { get; set; }

    [JsonProperty("artist_mbid")]
    public string ArtistMbid { get; set; } = string.Empty;

    [JsonProperty("artist_name")]
    public string ArtistName { get; set; } = string.Empty;

    [JsonProperty("artist_country")]
    public string ArtistCountry { get; set; } = string.Empty;

    [JsonProperty("artist_alias_list")]
    public List<ArtistAliasList> ArtistAliasList { get; set; } = [];

    [JsonProperty("artist_rating")]
    public int ArtistRating { get; set; }

    [JsonProperty("artist_twitter_url")]
    public string ArtistTwitterUrl { get; set; } = string.Empty;

    [JsonProperty("updated_time")]
    public DateTime UpdatedTime { get; set; }
}

public class ArtistAliasList
{
    [JsonProperty("artist_alias")]
    public string ArtistAlias { get; set; } = string.Empty;
}
