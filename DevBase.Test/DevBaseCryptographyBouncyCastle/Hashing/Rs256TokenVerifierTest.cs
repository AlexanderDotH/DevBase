using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Rs256TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Signature { get; set; }
    private string PublicKey { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        
        this.Signature = "DXposvuuCxblh1NauIQGKU_SZ26SMFTYb_DPm2Vqh-ZSz2B3KpitavHdq4_HIvXaAgLqmV6AVMdiMOpUKVaAWRAyyG49LQCiez814ENe8vHqXphuMhy6POjN_cUoaeAJV_n1uSEZ-vo2Dv-InFrkctkij65y3c78WmeqmKytWoTWvs-e397fkMGkJWMEa21L9WqbaiMIztfV94ypL93TDXhLatmxG9kqFvb4BWMzA-pCdL5j4Lmm7qxlUVVuUDMF8WKLw6MB7X6GmPefK0jj3WMqECh-j_PRIhF0Ec5kbApjtBWeFu3ggc-LvO1GWys263qZXBaOi3nTQxOABMvRpA";
        
        this.PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAioaFh+rb68+32rN5h5jV
5Jl3p8hEBey4BoIlo7swQ04pT/Nb/5E3To2vXIw3rI6VUcxoAfMhqMiVHl+4xHB4
QC44l7IwnsSGzWrGpbiGoOFksQMsqAa0eUeA/VR0+KRo1bnQIXYiW3hXfQ2lncsD
5m57PN5RwPmEQmkBjjj2825Dcz4T3gm7d0B8/p0qinXD2X30A7ZQhnxiFVvw7z7I
qOEQjakchRa9upoGXbOn8y0IMO0m2fvkhdbG7yncFAohXIgNn8s4KaikQjGVNmdZ
SRg4bYQeBrEDenPaUETqdETCS2ct4SqBNuoLk0LTKW5DuRCuLKczdAIULqojozqq
qwIDAQAB
-----END PUBLIC KEY-----";
    }

    [Test]
    public void VerifyRs256SignatureTest()
    {
        bool result = new RsTokenVerifier<Sha256Digest>().VerifySignature(
                this.Header, 
                this.Payload, 
                this.Signature, 
                this.PublicKey);
        
        Assert.IsTrue(result);
    }
}