﻿using Newtonsoft.Json;

namespace DevBase.Api.Apis.Tidal.Structure.Json
{
    public class JsonTidalTrack
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("replayGain")]
        public double ReplayGain { get; set; }

        [JsonProperty("peak")]
        public double Peak { get; set; }

        [JsonProperty("allowStreaming")]
        public bool AllowStreaming { get; set; }

        [JsonProperty("streamReady")]
        public bool StreamReady { get; set; }

        [JsonProperty("streamStartDate")]
        public DateTime StreamStartDate { get; set; }

        [JsonProperty("premiumStreamingOnly")]
        public bool PremiumStreamingOnly { get; set; }

        [JsonProperty("trackNumber")]
        public int TrackNumber { get; set; }

        [JsonProperty("volumeNumber")]
        public int VolumeNumber { get; set; }

        [JsonProperty("version")]
        public object Version { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("isrc")]
        public string Isrc { get; set; }

        [JsonProperty("editable")]
        public bool Editable { get; set; }

        [JsonProperty("explicit")]
        public bool Explicit { get; set; }

        [JsonProperty("audioQuality")]
        public string AudioQuality { get; set; }

        [JsonProperty("audioModes")]
        public List<string> AudioModes { get; set; }

        [JsonProperty("artist")]
        public JsonTidalArtist Artist { get; set; }

        [JsonProperty("artists")]
        public List<JsonTidalArtist> Artists { get; set; }

        [JsonProperty("album")]
        public JsonTidalAlbum Album { get; set; }

        [JsonProperty("mixes")]
        public JsonTidalMixes Mixes { get; set; }
    }
}