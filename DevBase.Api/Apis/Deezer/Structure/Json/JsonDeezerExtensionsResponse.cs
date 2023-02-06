using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerExtensionsResponse
{
    [JsonProperty("cost")]
    public int cost { get; set; }
}