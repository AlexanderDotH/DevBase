using Newtonsoft.Json;

namespace DevBase.Api.Apis.Tidal.Structure.Json;

public class JsonTidalDownloadResult
{
    [JsonProperty("url")]
    public string url { get; set; }

    [JsonProperty("trackId")]
    public int trackId { get; set; }

    [JsonProperty("playTimeLeftInMinutes")]
    public int playTimeLeftInMinutes { get; set; }

    [JsonProperty("soundQuality")]
    public string soundQuality { get; set; }

    [JsonProperty("encryptionKey")]
    public string encryptionKey { get; set; }

    [JsonProperty("codec")]
    public string codec { get; set; }
}