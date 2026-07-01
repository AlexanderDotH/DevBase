using DevBase.Api.Apis.Musixmatch.Structure;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Requests;

public class AlbumTracksGet : MusixmatchApiResponse
{
    [JsonProperty("track_list")]
    public List<TrackList> Results { get; set; } = [];

    public class TrackList
    {
        [JsonProperty("track")]
        public Track? Track { get; set; }
    }
}
