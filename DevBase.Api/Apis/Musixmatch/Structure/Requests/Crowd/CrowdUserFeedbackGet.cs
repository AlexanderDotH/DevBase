using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public sealed class CrowdUserFeedbackGet : MusixMatchApiResponse
{
    [JsonProperty("feedback_list")]
    public List<FeedbackList> Feedbacks { get; set; } = [];
}
