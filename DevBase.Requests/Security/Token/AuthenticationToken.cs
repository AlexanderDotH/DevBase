using DevBase.Requests.Utils;
using DevBase.Typography.Encoded;
using Newtonsoft.Json.Linq;

namespace DevBase.Requests.Security.Token;

public class AuthenticationToken
{
    public AuthenticationTokenHeader Header { get; private set; }
    public AuthenticationTokenPayload Payload { get; private set; }
    
    public string Signature { get; private set; }
    public string RawToken { get; private set; }

    private AuthenticationToken(AuthenticationTokenHeader header, AuthenticationTokenPayload payload, string signature, string rawToken)
    {
        Header = header;
        Payload = payload;
        Signature = signature;
        RawToken = rawToken;
    }

    public static AuthenticationToken? FromString(string rawToken)
    {
        if (!rawToken.Contains("."))
            return null;
        
        string[] tokenElements = rawToken.Split(".");

        if (tokenElements.Length != 3)
            return null;

        Base64EncodedAString header = new Base64EncodedAString(tokenElements[0]);
        Base64EncodedAString payload = new Base64EncodedAString(tokenElements[1]);
        string signature = tokenElements[2];

        AuthenticationTokenHeader tokenHeader = ParseHeader(header)!;
        AuthenticationTokenPayload tokenPayload = ParsePayload(payload)!;

        return new AuthenticationToken(tokenHeader, tokenPayload, signature, rawToken);
    }

    private static AuthenticationTokenHeader? ParseHeader(Base64EncodedAString header)
    {
        string decoded = header.GetDecoded().ToString();

        if (string.IsNullOrEmpty(decoded))
            return null;
        
        JObject parsed = JObject.Parse(decoded);

        string algorithm;
        string type;
        string keyId;
        
        JsonUtils.TryGetString(parsed, "alg", out algorithm);
        JsonUtils.TryGetString(parsed, "typ", out type);
        JsonUtils.TryGetString(parsed, "kid", out keyId);

        AuthenticationTokenHeader tokenHeader = new AuthenticationTokenHeader()
        {
            Algorithm = algorithm,
            Type = type,
            KeyId = keyId,
            RawHeader = decoded
        };

        return tokenHeader;
    }
    
    private static AuthenticationTokenPayload? ParsePayload(Base64EncodedAString payload)
    {
        string decoded = payload.GetDecoded().ToString();

        if (string.IsNullOrEmpty(decoded))
            return null;
        
        JObject parsed = JObject.Parse(decoded);

        string issuer;
        DateTime issuedAt;
        DateTime expiresAt;

        JsonUtils.TryGetString(parsed, "iss", out issuer);
        JsonUtils.TryGetDateTime(parsed, "iat", out issuedAt);
        JsonUtils.TryGetDateTime(parsed, "exp", out expiresAt);

        AuthenticationTokenPayload tokenPayload = new AuthenticationTokenPayload()
        {
            Issuer = issuer,
            IssuedAt = issuedAt,
            ExpiresAt = expiresAt,
            RawPayload = decoded
        };
        
        return tokenPayload;
    }
}