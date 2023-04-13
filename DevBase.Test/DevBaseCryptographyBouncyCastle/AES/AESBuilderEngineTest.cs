using DevBase.Cryptography.BouncyCastle.AES;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.AES;

public class AESBuilderEngineTest
{
    private AESBuilderEngine _aesBuilderEngine;
    
    [SetUp]
    public void Setup()
    {
        this._aesBuilderEngine = new AESBuilderEngine().SetRandomKey().SetRandomSeed();
    }

    [Test]
    public void EncryptAndDecrypt()
    {
        string buffer = "Dummy text!";

        string encrypted = this._aesBuilderEngine.EncryptString(buffer);
        string decrypted = this._aesBuilderEngine.DecryptString(encrypted);
        
        Assert.AreEqual(buffer, decrypted);
    }
}