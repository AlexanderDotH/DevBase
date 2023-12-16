using DevBase.Api.Serializer;
using DevBase.Utilities;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Objects.Token;

public class AuthenticationToken
{
   
    public AuthenticationTokenHeader Header { get; private set; }
    public AuthenticationTokenPayload Payload { get; private set; }

    private AuthenticationToken(AuthenticationTokenHeader header, AuthenticationTokenPayload payload)
    {
        Header = header;
        Payload = payload;
    }

    public static AuthenticationToken FromString(string rawToken)
    {
        if (!rawToken.Contains("."))
            return null;
        
        string[] tokenElements = rawToken.Split(".");

        if (tokenElements.Length > 2)
            return null;

        string header = tokenElements[0];
        string payload = tokenElements[1];
    }

    private static AuthenticationTokenHeader ParseHeader(string header)
    {
        string decoded = StringUtils.DecodeBase64(header);

        if (String.IsNullOrEmpty(decoded))
            return null;
        
        JObject parsed = JObject.Parse(decoded);
    }
    
    
}