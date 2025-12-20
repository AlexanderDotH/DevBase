using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Ps512TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Signature { get; set; }
    private string PublicKey { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJQUzUxMiIsInR5cCI6IkpXVCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        
        this.Signature = "ppzEFkt8169ofoahYolnz3qXX-IiI2MTfTC9b1RRED2tf9OL1X_veLszyAiN3ATrgIFVJcRuM0FX72pBGpf04uKYX_tn3WliGZCy0es5PwhN3jkKHJNvrD7qruhxRm3uMNe69q_DLSK2_VQbav4f3Tqx3mcSxRbwzL91GRStYwRp7p9Y4i4_D1vnCzyTrP_VTUcNtbh0J1z3vOH6wy0z9WFF0B8PAgFHTN5FtRSAztBaYyLbA82pJE9Beg73C9coxvh6_IhyDat2WR8h_ACLXlFqGCDNs6EMnkD0ofYCfeL2sRHzlWmct7E4ZlUndNYHdjcivhYZUlho3EZ2p0237g";
        
        this.PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAra7JDUqJvMOExjMyxFGy
atcHmGFehY/5pFKcU+mLw5V/i1BkV2Vc+MpX3ulxGF7ERMk+B26e3ImyN4RE21G+
FPKX1JJIuqWeKifqirz94lKAWtXnBKm/H80PhzdPG5UhsukYJeqlkVnjbTbS/sHp
qpyONXry/4ZlJmVG2IRYr2iieZJzb0ZHBA7Nu70iUniMUI7cSLfDRiwxa/yYMZ5j
BDmgju+DP4JTYBnqeshPi5gv7wfTfawWs0m01R2KRktY2fR4mBOsPtuQ+eLMdYKL
VlpV977jTdmN+VKe3An1ztoY+AZeonYZ0GZe8tEOdMr0eEwOsB7ti0SQ74WT4wyW
iwIDAQAB
-----END PUBLIC KEY-----";
    }

    [Test]
    public void VerifyPs512SignatureTest()
    {
        bool result = new PsTokenVerifier<Sha512Digest>().VerifySignature(
            this.Header, 
            this.Payload, 
            this.Signature, 
            this.PublicKey);
        
        Assert.That(result, Is.True);
    }
}
