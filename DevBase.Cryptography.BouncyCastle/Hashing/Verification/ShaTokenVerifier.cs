using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace DevBase.Cryptography.BouncyCastle.Hashing.Verification;

public class ShaTokenVerifier<T> : SymmetricTokenVerifier<T> where T : IDigest
{
    protected override bool VerifySignature(byte[] content, byte[] signature, byte[] secret)
    {
        IDigest digest = (IDigest)Activator.CreateInstance(typeof(T))!;
        
        // Init hash with original content and the secret
        HMac hmac = new HMac(digest);
        hmac.Init(new KeyParameter(secret));
        hmac.BlockUpdate(content, 0, content.Length);
        
        // Calculates the content hash
        byte[] expectedHash = new byte[hmac.GetMacSize()]; 
        hmac.DoFinal(expectedHash, 0);

        // Compares result with the signature
        return Arrays.ConstantTimeAreEqual(expectedHash, signature);
    }
}