using DevBase.Api.Apis.Musixmatch.Structure;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class ChartTracksGet : MusixmatchApiResponse
{
    [JsonProperty("track_list")]
    public List<TrackListItem> TrackList { get; set; } = [];

    public class TrackListItem
    {
        [JsonProperty("track")]
        public string Track { get; set; } = string.Empty;
    }
}
