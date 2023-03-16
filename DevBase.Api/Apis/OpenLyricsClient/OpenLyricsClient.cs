using System.Net;
using DevBase.Api.Apis.OpenLyricsClient.Structure.Json;
using DevBase.Api.Apis.Tidal.Structure.Json;
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
        this._baseUrl = "https://openlyricsclient.com/api";
    }

    public async Task<JsonOpenLyricsClientAccess> GetAccessToken(string refreshToken)
    {
        RequestData data = new RequestData(string.Format("{0}/auth/spotify/refresh", this._baseUrl));

        JObject jObject = new JObject();
        jObject["refreshToken"] = refreshToken;
        
        data.AddContent(jObject.ToString());
        data.SetContentType(EnumContentType.APPLICATION_JSON);
        
        ResponseData responseData = await new Request(data).GetResponseAsync();
        return new JsonDeserializer<JsonOpenLyricsClientAccess>().Deserialize(responseData.GetContentAsString());
    } 
    
    public async Task<JsonOpenLyricsClientAiPredictionResponse> SubmitAiSync(string title, string album, long duration, params string[] artists)
    {
        RequestData data = new RequestData(string.Format("{0}/ai/submit", this._baseUrl));

        JObject jObject = new JObject();
        
        jObject["title"] = title;
        jObject["album"] = album;
        jObject["artists"] = new JArray(artists);
        jObject["duration"] = duration;
        
        data.AddContent(jObject.ToString());
        data.SetContentType(EnumContentType.APPLICATION_JSON);
        
        ResponseData responseData = await new Request(data).GetResponseAsync();
        return new JsonDeserializer<JsonOpenLyricsClientAiPredictionResponse>().Deserialize(responseData.GetContentAsString());
    } 
    
    public async Task<JsonOpenLyricsClientAiPredictionResult> GetAiSyncResult(string songID)
    {
        RequestData data = new RequestData(string.Format("{0}/ai/result", this._baseUrl));

        JObject jObject = new JObject();
        jObject["id"] = songID;
        
        data.AddContent(jObject.ToString());
        data.SetContentType(EnumContentType.APPLICATION_JSON);

        try
        {
            ResponseData responseData = await new Request(data).GetResponseAsync();
            return new JsonDeserializer<JsonOpenLyricsClientAiPredictionResult>().Deserialize(responseData.GetContentAsString());
        }
        catch (System.Net.WebException e)
        {
            HttpWebResponse webResponse = (HttpWebResponse)e.Response;

            switch (webResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new System.Exception("The prediction is still running, try again later");
                
                case HttpStatusCode.Conflict:
                    throw new System.Exception(
                        "Could not find the prediction, please resubmit the prediction or check if the id is correct");
            }
        }

        return null;
    } 
    
}