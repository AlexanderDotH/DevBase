﻿using Newtonsoft.Json;

namespace DevBase.Api.Apis.Replicate.Structure;

public class ReplicatePredictionResultUrls
{
    [JsonProperty("get")]
    public string get { get; set; }

    [JsonProperty("cancel")]
    public string cancel { get; set; }
}