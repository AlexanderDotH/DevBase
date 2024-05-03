using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Es512TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Signature { get; set; }
    private string PublicKey { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJFUzUxMiIsInR5cCI6IkpXVCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        
        this.Signature = "ABjXHHyHa6ispKBsUCSixDbe8UnDBp9_amNk8Mg_UhdDoga2GgVrMVVDpPMDqztrko0LONkau4rnSLuEBwkBi0oQAOy-BF86XtHRe-PbSxRmmLA66KT3oXmDlBgabsVCDjPp1nTCRgCAEpZFUoQhPfcAzmA8iVWA8f8jFvU4_PXpWlOR";
        
        this.PublicKey = @"-----BEGIN PUBLIC KEY-----
MIGbMBAGByqGSM49AgEGBSuBBAAjA4GGAAQBUilpEQ3cROq1hLHSQZ4icsGomBRb
gTC4aYvElygmD8fk6L2hII8IbK0aG8JIQnDAyUBkD7OD9nV4OPd+xrvy7YUAo5CQ
ICM9xwPp2BR5R8LwqMAzGNZZvGC6DOSRnu4NrqRjCB3XAabVL3APVh7K/84QHpei
ocM9ODffKTDdc7wQ+Nk=
-----END PUBLIC KEY-----";
    }

    [Test]
    public void VerifyEs512SignatureTest()
    {
        bool result = new EsTokenVerifier<Sha512Digest>().VerifySignature(
            this.Header, 
            this.Payload, 
            this.Signature, 
            this.PublicKey);
        
        Assert.IsTrue(result);
    }
}