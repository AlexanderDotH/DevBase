using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public class MusicGenre
{
    [JsonProperty("music_genre_id")]
    public int MusicGenreId { get; set; }

    [JsonProperty("music_genre_parent_id")]
    public int MusicGenreParentId { get; set; }

    [JsonProperty("music_genre_name")]
    public string MusicGenreName { get; set; } = string.Empty;

    [JsonProperty("music_genre_name_extended")]
    public string MusicGenreNameExtended { get; set; } = string.Empty;

    [JsonProperty("music_genre_vanity")]
    public string MusicGenreVanity { get; set; } = string.Empty;
}

public class MusicGenreList
{
    [JsonProperty("music_genre")]
    public MusicGenre? MusicGenre { get; set; }
}

public class MusicGenreLayout
{
    [JsonProperty("music_genre_list")]
    public List<MusicGenreList>? MusicGenreList { get; set; }
}
