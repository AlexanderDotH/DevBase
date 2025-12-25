using DevBase.Cryptography.BouncyCastle.AES;

namespace DevBase.Test.DevBaseCryptographyBouncyCastle.AES;

/// <summary>
/// Tests for AESBuilderEngine.
/// </summary>
public class AESBuilderEngineTest
{
    private AESBuilderEngine _aesBuilderEngine;
    
    /// <summary>
    /// Sets up the test environment with a random key and seed.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._aesBuilderEngine = new AESBuilderEngine().SetRandomKey().SetRandomSeed();
    }

    /// <summary>
    /// Tests encryption and decryption of a string.
    /// </summary>
    [Test]
    public void EncryptAndDecrypt()
    {
        string buffer = "Dummy text!";

        string encrypted = this._aesBuilderEngine.EncryptString(buffer);
        string decrypted = this._aesBuilderEngine.DecryptString(encrypted);
        
        Assert.That(decrypted, Is.EqualTo(buffer));
    }
}
