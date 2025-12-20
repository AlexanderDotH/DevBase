using DevBase.Cryptography.BouncyCastle.Hashing;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;
using Org.BouncyCastle.Crypto.Digests;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.Hashing;

public class Ps256TokenVerifierTest
{
    private string Header { get; set; }
    private string Payload { get; set; }
    private string Signature { get; set; }
    private string PublicKey { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        this.Header = "eyJhbGciOiJQUzI1NiIsInR5cCI6IkpXVCJ9";
        this.Payload =
            "eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0";
        
        this.Signature = "fqMFtqMaCtKmkJKBkeVtVO9rLjfyCFzy7Mk9lOxQ_6nN1PHL9l5ybTfVXZgusW9NEk4P7uHFMcfHV56FlH-JDb11kuoIVm__P0fZamWz0d7uEauKF3bEn1nvpZkbDLiDDFLmNePYVIO9IAlpGCchEYozuFUQQaLNjFDkbHWz-ClFgRWygV3HllgfXLOHO6lrihvk8XOMR--pvkos9pE0z9kk3-cgOYVg8x5D43IG9xhMkZhd4u_efIAy9gX86k_zSSPDddLl30OJTw-HdHCpGCKjPQMJBAFQZYCQmMs84SenaMbehe7NfZqkNcESj28Gp-iCoqTXAcK9i1MrNnTn5g";
        
        this.PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzVKKuBKOWDKKYH7UyjGY
/2ImwObn7/MZjNgBfOrguNHsnOvpJZQRTs/TNZZmk8b0NZND5kllw1GtW+9p8ZYX
BpmyxOl15aI0l95rzoesvVY0abZ7tFsKg+bGgKm9ACnWcNayjVqZOCnf6apNJDA3
k8Bji7ZWkY52WgWrE/VbIPutEZYQDGsjEUps4GR4SaQx00E6hks99HJvM/KY0nnv
cdGVZRw5LG9voHJOSAN5cBaWZcC5GNXIcC+7C0hNqzRopzKyGAX9jaZOv/NDsb9B
c7xqy7PRxb3NOwHqrshEV7kjPDlhFnaKwkpUJro4ITa0NAiUK0VXn8vRF+gzLTfw
1QIDAQAB
-----END PUBLIC KEY-----";
    }

    [Test]
    public void VerifyPs256SignatureTest()
    {
        bool result = new PsTokenVerifier<Sha256Digest>().VerifySignature(
            this.Header, 
            this.Payload, 
            this.Signature, 
            this.PublicKey);
        
        Assert.That(result, Is.True);
    }
}
