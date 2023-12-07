using Newtonsoft.Json;

namespace DevBase.Api.Apis.NetEase.Structure.Json;

public class JsonNetEaseUrlResponseData
{
    [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("br")]
        public int br { get; set; }

        [JsonProperty("size")]
        public int size { get; set; }

        [JsonProperty("md5")]
        public string md5 { get; set; }

        [JsonProperty("code")]
        public int code { get; set; }

        [JsonProperty("expi")]
        public int expi { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("gain")]
        public double gain { get; set; }

        [JsonProperty("peak")]
        public double peak { get; set; }

        [JsonProperty("fee")]
        public int fee { get; set; }

        [JsonProperty("uf")]
        public object uf { get; set; }

        [JsonProperty("payed")]
        public int payed { get; set; }

        [JsonProperty("flag")]
        public int flag { get; set; }

        [JsonProperty("canExtend")]
        public bool canExtend { get; set; }

        [JsonProperty("freeTrialInfo")]
        public object freeTrialInfo { get; set; }

        [JsonProperty("level")]
        public string level { get; set; }

        [JsonProperty("encodeType")]
        public string encodeType { get; set; }

        [JsonProperty("channelLayout")]
        public object channelLayout { get; set; }

        [JsonProperty("freeTrialPrivilege")]
        public JsonNetEaseUrlResponseDataFreeTrialPrivilege freeTrialPrivilege { get; set; }

        [JsonProperty("freeTimeTrialPrivilege")]
        public JsonNetEaseUrlResponseDataFreeTimeTrialPrivilege freeTimeTrialPrivilege { get; set; }

        [JsonProperty("urlSource")]
        public int urlSource { get; set; }

        [JsonProperty("rightSource")]
        public int rightSource { get; set; }

        [JsonProperty("podcastCtrp")]
        public object podcastCtrp { get; set; }

        [JsonProperty("effectTypes")]
        public object effectTypes { get; set; }

        [JsonProperty("time")]
        public int time { get; set; }
}