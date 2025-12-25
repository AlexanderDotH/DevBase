using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;

namespace DevBase.Cryptography.BouncyCastle.Hashing.Verification;

/// <summary>
/// Verifies RSASSA-PKCS1-v1_5 signatures for tokens.
/// </summary>
/// <typeparam name="T">The digest algorithm to use (e.g., SHA256).</typeparam>
public class RsTokenVerifier<T> : AsymmetricTokenVerifier where T : IDigest
{
    /// <inheritdoc />
    protected override bool VerifySignature(byte[] content, byte[] signature, string publicKey)
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