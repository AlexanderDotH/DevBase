using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class GraphQLAvailableMissionsListResponse
{
    [JsonProperty("items")]
    public List<Mission> Items { get; set; } = [];
}
