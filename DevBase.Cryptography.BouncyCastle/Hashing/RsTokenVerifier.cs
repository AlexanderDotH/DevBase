using System.Text;
using DevBase.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;

namespace DevBase.Cryptography.BouncyCastle.Hashing;

public class RsTokenVerifier<T> where T : IDigest
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
    
    private bool VerifySignature(byte[] content, byte[] signature, string publicKey)
    {
        IDigest digest = (IDigest)Activator.CreateInstance(typeof(T))!;

        using StringReader stringReader = new StringReader(publicKey);
        
        PemReader pemReader = new PemReader(stringReader);
        AsymmetricKeyParameter asymmetricKeyParameter = (AsymmetricKeyParameter)pemReader.ReadObject();
        
        stringReader.Close();
        
        RsaDigestSigner signer = new RsaDigestSigner(digest);
        signer.Init(false, asymmetricKeyParameter);
        signer.BlockUpdate(content, 0, content.Length);
        
        return signer.VerifySignature(signature);
    }
}