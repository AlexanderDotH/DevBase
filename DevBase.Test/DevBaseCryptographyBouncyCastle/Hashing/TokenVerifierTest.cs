using System.Text;
using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Extensions;
using DevBase.Typography.Encoded;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class TokenVerifierTest
{
    [Test]
    public void VerifySha256SignatureTest()
    {
        string header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjQyMCJ9";
        string payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        string signature = "HLtXQ7fbXS7RF6pJy-LkjZgfWBU_BE_85F8-A1o1efA";
        string secret = "i-like-jwt";
        
        byte[] bSignature = signature.ToBase64().UrlDecoded().GetDecodedBuffer();
        
        byte[] bSecret = Encoding.UTF8.GetBytes(secret);
        byte[] content = Encoding.UTF8.GetBytes($"{header}.{payload}");
        
        Assert.IsTrue(TokenVerifier.VerifySha256Signature(content, bSignature, bSecret));
    }
    
    private static string Base64UrlDecode(string input)
    {
        return input.Replace('-', '+').Replace('_', '/').PadRight(input.Length + (4 - input.Length % 4) % 4, '=');
    }
}