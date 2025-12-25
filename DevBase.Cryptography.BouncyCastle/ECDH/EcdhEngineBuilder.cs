using DevBase.Cryptography.BouncyCastle.Exception;
using DevBase.Cryptography.BouncyCastle.Extensions;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;

namespace DevBase.Cryptography.BouncyCastle.ECDH;

/// <summary>
/// Provides functionality for building and managing ECDH (Elliptic Curve Diffie-Hellman) key pairs and shared secrets.
/// </summary>
public class EcdhEngineBuilder
{
    private SecureRandom _secureRandom;
    private AsymmetricCipherKeyPair _keyPair;

    /// <summary>
    /// Initializes a new instance of the <see cref="EcdhEngineBuilder"/> class.
    /// </summary>
    public EcdhEngineBuilder()
    {
        this._secureRandom = new SecureRandom();
    }

    /// <summary>
    /// Generates a new ECDH key pair using the secp256r1 curve.
    /// </summary>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    public EcdhEngineBuilder GenerateKeyPair()
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

    /// <summary>
    /// Loads an existing ECDH key pair from byte arrays.
    /// </summary>
    /// <param name="publicKey">The public key bytes.</param>
    /// <param name="privateKey">The private key bytes.</param>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    public EcdhEngineBuilder FromExistingKeyPair(byte[] publicKey, byte[] privateKey)
    {
        this._keyPair = new AsymmetricCipherKeyPair(publicKey.ToEcdhPublicKey(), privateKey.ToEcdhPrivateKey());
        return this;
    }

    /// <summary>
    /// Loads an existing ECDH key pair from Base64 encoded strings.
    /// </summary>
    /// <param name="publicKey">The Base64 encoded public key.</param>
    /// <param name="privateKey">The Base64 encoded private key.</param>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    public EcdhEngineBuilder FromExistingKeyPair(string publicKey, string privateKey) =>
        FromExistingKeyPair(Convert.FromBase64String(publicKey), Convert.FromBase64String(privateKey));

    /// <summary>
    /// Derives a shared secret from the current private key and the provided public key.
    /// </summary>
    /// <param name="publicKey">The other party's public key.</param>
    /// <returns>The derived shared secret as a byte array.</returns>
    /// <exception cref="KeypairNotFoundException">Thrown if no key pair has been generated or loaded.</exception>
    public byte[] DeriveKeyPairs(AsymmetricKeyParameter publicKey)
    {
        if (this._keyPair == null)
            throw new KeypairNotFoundException("Keypair not found use \"GenerateKeyPair\" to generate a keypair");
        
        IBasicAgreement agreement = AgreementUtilities.GetBasicAgreement("ECDH");
        agreement.Init(this._keyPair.Private);
        
        BigInteger derivedSharedSecret = agreement.CalculateAgreement(publicKey);
        return derivedSharedSecret.ToByteArrayUnsigned();
    }

    /// <summary>
    /// Sets the seed for the random number generator using a long value.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    private EcdhEngineBuilder SetSeed(long seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }
    
    /// <summary>
    /// Sets the seed for the random number generator using a byte array.
    /// </summary>
    /// <param name="seed">The seed bytes.</param>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    private EcdhEngineBuilder SetSeed(byte[] seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }

    /// <summary>
    /// Gets the public key of the current key pair.
    /// </summary>
    public AsymmetricKeyParameter PublicKey
    {
        get => this._keyPair.Public;
    }
    
    /// <summary>
    /// Gets the private key of the current key pair.
    /// </summary>
    public AsymmetricKeyParameter PrivateKey
    {
        get => this._keyPair.Private;
    }
}