using Newtonsoft.Json;

namespace DevBaseApi.Apis.Deezer.Structure.Json;

public class JsonDeezerExtensionsResponse
{
    [JsonProperty("cost")]
    public int cost { get; set; }
}