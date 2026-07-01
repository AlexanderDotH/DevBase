using System.Text;
using DevBase.Cryptography.Blowfish;
using DevBase.Cryptography.BouncyCastle.AES;
using DevBase.Cryptography.BouncyCastle.ECDH;
using DevBase.Cryptography.MD5;

namespace DevBase.IntegrationTests.Cryptography;

[Category("Integration")]
[Category("Library")]
public sealed class CryptographyIntegrationTests
{
    [Test]
    public void Md5AndBlowfishCanHashEncryptAndDecryptTheSamePayload()
    {
        byte[] key = Encoding.ASCII.GetBytes("1234567890ABCDEF");
        byte[] iv = [0, 1, 2, 3, 4, 5, 6, 7];
        byte[] original = Encoding.UTF8.GetBytes("DevBase integration payload").CopyAndPadIfNotAlreadyPadded();
        byte[] encrypted = original.ToArray();
        Blowfish blowfish = new(key);

        bool encryptedResult = blowfish.Encrypt(encrypted, iv);
        string encryptedHash = MD5.ToMD5(encrypted);
        bool decryptedResult = blowfish.Decrypt(encrypted, iv);

        Assert.Multiple(() =>
        {
            Assert.That(encryptedResult, Is.True);
            Assert.That(encryptedHash, Is.Not.EqualTo(MD5.ToMD5(original)));
            Assert.That(decryptedResult, Is.True);
            Assert.That(encrypted, Is.EqualTo(original));
            Assert.That(MD5.ToMD5String("hello world"), Is.EqualTo("5eb63bbbe01eeed093cb22bb8f5acdc3"));
        });
    }

    [Test]
    public void BouncyCastleAesAndEcdhRoundTripsDataAndSharedSecrets()
    {
        byte[] aesKey = Enumerable.Range(1, 32).Select(value => (byte)value).ToArray();
        AESBuilderEngine aes = new AESBuilderEngine()
            .SetKey(aesKey)
            .SetSeed("devbase-integration-tests");
        string encrypted = aes.EncryptString("shared-secret-payload");

        EcdhEngineBuilder alice = new EcdhEngineBuilder().GenerateKeyPair();
        EcdhEngineBuilder bob = new EcdhEngineBuilder().GenerateKeyPair();
        byte[] aliceSecret = alice.DeriveKeyPairs(bob.PublicKey);
        byte[] bobSecret = bob.DeriveKeyPairs(alice.PublicKey);

        Assert.Multiple(() =>
        {
            Assert.That(encrypted, Is.Not.EqualTo("shared-secret-payload"));
            Assert.That(aes.DecryptString(encrypted), Is.EqualTo("shared-secret-payload"));
            Assert.That(aliceSecret, Is.Not.Empty);
            Assert.That(bobSecret, Is.EqualTo(aliceSecret));
        });
    }
}
