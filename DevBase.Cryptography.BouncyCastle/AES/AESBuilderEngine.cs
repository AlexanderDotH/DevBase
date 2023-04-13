using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace DevBase.Cryptography.BouncyCastle.AES;

public class AESBuilderEngine
{
    private SecureRandom _secureRandom;
    private byte[] _key;

    public AESBuilderEngine()
    {
        this._secureRandom = new SecureRandom();
        this._secureRandom.SetSeed(GenerateRandom(128));

        this._key = GenerateRandom(32);
    }
    
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

    public string EncryptString(string data) =>
        Convert.ToBase64String(Encrypt(Encoding.ASCII.GetBytes(data)));

    public string DecryptString(string encryptedData) => Encoding.ASCII.GetString(Decrypt(Convert.FromBase64String(encryptedData)));

    public AESBuilderEngine SetKey(byte[] key)
    {
        this._key = key;
        return this;
    }

    public AESBuilderEngine SetKey(string key) => SetKey(Convert.FromBase64String(key));

    public AESBuilderEngine SetRandomKey() => SetKey(GenerateRandom(32));
    
    public AESBuilderEngine SetSeed(byte[] seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }
    
    public AESBuilderEngine SetSeed(string seed) => SetSeed(Encoding.ASCII.GetBytes(seed));

    public AESBuilderEngine SetRandomSeed() => SetSeed(GenerateRandom(128));
    
    private byte[] GenerateRandom(int size)
    {
        byte[] random = new byte[size];
        new SecureRandom().NextBytes(random);
        return random;
    }
}