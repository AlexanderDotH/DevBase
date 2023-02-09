namespace DevBase.Cryptography.Blowfish;

using System;

/// <summary>
/// Blowfish encryption and decryption on fixed size (length = 8) data block.
/// Codec is a relatively expensive object, because it must construct P-array and S-blocks from provided key.
/// It is expected to be used many times and it is thread-safe.
/// </summary>
public sealed class Codec
{
    private readonly uint[] p;
    private readonly uint[] s0;
    private readonly uint[] s1;
    private readonly uint[] s2;
    private readonly uint[] s3;

    /// <summary>
    /// Create codec instance and compute P-array and S-blocks.
    /// </summary>
    /// <param name="key">cipher key; valid size is &lt;8, 448&gt;</param>
    /// <exception cref="ArgumentException">on invalid input</exception>
    public Codec(byte[] key)
    {
        if (key is null || key.Length is < 8 or > 448)
        {
            throw new ArgumentException("invalid key length; not in <8, 448>", nameof(key));
        }

        p = Init.P();
        s0 = Init.S0();
        s1 = Init.S1();
        s2 = Init.S2();
        s3 = Init.S3();

        var j = 0;
        for (var i = 0; i < 18; i++)
        {
            var d1 = key[j % key.Length];
            var d2 = key[(j + 1) % key.Length];
            var d3 = key[(j + 2) % key.Length];
            var d4 = key[(j + 3) % key.Length];
            var d = (uint)(((d1 * 256 + d2) * 256 + d3) * 256 + d4);
            p[i] ^= d;
            j = (j + 4) % key.Length;
        }

        uint xl = 0;
        uint xr = 0;
        for (var i = 0; i < 18; i += 2)
        {
            Encipher(ref xl, ref xr);
            p[i] = xl;
            p[i + 1] = xr;
        }

        for (var i = 0; i < 256; i += 2)
        {
            Encipher(ref xl, ref xr);
            s0[i] = xl;
            s0[i + 1] = xr;
        }

        for (var i = 0; i < 256; i += 2)
        {
            Encipher(ref xl, ref xr);
            s1[i] = xl;
            s1[i + 1] = xr;
        }

        for (var i = 0; i < 256; i += 2)
        {
            Encipher(ref xl, ref xr);
            s2[i] = xl;
            s2[i + 1] = xr;
        }

        for (var i = 0; i < 256; i += 2)
        {
            Encipher(ref xl, ref xr);
            s3[i] = xl;
            s3[i + 1] = xr;
        }
    }

    private void Encipher(ref uint xl, ref uint xr)
    {
        xl ^= p[0];
        for (var i = 0; i < 16; i += 2)
        {
            xr = Round(xr, xl, i + 1);
            xl = Round(xl, xr, i + 2);
        }

        xr ^= p[17];
        (xl, xr) = (xr, xl);
    }

    private void Decipher(ref uint xl, ref uint xr)
    {
        xl ^= p[17];
        for (var i = 16; i > 0; i -= 2)
        {
            xr = Round(xr, xl, i);
            xl = Round(xl, xr, i - 1);
        }

        xr ^= p[0];
        (xl, xr) = (xr, xl);
    }

    private uint Round(uint a, uint b, int n)
    {
        var x = s0[b >> 24];
        x += s1[b >> 16 & 0xFF];
        x ^= s2[b >> 8 & 0xFF];
        x += s3[b & 0xFF];
        x ^= p[n];
        return x ^ a;
    }

    /// <summary>
    /// Encrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="block">only first 8 bytes are encrypted</param>
    public void Encrypt(Span<byte> block)
    {
        var xl = (uint)((block[0] << 24) | (block[1] << 16) | (block[2] << 8) | block[3]);
        var xr = (uint)((block[4] << 24) | (block[5] << 16) | (block[6] << 8) | block[7]);
        Encipher(ref xl, ref xr);
        block[0] = (byte)(xl >> 24);
        block[1] = (byte)(xl >> 16);
        block[2] = (byte)(xl >> 8);
        block[3] = (byte)xl;
        block[4] = (byte)(xr >> 24);
        block[5] = (byte)(xr >> 16);
        block[6] = (byte)(xr >> 8);
        block[7] = (byte)xr;
    }

    /// <summary>
    /// Encrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="offset">start encryption at this index of the data buffer</param>
    /// <param name="data">only first 8 bytes are encrypted from the offset</param>
    public void Encrypt(int offset, byte[] data)
    {
        var xl = (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);
        var xr = (uint)((data[offset + 4] << 24) | (data[offset + 5] << 16) | (data[offset + 6] << 8) | data[offset + 7]);
        Encipher(ref xl, ref xr);
        data[offset] = (byte)(xl >> 24);
        data[offset + 1] = (byte)(xl >> 16);
        data[offset + 2] = (byte)(xl >> 8);
        data[offset + 3] = (byte)xl;
        data[offset + 4] = (byte)(xr >> 24);
        data[offset + 5] = (byte)(xr >> 16);
        data[offset + 6] = (byte)(xr >> 8);
        data[offset + 7] = (byte)xr;
    }

    /// <summary>
    /// Decrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="block">only first 8 bytes are decrypted</param>
    public void Decrypt(Span<byte> block)
    {
        var xl = (uint)((block[0] << 24) | (block[1] << 16) | (block[2] << 8) | block[3]);
        var xr = (uint)((block[4] << 24) | (block[5] << 16) | (block[6] << 8) | block[7]);
        Decipher(ref xl, ref xr);
        block[0] = (byte)(xl >> 24);
        block[1] = (byte)(xl >> 16);
        block[2] = (byte)(xl >> 8);
        block[3] = (byte)xl;
        block[4] = (byte)(xr >> 24);
        block[5] = (byte)(xr >> 16);
        block[6] = (byte)(xr >> 8);
        block[7] = (byte)xr;
    }

    /// <summary>
    /// Decrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="offset">start decryption at this index of the data buffer</param>
    /// <param name="data">only first 8 bytes are decrypted from the offset</param>
    public void Decrypt(int offset, byte[] data)
    {
        var xl = (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);
        var xr = (uint)((data[offset + 4] << 24) | (data[offset + 5] << 16) | (data[offset + 6] << 8) | data[offset + 7]);
        Decipher(ref xl, ref xr);
        data[offset] = (byte)(xl >> 24);
        data[offset + 1] = (byte)(xl >> 16);
        data[offset + 2] = (byte)(xl >> 8);
        data[offset + 3] = (byte)xl;
        data[offset + 4] = (byte)(xr >> 24);
        data[offset + 5] = (byte)(xr >> 16);
        data[offset + 6] = (byte)(xr >> 8);
        data[offset + 7] = (byte)xr;
    }
}