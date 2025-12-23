using System.Net;
using DevBase.Api.Apis.OpenLyricsClient.Structure.Enum;
using DevBase.Api.Apis.OpenLyricsClient.Structure.Json;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Api.Serializer;
using DevBase.Cryptography.BouncyCastle.Sealing;
using DevBase.Enums;
using DevBase.Net.Core;
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
        JObject jObject = new JObject();
        jObject["refreshToken"] = refreshToken;
        
        Response response = await new Request($"{this._baseUrl}/auth/spotify/refresh")
            .AsPost()
            .WithJsonBody(jObject.ToString())
            .SendAsync();
        
        return await response.ParseJsonAsync<JsonOpenLyricsClientAccess>(false);
    } 
    
    public async Task<JsonOpenLyricsClientAiSyncItem[]> AiSync(JsonOpenLyricsClientSubscription subscription, string title, string album, long duration, string model = "", params string[] artists)
    {
        JObject jObject = new JObject();
        
        jObject["title"] = title;
        jObject["album"] = album;
        jObject["artists"] = new JArray(artists);
        jObject["duration"] = duration;
        jObject["model"] = model;
        jObject["sealedAccess"] = ToSealedAccess(subscription);

        Response response = await new Request($"{this._baseUrl}/ai/sync")
            .AsPost()
            .WithTimeout(TimeSpan.FromMinutes(10))
            .WithJsonBody(jObject.ToString())
            .SendAsync();
        
        return await response.ParseJsonAsync<JsonOpenLyricsClientAiSyncItem[]>(false);
    } 
    
    #pragma warning disable S1133
    [Obsolete("This api call only works with a older backend")]
    public async Task<JsonOpenLyricsClientAiPredictionResult> GetAiSyncResult(string songID)
    {
        JObject jObject = new JObject();
        jObject["id"] = songID;

        Response response = await new Request($"{this._baseUrl}/ai/result")
            .AsPost()
            .WithJsonBody(jObject.ToString())
            .SendAsync();
            
        if (response.StatusCode == HttpStatusCode.NotFound)
             return Throw<object>(new OpenLyricsClientException(EnumOpenLyricsClientExceptionType.PredictionInProgress));
             
        if (response.StatusCode == HttpStatusCode.Conflict)
             return Throw<object>(new OpenLyricsClientException(EnumOpenLyricsClientExceptionType.PredictionUnavailable));
        
        if (response.IsSuccessStatusCode)
             return await response.ParseJsonAsync<JsonOpenLyricsClientAiPredictionResult>(false);

        return null;
    }
    #pragma warning restore S1133

    public async Task<JsonOpenLyricsClientSubscription> CreateSubscription()
    {
        CheckSealing();
        
        Response response = await new Request($"{this._baseUrl}/subscription/create").SendAsync();
        return await response.ParseJsonAsync<JsonOpenLyricsClientSubscription>(false);
    }

    public async Task<JsonOpenLyricsClientSubscriptionModel> CheckSubscription(JsonOpenLyricsClientSubscription subscription)
    {
        Response response = await new Request($"{this._baseUrl}/subscription/check")
            .AsPost()
            .WithJsonBody(ToSealedAccess(subscription).ToString())
            .SendAsync();
        
        return await response.ParseJsonAsync<JsonOpenLyricsClientSubscriptionModel>(false);
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
