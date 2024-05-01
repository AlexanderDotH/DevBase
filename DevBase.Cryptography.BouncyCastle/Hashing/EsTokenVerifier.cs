using System.Text;
using DevBase.Extensions;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace DevBase.Cryptography.BouncyCastle.Hashing;

public class EsTokenVerifier<T> where T : IDigest
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
        ECPublicKeyParameters ecPublicKeyParameters = (ECPublicKeyParameters)pemReader.ReadObject();
        
        stringReader.Close();
        
        DsaDigestSigner signer = new DsaDigestSigner(new ECDsaSigner(), digest);
        signer.Init(false, ecPublicKeyParameters);
        signer.BlockUpdate(content, 0, content.Length);

        byte[] asn1Signature = ToAsn1Der(signature);
        return signer.VerifySignature(asn1Signature);
    }

    private byte[] ToAsn1Der(byte[] p1363Signature)
    {
        int len = p1363Signature.Length / 2;
        
        BigInteger r = new BigInteger(1, p1363Signature.Take(len).ToArray());
        BigInteger s = new BigInteger(1, p1363Signature.Skip(len).ToArray());

        DerSequence seq = new DerSequence(
            new DerInteger(r),
            new DerInteger(s));

        return seq.GetDerEncoded();
    }
}