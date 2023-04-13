using System.Text;
using DevBase.Extensions;
using DevBase.Generics;

namespace DevBase.Cryptography.BouncyCastle.Identifier;

public class Identification
{
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