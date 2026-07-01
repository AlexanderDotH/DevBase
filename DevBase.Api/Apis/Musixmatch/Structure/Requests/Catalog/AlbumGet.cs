using DevBase.Api.Apis.Musixmatch.Structure;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class AlbumGet : MusixmatchApiResponse
{
    [JsonProperty("album")]
    public Album? Album { get; set; }
}
