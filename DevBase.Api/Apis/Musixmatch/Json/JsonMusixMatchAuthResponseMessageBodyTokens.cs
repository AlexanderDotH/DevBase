using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Json;

public class JsonMusixMatchAuthResponseMessageBodyTokens
{
    [JsonProperty("musixmatch-artists-v2.0")]
    public string musixmatchartistsv20 { get; set; }

    [JsonProperty("mxm-backoffice-v1.0")]
    public string mxmbackofficev10 { get; set; }

    [JsonProperty("musixmatch-podcasts-v2.0")]
    public string musixmatchpodcastsv20 { get; set; }

    [JsonProperty("musixmatch-podcasts-v2.0-pp")]
    public string musixmatchpodcastsv20pp { get; set; }

    [JsonProperty("mxm-pro-web-v1.0")]
    public string mxmprowebv10 { get; set; }

    [JsonProperty("mxm-com-v1.0")]
    public string mxmcomv10 { get; set; }

    [JsonProperty("musixmatch-publishers-v2.0")]
    public string musixmatchpublishersv20 { get; set; }

    [JsonProperty("web-desktop-app-v1.0")]
    public string webdesktopappv10 { get; set; }

    [JsonProperty("community-app-v1.0")]
    public string communityappv10 { get; set; }

    [JsonProperty("mxm-proxy-manager-v1.0")]
    public string mxmproxymanagerv10 { get; set; }

    [JsonProperty("mxm-studio-v1.0")]
    public string mxmstudiov10 { get; set; }

    [JsonProperty("mxm-account-v1.0")]
    public string mxmaccountv10 { get; set; }
}