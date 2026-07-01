using DevBase.Api.Apis.Musixmatch.Structure;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class TrackSnippetGet : MusixmatchApiResponse
{
    [JsonProperty("snippet")]
    public Snippet? Snippet { get; set; }
}
