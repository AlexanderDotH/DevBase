using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class CrowdChartUsersGet : MusixMatchApiResponse
{
    [JsonProperty("user_chart_list")]
    public List<UserList> Results { get; set; } = [];

    public sealed class UserList
    {
        [JsonProperty("user")]
        public User? User { get; set; }
    }
}
