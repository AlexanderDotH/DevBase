using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class TrackRichsyncGet
{
    [JsonProperty("richsync")]
    public RichsyncRawResponse Richsync { get; set; } = new();
}
