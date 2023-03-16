using DevBase.Api.Apis.Replicate.Structure;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Generics;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.ResponseData;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.Replicate;

public class Replicate
{
    private readonly string _endpoint;

    private AList<string> _tokens;

    public Replicate(AList<string> tokens)
    {
        this._endpoint = "https://api.replicate.com/v1";

        this._tokens = tokens;
    }
    
    public Replicate() : this(null) {}

    public async Task<ReplicatePredictionResponse> Predict(string modelID, string linkToAudio, string model, string apiKey, string webhook = "https://example.com")
    {
        JObject jObject = new JObject
        {
            {"version", modelID},
            {"input", 
                new JObject
                {
                {"audio", linkToAudio}, 
                {"model", model},
                {"transcription", "srt"}
                }
            },
            {"webhook", webhook}
        };

        RequestData requestData = new RequestData(string.Format("{0}/predictions", this._endpoint));
        
        requestData.AddContent(jObject.ToString());
        requestData.Header.Add("Authorization", 
            string.Format("Token {0}", apiKey));

        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        return new JsonDeserializer<ReplicatePredictionResponse>().Deserialize(responseData.GetContentAsString());
    }

    public async Task<ReplicatePredictionResponse> Predict(string modelID, string linkToAudio, string model, string webhook = "https://example.com")
    {
        if (this._tokens == null)
            return null;
        
        return await Predict(modelID, linkToAudio, model, this._tokens.GetRandom(), webhook);
    }

    public async Task<ReplicatePredictionResult> GetResult(string predictionID, string apiKey)
    {
        RequestData requestData = new RequestData(string.Format("{0}/predictions/{1}", this._endpoint, predictionID));

        requestData.SetContentType(EnumContentType.APPLICATION_JSON);

        requestData.Header.Add("Authorization", 
            string.Format("Token {0}", apiKey));
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        return new JsonDeserializer<ReplicatePredictionResult>().Deserialize(responseData.GetContentAsString());
    }
}