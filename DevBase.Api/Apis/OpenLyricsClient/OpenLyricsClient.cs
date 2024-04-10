using System.Net;
using DevBase.Api.Apis.OpenLyricsClient.Structure.Enum;
using DevBase.Api.Apis.OpenLyricsClient.Structure.Json;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Api.Serializer;
using DevBase.Cryptography.BouncyCastle.Sealing;
using DevBase.Enums;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.ResponseData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.OpenLyricsClient;

public class OpenLyricsClient : ApiClient
{
    private readonly string _baseUrl;
    private readonly Sealing _sealing;

    public OpenLyricsClient()
    {
        this._baseUrl = "https://openlyricsclient.com/api";
    }
    
    public OpenLyricsClient(string serverPublicKey) : this()
    {
        this._sealing = new Sealing(serverPublicKey);
    }

    public async Task<JsonOpenLyricsClientAccess> GetAccessToken(string refreshToken)
    {
        RequestData data = new RequestData($"{this._baseUrl}/auth/spotify/refresh");

        JObject jObject = new JObject();
        jObject["refreshToken"] = refreshToken;
        
        data.AddContent(jObject.ToString());
        data.SetContentType(EnumContentType.APPLICATION_JSON);
        
        ResponseData responseData = await new Request(data).GetResponseAsync();
        return new JsonDeserializer<JsonOpenLyricsClientAccess>().Deserialize(responseData.GetContentAsString());
    } 
    
    public async Task<JsonOpenLyricsClientAiSyncItem[]> AiSync(JsonOpenLyricsClientSubscription subscription, string title, string album, long duration, string model = "", params string[] artists)
    {
        RequestData data = new RequestData($"{this._baseUrl}/ai/sync");
        
        JObject jObject = new JObject();
        
        jObject["title"] = title;
        jObject["album"] = album;
        jObject["artists"] = new JArray(artists);
        jObject["duration"] = duration;
        jObject["model"] = model;
        jObject["sealedAccess"] = ToSealedAccess(subscription);

        data.Timeout = TimeSpan.FromMinutes(10);
        data.AddContent(jObject.ToString());
        data.SetContentType(EnumContentType.APPLICATION_JSON);
        
        ResponseData responseData = await new Request(data).GetResponseAsync();
        return new JsonDeserializer<JsonOpenLyricsClientAiSyncItem[]>().Deserialize(responseData.GetContentAsString());
    } 
    
    #pragma warning disable S1133
    [Obsolete("This api call only works with a older backend")]
    public async Task<JsonOpenLyricsClientAiPredictionResult> GetAiSyncResult(string songID)
    {
        RequestData data = new RequestData($"{this._baseUrl}/ai/result");

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
            HttpWebResponse webResponse = (HttpWebResponse)e.Response!;

            switch (webResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return Throw<object>(
                        new OpenLyricsClientException(EnumOpenLyricsClientExceptionType.PredictionInProgress));
                
                case HttpStatusCode.Conflict:
                    return Throw<object>(
                        new OpenLyricsClientException(EnumOpenLyricsClientExceptionType.PredictionUnavailable));
            }
        }

        return null;
    }
    #pragma warning restore S1133

    public async Task<JsonOpenLyricsClientSubscription> CreateSubscription()
    {
        CheckSealing();
        
        RequestData data = new RequestData($"{this._baseUrl}/subscription/create");
        Request request = new Request(data);
        ResponseData response = await request.GetResponseAsync();
        return new JsonDeserializer<JsonOpenLyricsClientSubscription>().Deserialize(response.GetContentAsString());
    }

    public async Task<JsonOpenLyricsClientSubscriptionModel> CheckSubscription(JsonOpenLyricsClientSubscription subscription)
    {
        RequestData data = new RequestData($"{this._baseUrl}/subscription/check");
        
        data.AddContent(ToSealedAccess(subscription).ToString());
        data.SetContentType(EnumContentType.APPLICATION_JSON);

        Request request = new Request(data);
        ResponseData response = await request.GetResponseAsync();
        
        return new JsonDeserializer<JsonOpenLyricsClientSubscriptionModel>().Deserialize(response.GetContentAsString());
    }

    private JObject ToSealedAccess(JsonOpenLyricsClientSubscription subscription)
    {
        CheckSealing();
        
        JObject jObject = new JObject();
        jObject["userID"] = subscription.UserID;
        jObject["userSecret"] = subscription.UserSecret;

        string secret = jObject.ToString();

        string seal = this._sealing.Seal(secret);

        JObject sealObject = new JObject();
        sealObject["sealed"] = seal;

        return sealObject;
    }
    
    private void CheckSealing()
    {
        if (this._sealing == null)
            Throw<object>(new OpenLyricsClientException(EnumOpenLyricsClientExceptionType.SealingNotInitialized));
    }
}