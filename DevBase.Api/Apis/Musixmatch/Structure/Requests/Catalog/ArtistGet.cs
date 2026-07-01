using DevBase.Api.Apis.Musixmatch.Structure;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class ArtistGet : MusixmatchApiResponse
{
    [JsonProperty("artist")]
    public Artist? Artist { get; set; }
}
