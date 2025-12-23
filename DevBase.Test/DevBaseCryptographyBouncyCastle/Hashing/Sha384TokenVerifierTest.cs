using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Sha384TokenVerifierTest
{
    
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Secret { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJIUzM4NCIsInR5cCI6IkpXVCIsImtpZCI6IjQyMCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        this.Secret = "i-like-jwt";
    }
    
    [Test]
    public void VerifySha384SignatureTest()
    {
        string signature = "6JHvXXRl_zhFbTysXc_2QQLLTwDv0mhHwluLRdrpq-AQjL-k140wrEH4OJ4-o4XH";
        
        Assert.That(
            new ShaTokenVerifier<Sha384Digest>().VerifySignature(
                this.Header, 
                this.Payload, 
                signature, 
                this.Secret, 
                false), Is.True);
    }
    
    [Test]
    public void VerifySha384EncodedSignatureTest()
    {
        string signature = "ihVYe7AM20XGONm1VAdhL170wSu75N17jubkdIyk-pZ1qWYBDgOp-uFaNCyCCTnw";
        
        Assert.That(
            new ShaTokenVerifier<Sha384Digest>().VerifySignature(
                this.Header, 
                this.Payload, 
                signature, 
                this.Secret, 
                true), Is.True);
    }
}
