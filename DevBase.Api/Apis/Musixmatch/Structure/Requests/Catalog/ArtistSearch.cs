using DevBase.Api.Apis.Musixmatch.Structure;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class ArtistSearch : MusixmatchApiResponse
{
    [JsonProperty("artist_list")]
    public List<ArtistList> Results { get; set; } = [];

    public class ArtistList
    {
        [JsonProperty("artist")]
        public Artist? Artist { get; set; }
    }
}
