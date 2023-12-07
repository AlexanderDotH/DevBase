using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseDetailResponseSong
{
    [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("pst")]
        public int pst { get; set; }

        [JsonProperty("t")]
        public int t { get; set; }

        [JsonProperty("ar")]
        public List<JsonNetEaseDetailResponseSongAr> Artists { get; set; }

        [JsonProperty("alia")]
        public List<object> Alias { get; set; }

        [JsonProperty("pop")]
        public int pop { get; set; }

        [JsonProperty("st")]
        public int st { get; set; }

        [JsonProperty("rt")]
        public string rt { get; set; }

        [JsonProperty("fee")]
        public int fee { get; set; }

        [JsonProperty("v")]
        public int v { get; set; }

        [JsonProperty("crbt")]
        public object crbt { get; set; }

        [JsonProperty("cf")]
        public string cf { get; set; }

        [JsonProperty("al")]
        public JsonNetEaseDetailResponseSongAl Album { get; set; }

        [JsonProperty("dt")]
        public int dt { get; set; }

        [JsonProperty("h")]
        public JsonNetEaseDetailResponseSongH High { get; set; }

        [JsonProperty("m")]
        public JsonNetEaseDetailResponseSongM Medium { get; set; }

        [JsonProperty("l")]
        public JsonNetEaseDetailResponseSongL Low { get; set; }

        [JsonProperty("sq")]
        public JsonNetEaseDetailResponseSongSq sq { get; set; }

        [JsonProperty("hr")]
        public object hr { get; set; }

        [JsonProperty("a")]
        public object a { get; set; }

        [JsonProperty("cd")]
        public string cd { get; set; }

        [JsonProperty("no")]
        public int no { get; set; }

        [JsonProperty("rtUrl")]
        public object rtUrl { get; set; }

        [JsonProperty("ftype")]
        public int ftype { get; set; }

        [JsonProperty("rtUrls")]
        public List<object> rtUrls { get; set; }

        [JsonProperty("djId")]
        public int djId { get; set; }

        [JsonProperty("copyright")]
        public int copyright { get; set; }

        [JsonProperty("s_id")]
        public int s_id { get; set; }

        [JsonProperty("mark")]
        public int mark { get; set; }

        [JsonProperty("originCoverType")]
        public int originCoverType { get; set; }

        [JsonProperty("originSongSimpleData")]
        public object originSongSimpleData { get; set; }

        [JsonProperty("tagPicList")]
        public object tagPicList { get; set; }

        [JsonProperty("resourceState")]
        public bool resourceState { get; set; }

        [JsonProperty("version")]
        public int version { get; set; }

        [JsonProperty("songJumpInfo")]
        public object songJumpInfo { get; set; }

        [JsonProperty("entertainmentTags")]
        public object entertainmentTags { get; set; }

        [JsonProperty("awardTags")]
        public object awardTags { get; set; }

        [JsonProperty("single")]
        public int single { get; set; }

        [JsonProperty("noCopyrightRcmd")]
        public object noCopyrightRcmd { get; set; }

        [JsonProperty("rtype")]
        public int rtype { get; set; }

        [JsonProperty("rurl")]
        public object rurl { get; set; }

        [JsonProperty("mst")]
        public int mst { get; set; }

        [JsonProperty("cp")]
        public int cp { get; set; }

        [JsonProperty("mv")]
        public int mv { get; set; }

        [JsonProperty("publishTime")]
        public object publishTime { get; set; }
}