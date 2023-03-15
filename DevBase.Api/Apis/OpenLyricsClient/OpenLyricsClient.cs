using DevBase.Api.Apis.OpenLyricsClient.Structure.Json;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.ResponseData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.OpenLyricsClient;

public class OpenLyricsClient
{
    private readonly string _baseUrl;

    public OpenLyricsClient()
    {
        this._baseUrl = "https://openlyricsclient.com/api/auth/spotify";
    }

    public async Task<JsonOpenLyricsClientAccess> GetAccessToken(string refreshToken)
    {
        RequestData data = new RequestData(string.Format("{0}/refresh", this._baseUrl));

        JObject jObject = new JObject();
        jObject["refreshToken"] = refreshToken;
        
        data.AddContent(jObject.ToString());
        data.SetContentType(EnumContentType.APPLICATION_JSON);
        
        ResponseData responseData = await new Request(data).GetResponseAsync();
        return new JsonDeserializer<JsonOpenLyricsClientAccess>().Deserialize(responseData.GetContentAsString());
    } 
    
}