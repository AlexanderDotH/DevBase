using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class JwtGet
{
    [JsonProperty("jwt")]
    public string? JwtToken { get; set; }

    [JsonProperty("expire")]
    public DateTime Expire { get; set; }
}
