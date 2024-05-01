using DevBase.Cryptography.BouncyCastle.Hashing;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Es256TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Signature { get; set; }
    private string PublicKey { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        
        this.Signature = "cx0Jmnm2vJcLqZzcCkLilx35EgMD90D30VGWF6x79E-KH9i_KZspLVlrU6z1pwI7ee1v4TiiAMT3qmcplhxWBg";
        
        this.PublicKey = @"-----BEGIN PUBLIC KEY-----
MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE4M3lWA7oV+YHG6tUv5U3d86SqOKQ
DxnD54ZoQAj5Tkixe5fyp4EehhG6yqtyRauC9Fhcrky8+s2CuMHKpVZa7w==
-----END PUBLIC KEY-----";
    }

    [Test]
    public void VerifyEs256SignatureTest()
    {
        bool result = new EsTokenVerifier<Sha256Digest>().VerifySignature(
            this.Header, 
            this.Payload, 
            this.Signature, 
            this.PublicKey);
        
        Assert.IsTrue(result);
    }
}