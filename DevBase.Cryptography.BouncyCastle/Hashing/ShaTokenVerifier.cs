using System.Text;
using DevBase.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace DevBase.Cryptography.BouncyCastle.Hashing;

public class ShaTokenVerifier<T> where T : IDigest
{
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    public bool VerifySignature(string header, string payload, string signature, string secret, bool isSecretEncoded = false)
    {
        byte[] bSignature = signature
            .ToBase64()
            .UrlDecoded()
            .GetDecodedBuffer();

        byte[] bSecret = isSecretEncoded ? 
            secret.ToBase64().UrlDecoded().GetDecodedBuffer() : 
            this.Encoding.GetBytes(secret);

        byte[] bHeader = this.Encoding.GetBytes(header);
        byte[] bSeparator = this.Encoding.GetBytes(".");
        byte[] bPayload = this.Encoding.GetBytes(payload);

        byte[] bContent = new byte[bHeader.Length + bSeparator.Length + bPayload.Length];
        
        bHeader.CopyTo(bContent, 0);
        bSeparator.CopyTo(bContent, bHeader.Length);
        bPayload.CopyTo(bContent, bHeader.Length + bSeparator.Length);

        return VerifySignature(bContent, bSignature, bSecret);
    }
    
    private bool VerifySignature(byte[] content, byte[] signature, byte[] secret)
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