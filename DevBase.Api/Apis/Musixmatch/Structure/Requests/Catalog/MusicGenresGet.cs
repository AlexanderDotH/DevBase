using DevBase.Api.Apis.Musixmatch.Structure;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class MusicGenresGet : MusixmatchApiResponse
{
    [JsonProperty("music_genre_list")]
    public List<MusicGenreList> Results { get; set; } = [];

    public class MusicGenreList
    {
        [JsonProperty("music_genre")]
        public MusicGenre? MusicGenre { get; set; }
    }
}
