using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace DevBase.Cryptography.BouncyCastle.AES;

/// <summary>
/// Provides AES encryption and decryption functionality using GCM mode.
/// </summary>
public class AESBuilderEngine
{
    private SecureRandom _secureRandom;
    private byte[] _key;

    /// <summary>
    /// Initializes a new instance of the <see cref="AESBuilderEngine"/> class with a random key.
    /// </summary>
    public AESBuilderEngine()
    {
        this._secureRandom = new SecureRandom();
        this._secureRandom.SetSeed(GenerateRandom(128));

        this._key = GenerateRandom(32);
    }
    
    /// <summary>
    /// Encrypts the specified buffer using AES-GCM.
    /// </summary>
    /// <param name="buffer">The data to encrypt.</param>
    /// <returns>A byte array containing the nonce followed by the encrypted data.</returns>
    public byte[] Encrypt(byte[] buffer)
    {
        // Generate nonce
        byte[] nonce = new byte[12];
        this._secureRandom.NextBytes(nonce);

        // Create parameters
        GcmBlockCipher blockCipher = new GcmBlockCipher(new AesEngine());
        AeadParameters parameters = new AeadParameters(new KeyParameter(this._key), 128, nonce);
        
        // Initialize the engine
        blockCipher.Init(true, parameters);

        // Encrypt
        byte[] encrypted = new byte[blockCipher.GetOutputSize(buffer.Length)];
        int size = blockCipher.ProcessBytes(buffer, 0, buffer.Length, encrypted, 0);
        blockCipher.DoFinal(encrypted, size);

        // Write nonce + encrypted buffer
        using MemoryStream memoryStream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(memoryStream);
        
        writer.Write(nonce);
        writer.Write(encrypted);

        return memoryStream.ToArray();
    }

    /// <summary>
    /// Decrypts the specified buffer using AES-GCM.
    /// </summary>
    /// <param name="buffer">The data to decrypt, expected to contain the nonce followed by the ciphertext.</param>
    /// <returns>The decrypted data.</returns>
    public byte[] Decrypt(byte[] buffer)
    {
        using MemoryStream memoryStream = new MemoryStream(buffer);
        using BinaryReader reader = new BinaryReader(memoryStream);

        // Parse nonce and encrypted content
        byte[] nonce = reader.ReadBytes(12);
        byte[] encrypted = reader.ReadBytes(buffer.Length - 12);
        
        // Create parameters
        GcmBlockCipher blockCipher = new GcmBlockCipher(new AesEngine());
        AeadParameters parameters = new AeadParameters(new KeyParameter(this._key), 128, nonce);
        
        // Initialize the engine
        blockCipher.Init(false, parameters);
        
        // Decrypt
        byte[] decrypted = new byte[blockCipher.GetOutputSize(encrypted.Length)];
        int size = blockCipher.ProcessBytes(encrypted, 0, encrypted.Length, decrypted, 0);
        blockCipher.DoFinal(decrypted, size);

        return decrypted;
    }

    /// <summary>
    /// Encrypts the specified string using AES-GCM and returns the result as a Base64 string.
    /// </summary>
    /// <param name="data">The string to encrypt.</param>
    /// <returns>The encrypted data as a Base64 string.</returns>
    public string EncryptString(string data) =>
        Convert.ToBase64String(Encrypt(Encoding.ASCII.GetBytes(data)));

    /// <summary>
    /// Decrypts the specified Base64 encoded string using AES-GCM.
    /// </summary>
    /// <param name="encryptedData">The Base64 encoded encrypted data.</param>
    /// <returns>The decrypted string.</returns>
    public string DecryptString(string encryptedData) => Encoding.ASCII.GetString(Decrypt(Convert.FromBase64String(encryptedData)));

    /// <summary>
    /// Sets the encryption key.
    /// </summary>
    /// <param name="key">The key as a byte array.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetKey(byte[] key)
    {
        this._key = key;
        return this;
    }

    /// <summary>
    /// Sets the encryption key from a Base64 encoded string.
    /// </summary>
    /// <param name="key">The Base64 encoded key.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetKey(string key) => SetKey(Convert.FromBase64String(key));

    /// <summary>
    /// Sets a random encryption key.
    /// </summary>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetRandomKey() => SetKey(GenerateRandom(32));
    
    /// <summary>
    /// Sets the seed for the random number generator.
    /// </summary>
    /// <param name="seed">The seed as a byte array.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetSeed(byte[] seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }
    
    /// <summary>
    /// Sets the seed for the random number generator from a string.
    /// </summary>
    /// <param name="seed">The seed string.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetSeed(string seed) => SetSeed(Encoding.ASCII.GetBytes(seed));

    /// <summary>
    /// Sets a random seed for the random number generator.
    /// </summary>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetRandomSeed() => SetSeed(GenerateRandom(128));
    
    /// <summary>
    /// Generates a random byte array of the specified size.
    /// </summary>
    /// <param name="size">The size of the array.</param>
    /// <returns>A random byte array.</returns>
    private byte[] GenerateRandom(int size)
    {
        byte[] random = new byte[size];
        new SecureRandom().NextBytes(random);
        return random;
    }
}