using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class TrackSubtitleGet
{
    [JsonProperty("subtitle")]
    public SubtitleRawResponse Subtitle { get; set; } = new();
}
