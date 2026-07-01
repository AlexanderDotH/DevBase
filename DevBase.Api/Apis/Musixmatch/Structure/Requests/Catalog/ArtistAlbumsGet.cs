using DevBase.Api.Apis.Musixmatch.Structure;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class ArtistAlbumsGet : MusixmatchApiResponse
{
    [JsonProperty("album_list")]
    public List<AlbumList> Results { get; set; } = [];

    public class AlbumList
    {
        [JsonProperty("album")]
        public Album? Album { get; set; }
    }
}
