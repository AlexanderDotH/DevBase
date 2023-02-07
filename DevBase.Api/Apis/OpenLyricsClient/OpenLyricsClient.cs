using DevBase.Api.Apis.OpenLyricsClient.Structure.Json;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.ResponseData;
using Newtonsoft.Json;

namespace DevBase.Api.Apis.OpenLyricsClient;

public class OpenLyricsClient
{
    private readonly string _baseAuthUrl;
    private readonly string _baseAuthBeginUrl;
    private readonly string _redirectUrl;

    public OpenLyricsClient()
    {
        this._baseAuthUrl = "https://www.openlyricsclient.com/connect/spotify/auth";
        this._baseAuthBeginUrl = "https://www.openlyricsclient.com/connect/spotify/begin";
        this._redirectUrl = "https://www.openlyricsclient.com/connect/spotify/complete";
    }

    public async Task<JsonOpenLyricsClientAccess> GetAccessToken(string refreshToken)
    {
        RequestData data = new RequestData(string.Format("{0}/refresh.php?refresh_token={1}", this._baseAuthUrl, refreshToken));
        ResponseData responseData = await new Request(data).GetResponseAsync();
        return JsonConvert.DeserializeObject<JsonOpenLyricsClientAccess>(responseData.GetContentAsString());
    } 
    
}