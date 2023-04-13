using System.Runtime.CompilerServices;
using System.Text;
using Org.BouncyCastle.Security;

namespace DevBase.Cryptography.BouncyCastle.Random;

public class Random
{
    private SecureRandom _secureRandom;
    private byte[] _seed;

    public Random()
    {
        this._secureRandom = new SecureRandom();
    }
    
    public byte[] GenerateRandomBytes(int size)
    {
        byte[] randomBytes = new byte[size];
        this._secureRandom.NextBytes(randomBytes, 0, randomBytes.Length);
        return randomBytes;
    }

    public string RandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_")
    {
        StringBuilder stringBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
            stringBuilder.Append(charset[this._secureRandom.Next(0, charset.Length)]);

        return stringBuilder.ToString();
    }

    public string RandomBase64(int length) => Convert.ToBase64String(this.GenerateRandomBytes(length));

    public int RandomInt() => this._secureRandom.NextInt();
    
    public Random SetSeed(long seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }

    public Random SetSeed(byte[] seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }
}