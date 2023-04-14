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

public static class AsymmetricKeyParameterExtension
{
    public static byte[] PublicKeyToArray(this AsymmetricKeyParameter keyParameter)
    {
        if (keyParameter is ECPublicKeyParameters ecPublicKey)
        {
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(ecPublicKey);
            return publicKeyInfo.GetDerEncoded();
        }

        throw new ArgumentException("Unsupported public key type");
    }
    
    public static byte[] PrivateKeyToArray(this AsymmetricKeyParameter keyParameter)
    {
        if (keyParameter is ECPrivateKeyParameters ecPrivateKey)
        {
            return ecPrivateKey.D.ToByteArrayUnsigned();
        }

        throw new ArgumentException("Unsupported private key type");
    }
    
    public static AsymmetricKeyParameter ToECDHPublicKey(this byte[] keySequence)
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
    
    public static AsymmetricKeyParameter ToECDHPrivateKey(this byte[] keySequence)
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