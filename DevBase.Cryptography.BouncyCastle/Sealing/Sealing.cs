using System.Text;
using DevBase.Cryptography.BouncyCastle.AES;
using DevBase.Cryptography.BouncyCastle.ECDH;
using DevBase.Cryptography.BouncyCastle.Extensions;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Tls;

namespace DevBase.Cryptography.BouncyCastle.Sealing;

public class Sealing
{
    private byte[] _othersPublicKey;
    private byte[] _sharedSecret;
    private EcdhEngineBuilder _ecdhEngine;
    private AESBuilderEngine _aesEngine;

    public Sealing(byte[] othersPublicKey)
    {
        this._othersPublicKey = othersPublicKey;
        this._ecdhEngine = new EcdhEngineBuilder().GenerateKeyPair();
        
        this._sharedSecret = this._ecdhEngine.DeriveKeyPairs(this._othersPublicKey.ToEcdhPublicKey());
        this._aesEngine = new AESBuilderEngine().SetKey(this._sharedSecret);
    }

    public Sealing(string othersPublicKey) : this(Convert.FromBase64String(othersPublicKey)) { }

    public Sealing(byte[] publicKey, byte[] privateKey)
    {
        this._ecdhEngine = new EcdhEngineBuilder().FromExistingKeyPair(publicKey, privateKey);
    }

    public Sealing(string publicKey, string privateKey) : this(Convert.FromBase64String(publicKey),
        Convert.FromBase64String(privateKey)) {}
    
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

    public string Seal(string unsealedMessage) => Convert.ToBase64String(Seal(Encoding.ASCII.GetBytes(unsealedMessage)));
    
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

    public string UnSeal(string sealedMessage) =>
        Encoding.ASCII.GetString(UnSeal(Convert.FromBase64String(sealedMessage)));
}