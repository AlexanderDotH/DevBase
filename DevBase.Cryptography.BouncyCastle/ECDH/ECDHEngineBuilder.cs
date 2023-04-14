using DevBase.Cryptography.BouncyCastle.Extensions;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace DevBase.Cryptography.BouncyCastle.ECDH;

public class ECDHEngineBuilder
{
    private SecureRandom _secureRandom;
    private AsymmetricCipherKeyPair _keyPair;

    public ECDHEngineBuilder()
    {
        this._secureRandom = new SecureRandom();
    }

    public ECDHEngineBuilder GenerateKeyPair()
    {
        // Create parameters
        X9ECParameters parameters = ECNamedCurveTable.GetByName("secp256r1");
        ECDomainParameters domainParameters = new ECDomainParameters(parameters);
        ECKeyGenerationParameters keyGenerationParameters = new ECKeyGenerationParameters(domainParameters, this._secureRandom);
        
        // Generate keypair
        IAsymmetricCipherKeyPairGenerator keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDH");
        keyPairGenerator.Init(keyGenerationParameters);
        this._keyPair = keyPairGenerator.GenerateKeyPair();

        return this;
    }

    public ECDHEngineBuilder FromExistingKeyPair(byte[] publicKey, byte[] privateKey)
    {
        this._keyPair = new AsymmetricCipherKeyPair(publicKey.ToECDHPublicKey(), privateKey.ToECDHPrivateKey());
        return this;
    }

    public ECDHEngineBuilder FromExistingKeyPair(string publicKey, string privateKey) =>
        FromExistingKeyPair(Convert.FromBase64String(publicKey), Convert.FromBase64String(privateKey));

    public byte[] DeriveKeyPairs(AsymmetricKeyParameter publicKey)
    {
        if (this._keyPair == null)
            throw new System.Exception("Keypair not found use \"GenerateKeyPair\" to generate a keypair");
        
        IBasicAgreement agreement = AgreementUtilities.GetBasicAgreement("ECDH");
        agreement.Init(this._keyPair.Private);
        
        BigInteger derivedSharedSecret = agreement.CalculateAgreement(publicKey);
        return derivedSharedSecret.ToByteArrayUnsigned();
    }

    private ECDHEngineBuilder SetSeed(long seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }
    
    private ECDHEngineBuilder SetSeed(byte[] seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }

    public AsymmetricKeyParameter PublicKey
    {
        get => this._keyPair.Public;
    }
    
    public AsymmetricKeyParameter PrivateKey
    {
        get => this._keyPair.Private;
    }
}