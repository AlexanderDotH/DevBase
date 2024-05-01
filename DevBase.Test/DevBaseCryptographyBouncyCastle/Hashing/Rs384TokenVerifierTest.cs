using DevBase.Cryptography.BouncyCastle.Hashing;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Rs384TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Signature { get; set; }
    private string PublicKey { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJSUzM4NCIsInR5cCI6IkpXVCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        
        this.Signature = "K7mIQonGQ3_bfIxa8zUbgoNANub1w-L6RSvedRIMjIIUVoLX86rcli9tp_-WuS1dmy9hCy1vniHTuirK13gXcFwO-AAzIlxB-X2ksUZoe-LagKMPglnjp4djr46CTUyBtJErp9XWdK9Rt7A0nPPI0GBYDfiAXadcZpHTyysD_MVhHhKsHQvK3Vqs5D76BG6kx_Ze399TYR7yBCrb7iyeLi5CczyvqQyJJyMCntvi-cNR4OUwFsOe-bCErgy3Wz688hC2p6-drN2ppcrruVBaKI1VYRt3yDaC0JJSzRnOPARz-6DIiWsssB1ehXfSLRygQXxt86-Y245791yzxZFCPw";
        
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
    public void VerifyRs384SignatureTest()
    {
        bool result = new RsTokenVerifier<Sha384Digest>().VerifySignature(
            this.Header, 
            this.Payload, 
            this.Signature, 
            this.PublicKey);
        
        Assert.IsTrue(result);
    }
}