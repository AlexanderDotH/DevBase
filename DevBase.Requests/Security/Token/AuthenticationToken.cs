using System.Security.Cryptography;
using DevBase.Extensions;
using DevBase.Generics;
using DevBase.Requests.Utils;
using DevBase.Typography.Encoded;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace DevBase.Requests.Security.Token;

public class AuthenticationToken
{
    public AuthenticationTokenHeader Header { get; private set; }
    public AuthenticationTokenPayload Payload { get; private set; }
    public AuthenticationTokenSignature Signature { get; private set; }
    
    public string RawToken { get; private set; }

    private AuthenticationToken(
        AuthenticationTokenHeader header, 
        AuthenticationTokenPayload payload, 
        AuthenticationTokenSignature signature, 
        string rawToken
        )
    {
        Header = header;
        Payload = payload;
        Signature = signature;
        RawToken = rawToken;
    }

    public static AuthenticationToken? FromString(
        string rawToken, 
        bool verifyToken = true, 
        string tokenSecret = ""
        )
    {
        if (!rawToken.Contains("."))
            return null;
        
        string[] tokenElements = rawToken.Split(".");

        if (tokenElements.Length != 3)
            return null;

        AuthenticationTokenHeader tokenHeader = ParseHeader(tokenElements[0].ToBase64())!;
        AuthenticationTokenPayload tokenPayload = ParsePayload(tokenElements[1].ToBase64())!;
        AuthenticationTokenSignature tokenSignature = ParseSignature(tokenHeader, tokenPayload, tokenElements[2].ToBase64(), verifyToken, tokenSecret)!;

        return new AuthenticationToken(tokenHeader, tokenPayload, tokenSignature, rawToken);
    }

    private static AuthenticationTokenSignature? ParseSignature(
        AuthenticationTokenHeader tokenHeader, 
        AuthenticationTokenPayload tokenPayload, 
        Base64EncodedAString signature, 
        bool shouldVerify, 
        string tokenSecret)
    {
        string decoded = signature.GetDecoded().ToString();

        if (string.IsNullOrEmpty(decoded))
            return null;

        bool isJwtVerified = false;

        if (shouldVerify)
        {
            byte[] publicKey = signature.GetDecodedBuffer();
            byte[] bContent = null;
        }
        
        return new AuthenticationTokenSignature()
        {
            Signature = decoded,
            Verified = isJwtVerified
        };
    }
    
    private static AuthenticationTokenHeader? ParseHeader(Base64EncodedAString header)
    {
        string decoded = header.GetDecoded().ToString();

        if (string.IsNullOrEmpty(decoded))
            return null;
        
        JObject parsed = JObject.Parse(decoded);

        KeyValuePair<string, string> algorithm;
        KeyValuePair<string, string> type;
        KeyValuePair<string, string> keyId;
        
        JsonUtils.TryGetString(parsed, "alg", out algorithm);
        JsonUtils.TryGetString(parsed, "typ", out type);
        JsonUtils.TryGetString(parsed, "kid", out keyId);
        
        AuthenticationTokenHeader tokenHeader = new AuthenticationTokenHeader()
        {
            Algorithm = algorithm.Value,
            Type = type.Value,
            KeyId = keyId.Value,
            RawHeader = decoded,
            Header = GetRaw(parsed)
        };

        return tokenHeader;
    }
    
    private static AuthenticationTokenPayload? ParsePayload(Base64EncodedAString payload)
    {
        string decoded = payload.GetDecoded().ToString();

        if (string.IsNullOrEmpty(decoded))
            return null;
        
        JObject parsed = JObject.Parse(decoded);

        KeyValuePair<string, string> issuer;
        KeyValuePair<string, DateTime> issuedAt;
        KeyValuePair<string, DateTime> expiresAt;

        JsonUtils.TryGetString(parsed, "iss", out issuer);
        JsonUtils.TryGetDateTime(parsed, "iat", out issuedAt);
        JsonUtils.TryGetDateTime(parsed, "exp", out expiresAt);

        AuthenticationTokenPayload tokenPayload = new AuthenticationTokenPayload()
        {
            Issuer = issuer.Value,
            IssuedAt = issuedAt.Value,
            ExpiresAt = expiresAt.Value,
            RawPayload = decoded,
            Payload = GetRaw(parsed)
        };
        
        return tokenPayload;
    }
    
    private static List<KeyValuePair<string, object>>? GetRaw(JObject? parsed)
    {
        AList<KeyValuePair<string, object>> rawHeader;

        JsonUtils.TryGetEntries(parsed!, out rawHeader);
        
        return rawHeader.GetAsList();
    }
}