using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace DevBase.Cryptography.BouncyCastle.Extensions;

/// <summary>
/// Provides extension methods for converting asymmetric key parameters to and from byte arrays.
/// </summary>
public static class AsymmetricKeyParameterExtension
{
    /// <summary>
    /// Converts an asymmetric public key parameter to its DER encoded byte array representation.
    /// </summary>
    /// <param name="keyParameter">The public key parameter.</param>
    /// <returns>The DER encoded byte array.</returns>
    /// <exception cref="ArgumentException">Thrown if the public key type is not supported.</exception>
    public static byte[] PublicKeyToArray(this AsymmetricKeyParameter keyParameter)
    {
        if (keyParameter is ECPublicKeyParameters ecPublicKey)
        {
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(ecPublicKey);
            return publicKeyInfo.GetDerEncoded();
        }

        throw new ArgumentException("Unsupported public key type");
    }
    
    /// <summary>
    /// Converts an asymmetric private key parameter to its unsigned byte array representation.
    /// </summary>
    /// <param name="keyParameter">The private key parameter.</param>
    /// <returns>The unsigned byte array representation of the private key.</returns>
    /// <exception cref="ArgumentException">Thrown if the private key type is not supported.</exception>
    public static byte[] PrivateKeyToArray(this AsymmetricKeyParameter keyParameter)
    {
        if (keyParameter is ECPrivateKeyParameters ecPrivateKey)
        {
            return ecPrivateKey.D.ToByteArrayUnsigned();
        }

        throw new ArgumentException("Unsupported private key type");
    }
    
    /// <summary>
    /// Converts a byte array to an ECDH public key parameter using the secp256r1 curve.
    /// </summary>
    /// <param name="keySequence">The byte array representing the public key.</param>
    /// <returns>The ECDH public key parameter.</returns>
    /// <exception cref="ArgumentException">Thrown if the byte array is invalid.</exception>
    public static AsymmetricKeyParameter ToEcdhPublicKey(this byte[] keySequence)
    {
        try
        {
            X9ECParameters ecParameters = ECNamedCurveTable.GetByName("secp256r1");
            ECDomainParameters domainParameters = new ECDomainParameters(ecParameters.Curve, ecParameters.G, ecParameters.N, ecParameters.H, ecParameters.GetSeed());

            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfo.GetInstance(Asn1Object.FromByteArray(keySequence));
            ECPublicKeyParameters publicKey = (ECPublicKeyParameters)PublicKeyFactory.CreateKey(publicKeyInfo);
            publicKey = new ECPublicKeyParameters(publicKey.Q, domainParameters);

            return publicKey;
        }
        catch (System.Exception ex)
        {
            throw new ArgumentException("Invalid public key bytes", ex);
        }
    }
    
    /// <summary>
    /// Converts a byte array to an ECDH private key parameter using the secp256r1 curve.
    /// </summary>
    /// <param name="keySequence">The byte array representing the private key.</param>
    /// <returns>The ECDH private key parameter.</returns>
    /// <exception cref="ArgumentException">Thrown if the byte array is invalid.</exception>
    public static AsymmetricKeyParameter ToEcdhPrivateKey(this byte[] keySequence)
    {
        try
        {
            X9ECParameters ecParameters = ECNamedCurveTable.GetByName("secp256r1");
            ECDomainParameters domainParameters = new ECDomainParameters(ecParameters.Curve, ecParameters.G, ecParameters.N, ecParameters.H, ecParameters.GetSeed());
            BigInteger privateKeyD = new BigInteger(1, keySequence);
            return new ECPrivateKeyParameters(privateKeyD, domainParameters);
        }
        catch (System.Exception e)
        {
            throw new ArgumentException("Invalid private key bytes", e);
        }
    }
}