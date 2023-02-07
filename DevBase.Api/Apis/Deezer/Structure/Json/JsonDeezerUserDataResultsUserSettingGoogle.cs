using Newtonsoft.Json;

namespace DevBase.Api.Apis.Deezer.Structure.Json;

public class JsonDeezerUserDataResultsUserSettingGoogle
{
    [JsonProperty("share_comment")]
    public bool share_comment { get; set; }

    [JsonProperty("share_favourite")]
    public bool share_favourite { get; set; }

    [JsonProperty("share_loved")]
    public bool share_loved { get; set; }

    [JsonProperty("share_listen")]
    public bool share_listen { get; set; }

    [JsonProperty("share_share")]
    public bool share_share { get; set; }

    [JsonProperty("country_store")]
    public bool country_store { get; set; }
}