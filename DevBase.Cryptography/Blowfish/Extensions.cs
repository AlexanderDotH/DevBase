namespace DevBase.Cryptography.Blowfish;

using System.Runtime.CompilerServices;
using System.Text;

public static class Extensions
{
    /// <summary>
    /// Return closest number divisible by 8 without remainder, which is equal or larger than original length.
    /// </summary>
    public static int PaddedLength(int originalLength)
    {
        var mod = originalLength % 8;
        return mod == 0
            ? originalLength
            : originalLength + 8 - mod;
    }

    /// <summary>
    /// Return if the data block has length in multiples of 8.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptyOrNotPadded(Span<byte> data)
        => data.Length == 0 || data.Length % 8 != 0;

    /// <summary>
    /// Return same array if its length is multiple of 8; otherwise create new array with adjusted length
    /// and copy original array at the beginning.
    /// </summary>
    public static byte[] CopyAndPadIfNotAlreadyPadded(this byte[] data)
    {
        var paddedLength = PaddedLength(data.Length);
        if (paddedLength == data.Length)
        {
            return data;
        }

        var padded = new byte[paddedLength];
        data.CopyTo(padded, 0);

        return padded;
    }

    /// <summary>
    /// Format data block as hex string with optional formatting. Each byte is represented as two characters [0-9A-F].
    /// </summary>
    /// <param name="data">the data block</param>
    /// <param name="pretty">
    /// if <code>true</code> it will enable additional formatting; otherwise the bytes are placed on one line
    /// without separator. The default is <code>true</code>.
    /// </param>
    /// <param name="bytesPerLine">how many bytes to put on a line</param>
    /// <param name="byteSep">separate bytes with this string</param>
    /// <returns></returns>
    public static string ToHexString(
        this Span<byte> data, bool pretty = true, int bytesPerLine = 8, string byteSep = "")
    {
        if (!pretty)
        {
            return Convert.ToHexString(data);
        }

        var capacity = (2 + byteSep.Length) * data.Length + 2 * (data.Length / bytesPerLine + 1);
        var sb = new StringBuilder(capacity);
        var hasByteSep = byteSep.Length > 0;
        while (data.Length > 0)
        {
            var n = data.Length >= bytesPerLine ? bytesPerLine : data.Length;
            var block = data[..n];
            for (var i = 0; i < block.Length; i++)
            {
                sb.Append(block[i].ToString("X2"));
                if (i > 0 && hasByteSep)
                {
                    sb.Append(byteSep);
                }
            }

            sb.AppendLine();
            data = data[n..];
        }

        return sb.ToString();
    }
}