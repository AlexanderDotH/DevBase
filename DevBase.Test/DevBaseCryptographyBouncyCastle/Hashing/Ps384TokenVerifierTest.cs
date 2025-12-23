using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Ps384TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Signature { get; set; }
    private string PublicKey { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJQUzM4NCIsInR5cCI6IkpXVCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        
        this.Signature = "QWoddvAK6yS4ozR8orVHGnZluW--pgc1GiQhtVKJkuk-oT1ASFhe38TBaYzhyqEd4rFtSmueeCcceXZ2dSBw7oMX0KuVyWkuM8f2QSC-X4dZ5G5VtEuiKTi8wxH8b4prGTc3SvTx9JBAwxshGYnhfb5R3Yz6yxdmgaN5_3TXG4FBWcpvnvFG-jGgcB-Khi0K-5H5xAhf6vtTfEUTR5AhdEPDJUCz-qdD6vyQzh_wB2w-BZDVayfWyhRhRUPGCx2nAWz8SVA2NYxvPzI1FGdC3jAoE3bZkvLgbCPsZAH1X8lclSCXnC1ZtXYTWN19JCODBrhfzchXS_B1FFgSHNHTCw";
        
        this.PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxhVLitu7hxuHewdKddhC
0bT4zG5f05YbMdx4pCMB8sr+ShitH43LuErKXGKSFQlaM50v6riZVb93YremGBpr
azmHGHv3mNPuiLwFZy8AZ+AC3ON9N0RHmfl8Tp/8AttX3zB7D2CYbh7SitLlRekc
gh/D+/s+mQ3NuZAKAfbB0Ys11PTvQXWiVP0Dz6+7Ocg9E1TwutwSEF906yzgl804
gZTd46U5j72Fed3TxXQ7Nfd27nYVJZfPn10+sT2PNoBz6Bsr1Bv3Ulg4oabYNOD0
BodWzgVq2+1pSTuKDhogWcxTUbxJZwhL8QdrNRFDj0YgmwtWxwkwc2EIC9yEtfD/
/wIDAQAB
-----END PUBLIC KEY-----";
    }

    [Test]
    public void VerifyPs384SignatureTest()
    {
        bool result = new PsTokenVerifier<Sha384Digest>().VerifySignature(
            this.Header, 
            this.Payload, 
            this.Signature, 
            this.PublicKey);
        
        Assert.That(result, Is.True);
    }
}
