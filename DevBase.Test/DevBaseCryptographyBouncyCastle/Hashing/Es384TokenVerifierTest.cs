using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Es384TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Signature { get; set; }
    private string PublicKey { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJFUzM4NCIsInR5cCI6IkpXVCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        
        this.Signature = "0HXJ_rrgquV2M8OowE6mez4qF1o76IA7RY8dBin6eJXW4gIqoEh5MiTA_ZGfI8J4MTKnapZEowcq0TmBYxh31EcCHnA0djytLPa9Q7xoasuiaWxNosYcNY5eiC-PzlcM";
        
        this.PublicKey = @"-----BEGIN PUBLIC KEY-----
MHYwEAYHKoZIzj0CAQYFK4EEACIDYgAEhAJSkDoxAdYcVDI7TaMiI1I8DfLzu/Mp
Sa3ovbDeCCn6CnlnlrLijVZ5xtZPyKthcXfH4ktgJGvrY6lR+ic8uH1Y7dyPoNYD
kNvO6cBrD60+l5qqJp+MumaNKK4Vf39K
-----END PUBLIC KEY-----";
    }

    [Test]
    public void VerifyEs384SignatureTest()
    {
        bool result = new EsTokenVerifier<Sha384Digest>().VerifySignature(
            this.Header, 
            this.Payload, 
            this.Signature, 
            this.PublicKey);
        
        Assert.IsTrue(result);
    }
}