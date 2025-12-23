using System.Text.Json.Nodes;
using DevBase.Api.Apis.Musixmatch.Json;
using DevBase.Api.Apis.OpenAi.Json;
using DevBase.Api.Serializer;
using DevBase.Enums;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.ResponseData;
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
        RequestData requestData = new RequestData(string.Format("{0}/ws/1.1/account.post", this._authEndpoint), EnumRequestMethod.POST);

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
        
        requestData.AddContent(jObject.ToString());
        requestData.ContentTypeHolder.GetContentType(EnumContentType.APPLICATION_JSON);

        Request request = new Request(requestData);
        ResponseData responseData = await request.GetResponseAsync();
        
        return new JsonDeserializer<JsonMusixMatchAuthResponse>().Deserialize(responseData.GetContentAsString());
    }
}