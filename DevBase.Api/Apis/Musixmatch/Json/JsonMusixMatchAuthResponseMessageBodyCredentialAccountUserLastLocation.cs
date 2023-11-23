using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageBodyCredentialAccountUserLastLocation
{
    [JsonProperty("GEOIP_CITY_COUNTRY_CODE")]
    public string GEOIP_CITY_COUNTRY_CODE { get; set; }

    [JsonProperty("GEOIP_CITY_COUNTRY_CODE3")]
    public string GEOIP_CITY_COUNTRY_CODE3 { get; set; }

    [JsonProperty("GEOIP_CITY_COUNTRY_NAME")]
    public string GEOIP_CITY_COUNTRY_NAME { get; set; }

    [JsonProperty("GEOIP_CITY")]
    public string GEOIP_CITY { get; set; }

    [JsonProperty("GEOIP_CITY_CONTINENT_CODE")]
    public string GEOIP_CITY_CONTINENT_CODE { get; set; }

    [JsonProperty("GEOIP_LATITUDE")]
    public double GEOIP_LATITUDE { get; set; }

    [JsonProperty("GEOIP_LONGITUDE")]
    public double GEOIP_LONGITUDE { get; set; }

    [JsonProperty("GEOIP_AS_ORG")]
    public string GEOIP_AS_ORG { get; set; }

    [JsonProperty("GEOIP_ORG")]
    public string GEOIP_ORG { get; set; }

    [JsonProperty("GEOIP_ISP")]
    public string GEOIP_ISP { get; set; }

    [JsonProperty("GEOIP_NET_NAME")]
    public string GEOIP_NET_NAME { get; set; }

    [JsonProperty("BADIP_TAGS")]
    public List<object> BADIP_TAGS { get; set; }
}