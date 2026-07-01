using DevBase.Api.Apis.Musixmatch.Structure;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class TrackGet : MusixmatchApiResponse
{
    [JsonProperty("track")]
    public Track? Track { get; set; }
}
