using System.Text;
using DevBase.Requests.Constants;
using DevBase.Requests.Security.Token;

namespace DevBase.Requests.Validation;

public static class HeaderValidator
{

    public static ValidationResult ValidateBasicAuth(string headerValue)
    {
        if (string.IsNullOrWhiteSpace(headerValue))
            return ValidationResult.Fail("Basic authentication header is empty");

        string[] parts = headerValue.Split(' ', 2);
        if (parts.Length != 2 || !parts[0].AsSpan().Equals(AuthConstants.Basic.Span, StringComparison.OrdinalIgnoreCase))
            return ValidationResult.Fail("Invalid Basic authentication format");

        string base64Part = parts[1];
        if (!IsValidBase64(base64Part))
            return ValidationResult.Fail("Invalid Base64 encoding in Basic authentication");

        try
        {
            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64Part));
            if (!decoded.Contains(':'))
                return ValidationResult.Fail("Basic authentication must contain username:password");
        }
        catch
        {
            return ValidationResult.Fail("Failed to decode Basic authentication credentials");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateBearerAuth(string headerValue)
    {
        return ValidateBearerAuth(headerValue, verifySignature: false, secret: null, checkExpiration: false);
    }

    public static ValidationResult ValidateBearerAuth(
        string headerValue, 
        bool verifySignature, 
        string? secret, 
        bool checkExpiration = true)
    {
        if (string.IsNullOrWhiteSpace(headerValue))
            return ValidationResult.Fail("Bearer authentication header is empty");

        string[] parts = headerValue.Split(' ', 2);
        if (parts.Length != 2 || !parts[0].AsSpan().Equals(AuthConstants.Bearer.Span, StringComparison.OrdinalIgnoreCase))
            return ValidationResult.Fail("Invalid Bearer authentication format");

        string token = parts[1];
        if (string.IsNullOrWhiteSpace(token))
            return ValidationResult.Fail("Bearer token is empty");

        if (token.Length < 10)
            return ValidationResult.Fail("Bearer token is too short");

        if (!IsValidJwt(token))
            return ValidationResult.Success();

        return ValidateJwtToken(token, verifySignature, secret, checkExpiration);
    }

    public static ValidationResult ValidateJwtToken(
        string token, 
        bool verifySignature = false, 
        string? secret = null, 
        bool checkExpiration = true)
    {
        AuthenticationToken? jwt = AuthenticationToken.FromString(
            token, 
            verifyToken: verifySignature && !string.IsNullOrEmpty(secret), 
            tokenSecret: secret ?? "");

        if (jwt == null)
            return ValidationResult.Fail("Failed to parse JWT token");

        if (verifySignature && !string.IsNullOrEmpty(secret) && !jwt.Signature.Verified)
            return ValidationResult.Fail("JWT signature verification failed");

        if (checkExpiration && jwt.Payload.ExpiresAt != default && jwt.Payload.ExpiresAt < DateTime.UtcNow)
            return ValidationResult.Fail($"JWT token expired at {jwt.Payload.ExpiresAt:O}");

        if (checkExpiration && jwt.Payload.NotBefore != default && DateTime.UtcNow < DateTimeOffset.FromUnixTimeSeconds(jwt.Payload.NotBefore).UtcDateTime)
            return ValidationResult.Fail("JWT token not yet valid (nbf claim)");

        return ValidationResult.Success();
    }

    public static AuthenticationToken? ParseJwtToken(string token)
    {
        return AuthenticationToken.FromString(token, verifyToken: false);
    }

    public static AuthenticationToken? ParseAndVerifyJwtToken(string token, string secret)
    {
        return AuthenticationToken.FromString(token, verifyToken: true, tokenSecret: secret);
    }

    public static ValidationResult ValidateContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return ValidationResult.Fail("Content-Type is empty");

        // Extract MIME type without parameters (e.g., "application/json; charset=utf-8" -> "application/json")
        string mimeType = contentType.Split(';')[0].Trim();
        
        if (!IsValidMimeType(mimeType))
            return ValidationResult.Fail($"Invalid Content-Type format: {contentType}");

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateContentLength(string? contentLength, long actualLength)
    {
        if (string.IsNullOrWhiteSpace(contentLength))
            return ValidationResult.Success();

        if (!long.TryParse(contentLength, out long declaredLength))
            return ValidationResult.Fail("Content-Length must be a valid number");

        if (declaredLength != actualLength)
            return ValidationResult.Fail($"Content-Length mismatch: declared {declaredLength}, actual {actualLength}");

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateHost(string? host, Uri? requestUri)
    {
        if (string.IsNullOrWhiteSpace(host))
            return ValidationResult.Fail("Host header is empty");

        if (requestUri != null)
        {
            string expectedHost = requestUri.Port == 80 || requestUri.Port == 443
                ? requestUri.Host
                : $"{requestUri.Host}:{requestUri.Port}";

            if (!host.Equals(expectedHost, StringComparison.OrdinalIgnoreCase) &&
                !host.Equals(requestUri.Host, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Fail($"Host header '{host}' does not match request URI host '{expectedHost}'");
            }
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateUserAgent(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return ValidationResult.Fail("User-Agent is empty");

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateAccept(string? accept)
    {
        if (string.IsNullOrWhiteSpace(accept))
            return ValidationResult.Fail("Accept header is empty");

        if (accept.AsSpan().SequenceEqual(MimeConstants.Wildcard.Span))
            return ValidationResult.Success();

        string[] parts = accept.Split(',');
        foreach (string part in parts)
        {
            string mimeType = part.Split(';')[0].Trim();
            if (!mimeType.AsSpan().SequenceEqual(MimeConstants.Wildcard.Span) && !IsValidMimeType(mimeType))
                return ValidationResult.Fail($"Invalid Accept MIME type: {mimeType}");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateCookie(string? cookie)
    {
        if (string.IsNullOrWhiteSpace(cookie))
            return ValidationResult.Fail("Cookie header is empty");

        if (!IsValidCookie(cookie))
            return ValidationResult.Fail("Invalid cookie format");

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateAcceptEncoding(string? acceptEncoding)
    {
        if (string.IsNullOrWhiteSpace(acceptEncoding))
            return ValidationResult.Fail("Accept-Encoding header is empty");

        string[] parts = acceptEncoding.Split(',');
        
        foreach (string part in parts)
        {
            string encoding = part.Split(';')[0].Trim().ToLowerInvariant();
            if (!IsValidEncoding(encoding.AsSpan()))
                return ValidationResult.Fail($"Invalid Accept-Encoding value: {encoding}");
        }

        return ValidationResult.Success();
    }

    private static bool IsValidBase64(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        foreach (char c in value)
        {
            if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '+' || c == '/' || c == '='))
                return false;
        }
        return true;
    }

    private static bool IsValidJwt(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        int dotCount = 0;
        foreach (char c in token)
        {
            if (c == '.')
            {
                dotCount++;
                continue;
            }
            if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-' || c == '_'))
                return false;
        }
        return dotCount == 2;
    }

    private static bool IsValidMimeType(string mimeType)
    {
        if (string.IsNullOrEmpty(mimeType))
            return false;

        int slashIndex = mimeType.IndexOf('/');
        if (slashIndex <= 0 || slashIndex == mimeType.Length - 1)
            return false;

        for (int i = 0; i < mimeType.Length; i++)
        {
            char c = mimeType[i];
            if (i == slashIndex)
                continue;
            if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || 
                  c == '!' || c == '#' || c == '$' || c == '%' || c == '&' || c == '\'' || 
                  c == '*' || c == '+' || c == '.' || c == '^' || c == '_' || c == '`' || 
                  c == '|' || c == '~' || c == '-'))
                return false;
        }
        return true;
    }

    private static bool IsValidCookie(string cookie)
    {
        if (string.IsNullOrEmpty(cookie))
            return false;

        bool hasEquals = false;
        foreach (char c in cookie)
        {
            if (c == '=')
                hasEquals = true;
            if (char.IsWhiteSpace(c) && c != ' ')
                return false;
        }
        return hasEquals;
    }

    private static bool IsValidEncoding(ReadOnlySpan<char> encoding)
    {
        return encoding.Equals(EncodingConstants.Gzip.Span, StringComparison.OrdinalIgnoreCase) ||
               encoding.Equals(EncodingConstants.Deflate.Span, StringComparison.OrdinalIgnoreCase) ||
               encoding.Equals(EncodingConstants.Br.Span, StringComparison.OrdinalIgnoreCase) ||
               encoding.Equals(EncodingConstants.Identity.Span, StringComparison.OrdinalIgnoreCase) ||
               encoding.SequenceEqual("*");
    }
}
