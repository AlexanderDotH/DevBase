using System.Text;
using DevBase.Extensions;
using Org.BouncyCastle.Crypto;

namespace DevBase.Cryptography.BouncyCastle.Hashing;

public abstract class AsymmetricTokenVerifier<T> where T : IDigest
{
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    public bool VerifySignature(string header, string payload, string signature, string publicKey)
    {
        byte[] bSignature = signature
            .ToBase64()
            .UrlDecoded()
            .GetDecodedBuffer();

        byte[] bHeader = this.Encoding.GetBytes(header);
        byte[] bSeparator = this.Encoding.GetBytes(".");
        byte[] bPayload = this.Encoding.GetBytes(payload);

        byte[] bContent = new byte[bHeader.Length + bSeparator.Length + bPayload.Length];
        
        bHeader.CopyTo(bContent, 0);
        bSeparator.CopyTo(bContent, bHeader.Length);
        bPayload.CopyTo(bContent, bHeader.Length + bSeparator.Length);

        return VerifySignature(bContent, bSignature, publicKey);
    }

    protected abstract bool VerifySignature(byte[] content, byte[] signature, string publicKey);
}