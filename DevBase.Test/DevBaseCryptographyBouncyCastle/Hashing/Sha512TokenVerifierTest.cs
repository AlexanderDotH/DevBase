using DevBase.Cryptography.BouncyCastle.Hashing;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Sha512TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Secret { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCIsImtpZCI6IjQyMCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        this.Secret = "i-like-jwt";
    }
    
    [Test]
    public void VerifySha512SignatureTest()
    {
        string signature = "z-5PlzRnZV00drt1M-vpJ2kd21s7LbfI7KxPQPHzMlOkMZHDElXdF4mbohA-uoeuCvcdqgvEpiqCdq6KLCR1sw";
        
        Assert.IsTrue(
            new ShaTokenVerifier<Sha512Digest>().VerifySignature(
                this.Header, 
                this.Payload, 
                signature, 
                this.Secret, 
                false));
    }
    
    [Test]
    public void VerifySha512EncodedSignatureTest()
    {
        string signature = "bjhSZhWIuckeQEceMd8vP0kDggi_vUpodW_9lKrWWyyXCxi0tHGt3oYKiIF4u7uxDj768y8KbCmCoKA40DtsnA";
        
        Assert.IsTrue(
            new ShaTokenVerifier<Sha512Digest>().VerifySignature(
                this.Header, 
                this.Payload, 
                signature, 
                this.Secret, 
                true));
    }
}