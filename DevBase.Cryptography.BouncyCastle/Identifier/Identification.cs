using System.Text;
using DevBase.Extensions;
using DevBase.Generics;

namespace DevBase.Cryptography.BouncyCastle.Identifier;

/// <summary>
/// Provides methods for generating random identification strings.
/// </summary>
public class Identification
{
    /// <summary>
    /// Generates a random hexadecimal ID string.
    /// </summary>
    /// <param name="size">The number of bytes to generate for the ID. Defaults to 20.</param>
    /// <param name="seed">Optional seed for the random number generator.</param>
    /// <returns>A random hexadecimal string.</returns>
    public static string GenerateRandomId(int size = 20, byte[] seed = null)
    {
        byte[] s = seed == null ? new Random.Random().GenerateRandomBytes(16) : seed;

        Random.Random random = new Random.Random().SetSeed(s);

        AList<byte> randomArray = random.GenerateRandomBytes(size).ToAList();

        StringBuilder stringBuilder = new StringBuilder();
        randomArray.ForEach(t => stringBuilder.Append(t.ToString("x2")));

        return stringBuilder.ToString();
    }
}