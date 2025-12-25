using System.Runtime.CompilerServices;
using System.Text;
using Org.BouncyCastle.Security;

namespace DevBase.Cryptography.BouncyCastle.Random;

/// <summary>
/// Provides secure random number generation functionality.
/// </summary>
public class Random
{
    private SecureRandom _secureRandom;
    private byte[] _seed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Random"/> class.
    /// </summary>
    public Random()
    {
        this._secureRandom = new SecureRandom();
    }
    
    /// <summary>
    /// Generates a specified number of random bytes.
    /// </summary>
    /// <param name="size">The number of bytes to generate.</param>
    /// <returns>An array containing the random bytes.</returns>
    public byte[] GenerateRandomBytes(int size)
    {
        byte[] randomBytes = new byte[size];
        this._secureRandom.NextBytes(randomBytes, 0, randomBytes.Length);
        return randomBytes;
    }

    /// <summary>
    /// Generates a random string of the specified length using a given character set.
    /// </summary>
    /// <param name="length">The length of the string to generate.</param>
    /// <param name="charset">The character set to use. Defaults to alphanumeric characters and some symbols.</param>
    /// <returns>The generated random string.</returns>
    public string RandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_")
    {
        StringBuilder stringBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
            stringBuilder.Append(charset[this._secureRandom.Next(0, charset.Length)]);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Generates a random Base64 string of a specified byte length.
    /// </summary>
    /// <param name="length">The number of random bytes to generate before encoding.</param>
    /// <returns>A Base64 encoded string of random bytes.</returns>
    public string RandomBase64(int length) => Convert.ToBase64String(this.GenerateRandomBytes(length));

    /// <summary>
    /// Generates a random integer.
    /// </summary>
    /// <returns>A random integer.</returns>
    public int RandomInt() => this._secureRandom.NextInt();
    
    /// <summary>
    /// Sets the seed for the random number generator using a long value.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    /// <returns>The current instance of <see cref="Random"/>.</returns>
    public Random SetSeed(long seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }

    /// <summary>
    /// Sets the seed for the random number generator using a byte array.
    /// </summary>
    /// <param name="seed">The seed bytes.</param>
    /// <returns>The current instance of <see cref="Random"/>.</returns>
    public Random SetSeed(byte[] seed)
    {
        this._secureRandom.SetSeed(seed);
        return this;
    }
}