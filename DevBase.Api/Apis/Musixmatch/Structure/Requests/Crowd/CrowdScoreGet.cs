using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class CrowdScoreGet : MusixMatchApiResponse
{
    [JsonProperty("user")]
    public User? User { get; set; }
}
