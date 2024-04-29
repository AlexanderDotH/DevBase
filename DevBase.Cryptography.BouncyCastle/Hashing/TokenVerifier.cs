using System.Text;
using DevBase.Typography.Encoded;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace DevBase.Cryptography.BouncyCastle.Hashing;

public class TokenVerifier
{
    protected TokenVerifier() { }

    public static bool VerifySha256Signature(
        Base64EncodedAString content, 
        string signature,
        Base64EncodedAString secret) 
        => VerifySha256Signature(
            content.GetDecodedBuffer(), 
            Encoding.ASCII.GetBytes(signature), 
            secret.GetDecodedBuffer());
    
    public static bool VerifySha256Signature(byte[] content, byte[] signature, byte[] secret)
    {
        // Init hash with original content and the secret
        HMac hmac = new HMac(new Sha256Digest());
        hmac.Init(new KeyParameter(secret));
        hmac.BlockUpdate(content, 0, content.Length);
        
        // Calculates the content hash
        byte[] expectedHash = new byte[hmac.GetMacSize()]; 
        hmac.DoFinal(expectedHash, 0);

        // Compares result with the signature
        return Arrays.ConstantTimeAreEqual(expectedHash, signature);
    }
}