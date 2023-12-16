using Newtonsoft.Json;

namespace DevBase.Api.Apis.AppleMusic.Structure.Json;

public class JsonAppleMusicSearchResultResultsSongDataAttributes
{
    [JsonProperty("hasTimeSyncedLyrics")]
    public bool HasTimeSyncedLyrics { get; set; }

    [JsonProperty("albumName")]
    public string AlbumName { get; set; }

    [JsonProperty("genreNames")]
    public List<string> GenreNames { get; set; }

    [JsonProperty("trackNumber")]
    public int TrackNumber { get; set; }

    [JsonProperty("releaseDate")]
    public string ReleaseDate { get; set; }

    [JsonProperty("durationInMillis")]
    public int DurationInMillis { get; set; }
    
    [JsonProperty("isVocalAttenuationAllowed")]
    public bool IsVocalAttenuationAllowed { get; set; }

    [JsonProperty("isMasteredForItunes")]
    public bool IsMasteredForItunes { get; set; }

    [JsonProperty("isrc")]
    public string Isrc { get; set; }

    [JsonProperty("artwork")]
    public JsonAppleMusicSearchResultResultsSongDataAttributesArtwork Artwork { get; set; }

    [JsonProperty("audioLocale")]
    public string AudioLocale { get; set; }

    [JsonProperty("composerName")]
    public string ComposerName { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("playParams")]
    public JsonAppleMusicSearchResultResultsSongDataAttributesPlayParams PlayParams { get; set; }

    [JsonProperty("discNumber")]
    public int DiscNumber { get; set; }

    [JsonProperty("hasCredits")]
    public bool HasCredits { get; set; }

    [JsonProperty("hasLyrics")]
    public bool HasLyrics { get; set; }

    [JsonProperty("isAppleDigitalMaster")]
    public bool IsAppleDigitalMaster { get; set; }

    [JsonProperty("audioTraits")]
    public List<string> AudioTraits { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("previews")]
    public List<JsonAppleMusicSearchResultResultsSongDataAttributesPreview> Previews { get; set; }

    [JsonProperty("artistName")]
    public string ArtistName { get; set; }

    [JsonProperty("contentRating")]
    public string ContentRating { get; set; }

    [JsonProperty("movementName")]
    public string MovementName { get; set; }

    [JsonProperty("movementCount")]
    public int? MovementCount { get; set; }

    [JsonProperty("movementNumber")]
    public int? MovementNumber { get; set; }

    [JsonProperty("workName")]
    public string WorkName { get; set; }

    [JsonProperty("attribution")] 
    public string Attribution { get; set; }
    
}