using DevBase.Api.Apis.Replicate.Structure;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Generics;
using DevBase.Net.Core;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.Replicate;

public class Replicate : ApiClient
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

        Response response = await new Request($"{this._endpoint}/predictions")
            .AsPost()
            .WithJsonBody(jObject.ToString())
            .WithHeader("Authorization",$"Token {apiKey}")
            .SendAsync();

        return await response.ParseJsonAsync<ReplicatePredictionResponse>(false);
    }

    public async Task<ReplicatePredictionResponse> Predict(string modelID, string linkToAudio, string model, string webhook = "https://example.com")
    {
        if (this._tokens.IsEmpty())
            return Throw<object>(new ReplicateException(EnumReplicateExceptionType.TokenNotProvided));
        
        return await Predict(modelID, linkToAudio, model, this._tokens.GetRandom(), webhook);
    }

    public async Task<ReplicatePredictionResult> GetResult(string predictionID, string apiKey)
    {
        Response response = await new Request($"{this._endpoint}/predictions/{predictionID}")
            .AsGet()
            .WithAcceptJson()
            .WithHeader("Authorization", $"Token {apiKey}")
            .SendAsync();

        return await response.ParseJsonAsync<ReplicatePredictionResult>(false);
    }
}