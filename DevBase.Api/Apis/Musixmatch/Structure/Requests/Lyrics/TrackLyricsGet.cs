using DevBase.Api.Apis.Musixmatch.Structure;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class TrackLyricsGet : MusixmatchApiResponse
{
    [JsonProperty("lyrics")]
    public Lyrics? Lyrics { get; set; }
}
