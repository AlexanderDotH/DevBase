using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

/// <summary>
/// Tests for SHA256 Token Verifier.
/// </summary>
/// <summary>
/// Tests for SHA256 Token Verifier.
/// </summary>
public class Sha256TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Secret { get; set; }
    
    /// <summary>
    /// Sets up test data.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjQyMCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        this.Secret = "i-like-jwt";
    }
    
    /// <summary>
    /// Tests verification of a SHA256 signature (unencoded secret).
    /// </summary>
    [Test]
    public void VerifySha256SignatureTest()
    {
        string signature = "HLtXQ7fbXS7RF6pJy-LkjZgfWBU_BE_85F8-A1o1efA";
        
        Assert.That(
            new ShaTokenVerifier<Sha256Digest>().VerifySignature(
                this.Header, 
                this.Payload, 
                signature, 
                this.Secret, 
                false), Is.True);
    }
    
    /// <summary>
    /// Tests verification of a SHA256 signature (base64 encoded secret).
    /// </summary>
    [Test]
    public void VerifySha256EncodedSignatureTest()
    {
        string signature = "B7e4nqPd3ggaAAEq4iA9fUPjyJM2dWZyeFDyzDnzmG4";
        
        Assert.That(
            new ShaTokenVerifier<Sha256Digest>().VerifySignature(
                this.Header, 
                this.Payload, 
                signature, 
                this.Secret, 
                true), Is.True);
    }
}
