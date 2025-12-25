using System.Text;
using DevBase.Extensions;
using Org.BouncyCastle.Crypto;

namespace DevBase.Cryptography.BouncyCastle.Hashing;

/// <summary>
/// Abstract base class for verifying asymmetric signatures of tokens.
/// </summary>
public abstract class AsymmetricTokenVerifier
{
    /// <summary>
    /// Gets or sets the encoding used for the token parts. Defaults to UTF-8.
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Verifies the signature of a token.
    /// </summary>
    /// <param name="header">The token header.</param>
    /// <param name="payload">The token payload.</param>
    /// <param name="signature">The token signature (Base64Url encoded).</param>
    /// <param name="publicKey">The public key to use for verification.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Verifies the signature of the content bytes using the provided public key.
    /// </summary>
    /// <param name="content">The content bytes (header + "." + payload).</param>
    /// <param name="signature">The signature bytes.</param>
    /// <param name="publicKey">The public key.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    protected abstract bool VerifySignature(byte[] content, byte[] signature, string publicKey);
}