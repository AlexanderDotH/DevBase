using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Rs512TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Signature { get; set; }
    private string PublicKey { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJSUzUxMiIsInR5cCI6IkpXVCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";

        this.Signature = "S0HJ6pQtrXJkWUB_G-miVvlg9CT6I5R0YTwrIYd6hmvlUi2NO828m_qeaapTTEnlqua4GBGFHAhYLkq-nEkhnZ7ZlUGrukPmivryyo7o_HHCkN50KoWxR3458Jexr0PsxpSkwIFqz9CqlQW7vjTKG4Dd8t6aG05ON-okBRjb-oiETEQK-f6H8WZjOT3xMV8-vCSN2Z2ci2099G95GzMIuzUYcNrpfO4NnLrw2WVY0VE7vLnRB2NJ4usF8gfyv5W0NOxjIPbnSsRDJ44EjfNRC-CSQTFFyhQjyzAwwvFExz5xUI-Uyvh2i9a42PreNEFv-3vbmeZr8u5RoQnZVoE4tw";
        
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
    public void VerifyRs512SignatureTest()
    {
        bool result = new RsTokenVerifier<Sha512Digest>().VerifySignature(
            this.Header, 
            this.Payload, 
            this.Signature, 
            this.PublicKey);
        
        Assert.That(result, Is.True);
    }
}
