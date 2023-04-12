using DevBase.Api.Apis.OpenAi.Json;
using DevBase.Generics;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;

namespace DevBase.Api.Apis.OpenAi;

public class OpenAi
{
    private readonly string _baseUrl;
    private readonly string _apiKey;

    public OpenAi(string apiKey)
    {
        this._baseUrl = "https://api.openai.com/v1";
        this._apiKey = apiKey;
    }

    public async Task<OpenAiTranscription> Transcribe(byte[] audioFile)
    {
        RequestData requestData = new RequestData(string.Format("{0}/audio/transcription", this._baseUrl));

        AList<FormKeypair> formKeyPairs = new AList<FormKeypair>();
        formKeyPairs.Add(new FormKeypair());

        requestData.AddFormData();
    }
}