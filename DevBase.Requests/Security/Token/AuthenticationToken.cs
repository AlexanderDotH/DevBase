using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using DevBase.Extensions;
using DevBase.Generics;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Utils;
using DevBase.Typography.Encoded;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

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
        bool verifyToken = false, 
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

    #region Signing
 
    private static AuthenticationTokenSignature? ParseSignature(
        AuthenticationTokenHeader tokenHeader, 
        AuthenticationTokenPayload tokenPayload, 
        Base64EncodedAString signature, 
        bool shouldVerify, 
        string tokenSecret)
    {
        string decodedSignature = signature.UrlDecoded().GetDecoded().ToString();
        byte[] bDecodedSignature = signature.UrlDecoded().GetDecodedBuffer();

        if (string.IsNullOrEmpty(decodedSignature))
            return null;

        bool isJwtVerified = false;

        if (shouldVerify)
            isJwtVerified = VerifyToken(tokenHeader, tokenPayload, decodedSignature, tokenSecret);
        
        return new AuthenticationTokenSignature()
        {
            Signature = bDecodedSignature,
            Verified = isJwtVerified
        };
    }
    
    // Let's ignore the fact that the sha256, sha384 and sha512 secret can be encoded. Who does that?
    // That's the most unsecure way to code with dynamic datatypes but screw it.
    private static bool VerifyToken(
        AuthenticationTokenHeader tokenHeader, 
        AuthenticationTokenPayload tokenPayload, 
        string tokenSignature, 
        string tokenSecret)
    {
        if (string.IsNullOrEmpty(tokenHeader.Algorithm!))
            throw new TokenVerificationException(EnumTokenVerificationExceptionType.MissingField);

        dynamic verifier = ParseAlgorithmVerifier(tokenHeader.Algorithm);

        string header = tokenHeader.RawHeader!;
        string payload = tokenPayload.RawPayload!;

        return verifier.VerifySignature(header, payload, tokenSignature, tokenSecret);
    }

    private static dynamic ParseAlgorithmVerifier(string algorithm)
    {
        if (algorithm.Length != 5)
            throw new TokenVerificationException(EnumTokenVerificationExceptionType.InvalidLength);

        string algorithmName = algorithm.Substring(0, 2);
        string algorithmSize = algorithm.Substring(algorithm.Length - 3);

        switch (algorithmSize)
        {
            case "256":
                return algorithmName == "HS"
                    ? ParseSymmetricTokenVerifier<Sha256Digest>(algorithmName)
                    : ParseAsymmetricTokenVerifier<Sha256Digest>(algorithmName);
            case "384":
                return algorithmName == "HS"
                    ? ParseSymmetricTokenVerifier<Sha384Digest>(algorithmName)
                    : ParseAsymmetricTokenVerifier<Sha384Digest>(algorithmName);
            case "512":
                return algorithmName == "HS"
                    ? ParseSymmetricTokenVerifier<Sha512Digest>(algorithmName)
                    : ParseAsymmetricTokenVerifier<Sha512Digest>(algorithmName);

        }
        
        throw new TokenVerificationException(EnumTokenVerificationExceptionType.AlgorithmNotAvailable);
    }

    private static SymmetricTokenVerifier ParseSymmetricTokenVerifier<T>(string algorithmName) where T : IDigest
    {
        switch (algorithmName)
        {
            case "HS": return new ShaTokenVerifier<T>();
        }

        throw new TokenVerificationException(EnumTokenVerificationExceptionType.AlgorithmNotAvailable);
    }
    
    private static AsymmetricTokenVerifier ParseAsymmetricTokenVerifier<T>(string algorithmName) where T : IDigest
    {
        switch (algorithmName)
        {
            case "ES": return new EsTokenVerifier<T>();
            case "PS": return new PsTokenVerifier<T>();
            case "RS": return new RsTokenVerifier<T>();
        }

        throw new TokenVerificationException(EnumTokenVerificationExceptionType.AlgorithmNotAvailable);
    }    

    #endregion

    #region Header

    private static AuthenticationTokenHeader? ParseHeader(Base64EncodedAString header)
    {
        string decoded = header.GetDecoded().ToString();

        if (string.IsNullOrEmpty(decoded))
            return null;
        
        JObject parsed = JObject.Parse(decoded);

        KeyValuePair<string, string> algorithm;
        KeyValuePair<string, string> type;
        
        JsonUtils.TryGetString(parsed, "alg", out algorithm);
        JsonUtils.TryGetString(parsed, "typ", out type);
        
        AuthenticationTokenHeader tokenHeader = new AuthenticationTokenHeader()
        {
            Algorithm = algorithm.Value,
            Type = type.Value,
            RawHeader = decoded,
            Header = GetRaw(parsed)
        };

        return tokenHeader;
    }

    #endregion

    #region Payload

    private static AuthenticationTokenPayload? ParsePayload(Base64EncodedAString payload)
    {
        string decoded = payload.GetDecoded().ToString();

        if (string.IsNullOrEmpty(decoded))
            return null;
        
        JObject parsed = JObject.Parse(decoded);

        KeyValuePair<string, string> issuer;
        KeyValuePair<string, string> subject;
        KeyValuePair<string, string> audience;
        KeyValuePair<string, DateTime> expiresAt;
        KeyValuePair<string, int> notBefore;
        KeyValuePair<string, DateTime> issuedAt;
        KeyValuePair<string, string> jwtId;

        JsonUtils.TryGetString(parsed, "iss", out issuer);
        JsonUtils.TryGetString(parsed, "sub", out subject);
        JsonUtils.TryGetString(parsed, "aud", out audience);
        JsonUtils.TryGetDateTime(parsed, "exp", out expiresAt);
        JsonUtils.TryGetNumber(parsed, "nbf", out notBefore);
        JsonUtils.TryGetDateTime(parsed, "iat", out issuedAt);
        JsonUtils.TryGetString(parsed, "jti", out jwtId);
        
        AuthenticationTokenPayload tokenPayload = new AuthenticationTokenPayload()
        {
            Issuer = issuer.Value,
            Subject = subject.Value,
            Audience = audience.Value,
            ExpiresAt = expiresAt.Value,
            NotBefore = notBefore.Value,
            IssuedAt = issuedAt.Value,
            Id = jwtId.Value,
            RawPayload = decoded,
            Claims = GetRaw(parsed)
        };
        return tokenPayload;
    }

    #endregion
    
    private static Dictionary<string, object>? GetRaw(JObject? parsed)
    {
        Dictionary<string, object> rawHeader;

        JsonUtils.TryGetEntries(parsed!, out rawHeader);
        
        return rawHeader;
    }
}