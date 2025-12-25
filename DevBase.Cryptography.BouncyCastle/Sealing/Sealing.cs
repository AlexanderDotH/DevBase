using System.Text;
using DevBase.Cryptography.BouncyCastle.AES;
using DevBase.Cryptography.BouncyCastle.ECDH;
using DevBase.Cryptography.BouncyCastle.Extensions;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Tls;

namespace DevBase.Cryptography.BouncyCastle.Sealing;

/// <summary>
/// Provides functionality for sealing and unsealing messages using hybrid encryption (ECDH + AES).
/// </summary>
public class Sealing
{
    private byte[] _othersPublicKey;
    private byte[] _sharedSecret;
    private EcdhEngineBuilder _ecdhEngine;
    private AESBuilderEngine _aesEngine;

    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for sealing messages to a recipient.
    /// </summary>
    /// <param name="othersPublicKey">The recipient's public key.</param>
    public Sealing(byte[] othersPublicKey)
    {
        this._othersPublicKey = othersPublicKey;
        this._ecdhEngine = new EcdhEngineBuilder().GenerateKeyPair();
        
        this._sharedSecret = this._ecdhEngine.DeriveKeyPairs(this._othersPublicKey.ToEcdhPublicKey());
        this._aesEngine = new AESBuilderEngine().SetKey(this._sharedSecret);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for sealing messages to a recipient using Base64 encoded public key.
    /// </summary>
    /// <param name="othersPublicKey">The recipient's Base64 encoded public key.</param>
    public Sealing(string othersPublicKey) : this(Convert.FromBase64String(othersPublicKey)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for unsealing messages.
    /// </summary>
    /// <param name="publicKey">The own public key.</param>
    /// <param name="privateKey">The own private key.</param>
    public Sealing(byte[] publicKey, byte[] privateKey)
    {
        this._ecdhEngine = new EcdhEngineBuilder().FromExistingKeyPair(publicKey, privateKey);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for unsealing messages using Base64 encoded keys.
    /// </summary>
    /// <param name="publicKey">The own Base64 encoded public key.</param>
    /// <param name="privateKey">The own Base64 encoded private key.</param>
    public Sealing(string publicKey, string privateKey) : this(Convert.FromBase64String(publicKey),
        Convert.FromBase64String(privateKey)) {}
    
    /// <summary>
    /// Seals (encrypts) a message.
    /// </summary>
    /// <param name="unsealedMessage">The message to seal.</param>
    /// <returns>A byte array containing the sender's public key length, public key, and the encrypted message.</returns>
    public byte[] Seal(byte[] unsealedMessage)
    {
        using MemoryStream memoryStream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(memoryStream);

        byte[] sealedContent = this._aesEngine.Encrypt(unsealedMessage);
        byte[] ownPublicKey = this._ecdhEngine.PublicKey.PublicKeyToArray();
        
        writer.Write(ownPublicKey.Length);
        writer.Write(ownPublicKey);
        writer.Write(sealedContent);
        
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Seals (encrypts) a string message.
    /// </summary>
    /// <param name="unsealedMessage">The string message to seal.</param>
    /// <returns>A Base64 string containing the sealed message.</returns>
    public string Seal(string unsealedMessage) => Convert.ToBase64String(Seal(Encoding.ASCII.GetBytes(unsealedMessage)));
    
    /// <summary>
    /// Unseals (decrypts) a message.
    /// </summary>
    /// <param name="sealedMessage">The sealed message bytes.</param>
    /// <returns>The unsealed (decrypted) message bytes.</returns>
    public byte[] UnSeal(byte[] sealedMessage)
    {
        using MemoryStream memoryStream = new MemoryStream(sealedMessage);
        using BinaryReader reader = new BinaryReader(memoryStream);

        int publicKeySize = reader.ReadInt32();

        byte[] publicKey = reader.ReadBytes(publicKeySize);
        byte[] sealedByteSequence = reader.ReadBytes((int)(memoryStream.Length - memoryStream.Position));

        byte[] sharedSecret = this._ecdhEngine.DeriveKeyPairs(publicKey.ToEcdhPublicKey());

        if (this._sharedSecret == null)
        {
            this._sharedSecret = sharedSecret;
            this._aesEngine = new AESBuilderEngine().SetKey(this._sharedSecret);
        }
        
        byte[] unsealed = this._aesEngine.Decrypt(sealedByteSequence);
        return unsealed;
    }

    /// <summary>
    /// Unseals (decrypts) a Base64 encoded message string.
    /// </summary>
    /// <param name="sealedMessage">The Base64 encoded sealed message.</param>
    /// <returns>The unsealed (decrypted) string message.</returns>
    public string UnSeal(string sealedMessage) =>
        Encoding.ASCII.GetString(UnSeal(Convert.FromBase64String(sealedMessage)));
}