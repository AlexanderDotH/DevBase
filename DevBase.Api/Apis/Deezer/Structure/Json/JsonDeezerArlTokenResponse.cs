using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerArlTokenResponse
{
    [JsonProperty("error")]
    public List<object> error { get; set; }

    [JsonProperty("results")]
    public string results { get; set; }
}