using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserData
{
    [JsonProperty("error")]
    public List<object> error { get; set; }

    [JsonProperty("results")]
    public JsonDeezerUserDataResults results { get; set; }
}