using DevBase.Api.Apis.OpenAi.Json;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Generics;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;

namespace DevBase.Api.Apis.OpenAi;

public class OpenAi : ApiClient
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
        RequestData requestData = new RequestData(string.Format("{0}/audio/transcriptions", this._baseUrl), EnumRequestMethod.POST);

        requestData.AddAuthMethod(new Auth(this._apiKey, EnumAuthType.OAUTH2));
        
        AList<MultipartElement> formData = new AList<MultipartElement>();
        formData.Add(new MultipartElement("file", audioFile));
        formData.Add(new MultipartElement("model", "whisper-1"));
        formData.Add(new MultipartElement("response_format", "verbose_json"));

        requestData.AddMultipartFormData(formData);

        requestData.SetAccept(EnumCharsetType.ALL);
        requestData.Timeout = TimeSpan.FromMinutes(10);
        
        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();

        return new JsonDeserializer<OpenAiTranscription>().Deserialize(responseData.GetContentAsString());
    }
}