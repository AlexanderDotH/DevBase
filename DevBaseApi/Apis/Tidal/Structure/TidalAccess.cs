﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DevBaseApi.Apis.Tidal.Structure
{
    public class TidalAccess
    {
        [JsonProperty("isTidalConnected")]
        public bool IsTidalConnected { get; set; }

        [JsonProperty("userId")]
        public int UserID { get; set; }

        [JsonProperty("oAuthAccessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("oAuthRefreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("oAuthExpirationDate")]
        public long ExpirationDate { get; set; }
    }
}
