using DevBase.Api.Apis.OpenAi.Json;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Generics;
using DevBase.Net.Core;
using DevBase.Net.Data.Body;

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
        string url = $"{this._baseUrl}/audio/transcriptions";

        RequestKeyValueListBodyBuilder form = new RequestKeyValueListBodyBuilder()
            .AddFile("file", audioFile)
            .AddText("model", "whisper-1")
            .AddText("response_format", "verbose_json");

        Response response = await new Request(url)
            .AsPost()
            .UseBearerAuthentication(this._apiKey)
            .WithForm(form)
            .WithTimeout(TimeSpan.FromMinutes(10))
            .SendAsync();

        return await response.ParseJsonAsync<OpenAiTranscription>(false);
    }
}