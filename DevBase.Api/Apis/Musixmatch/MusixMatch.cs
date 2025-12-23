using System.Text.Json.Nodes;
using DevBase.Api.Apis.Musixmatch.Json;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Net.Core;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.Musixmatch;

public class MusixMatch : ApiClient
{
    private readonly string _authEndpoint;

    public MusixMatch()
    {
        this._authEndpoint = "https://account.musixmatch.com";
    }

    public async Task<JsonMusixMatchAuthResponse> Login(string email, string password)
    {
        string url = $"{this._authEndpoint}/ws/1.1/account.post";

        JObject jObject = new JObject
        {
            {
                "credential_list", new JArray
                {
                    new JObject
                    {
                        {
                            "credential", new JObject
                            {
                                {"action", "login"},
                                {"email", email},
                                {"password", password},
                                {"type", "mxm"}
                            }
                        }
                    }
                }
            }
        };
        
        Response response = await new Request(url)
            .AsPost()
            .WithJsonBody(jObject.ToString())
            .SendAsync();
        
        return await response.ParseJsonAsync<JsonMusixMatchAuthResponse>(false);
    }
}