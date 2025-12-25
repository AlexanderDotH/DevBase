# DevBase.Cryptography Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Cryptography project.

## Table of Contents

- [Blowfish](#blowfish)
  - [Blowfish](#blowfish-class)
  - [Codec](#codec)
  - [Extensions](#extensions)
  - [Init](#init)
- [MD5](#md5)
  - [MD5](#md5-class)

## Blowfish

### Blowfish (class)

```csharp
// This is the Blowfish CBC implementation from https://github.com/jdvor/encryption-blowfish
// This is NOT my code I just want to add it to my ecosystem to avoid too many libraries.

/// <summary>
/// Blowfish in CBC (cipher block chaining) block mode.
/// </summary>
public sealed class Blowfish
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Blowfish"/> class using a pre-configured codec.
    /// </summary>
    /// <param name="codec">The codec instance to use for encryption/decryption.</param>
    public Blowfish(Codec codec)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Blowfish"/> class with the specified key.
    /// </summary>
    /// <param name="key">The encryption key.</param>
    public Blowfish(byte[] key)
    
    /// <summary>
    /// Encrypt data.
    /// </summary>
    /// <param name="data">the length must be in multiples of 8</param>
    /// <param name="initVector">IV; the length must be exactly 8</param>
    /// <returns><code>true</code> if data has been encrypted; otherwise <code>false</code>.</returns>
    public bool Encrypt(Span<byte> data, ReadOnlySpan<byte> initVector)
    
    /// <summary>
    /// Decrypt data.
    /// </summary>
    /// <param name="data">the length must be in multiples of 8</param>
    /// <param name="initVector">IV; the length must be exactly 8</param>
    /// <returns><code>true</code> if data has been decrypted; otherwise <code>false</code>.</returns>
    public bool Decrypt(Span<byte> data, ReadOnlySpan<byte> initVector)
}
```

### Codec

```csharp
/// <summary>
/// Blowfish encryption and decryption on fixed size (length = 8) data block.
/// Codec is a relatively expensive object, because it must construct P-array and S-blocks from provided key.
/// It is expected to be used many times and it is thread-safe.
/// </summary>
public sealed class Codec
{
    /// <summary>
    /// Create codec instance and compute P-array and S-blocks.
    /// </summary>
    /// <param name="key">cipher key; valid size is &lt;8, 448&gt;</param>
    /// <exception cref="ArgumentException">on invalid input</exception>
    public Codec(byte[] key)
    
    /// <summary>
    /// Encrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="block">only first 8 bytes are encrypted</param>
    public void Encrypt(Span<byte> block)
    
    /// <summary>
    /// Encrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="offset">start encryption at this index of the data buffer</param>
    /// <param name="data">only first 8 bytes are encrypted from the offset</param>
    public void Encrypt(int offset, byte[] data)
    
    /// <summary>
    /// Decrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="block">only first 8 bytes are decrypted</param>
    public void Decrypt(Span<byte> block)
    
    /// <summary>
    /// Decrypt data block.
    /// There are no range checks within the method and it is expected that the caller will ensure big enough block.
    /// </summary>
    /// <param name="offset">start decryption at this index of the data buffer</param>
    /// <param name="data">only first 8 bytes are decrypted from the offset</param>
    public void Decrypt(int offset, byte[] data)
}
```

### Extensions

```csharp
public static class Extensions
{
    /// <summary>
    /// Return closest number divisible by 8 without remainder, which is equal or larger than original length.
    /// </summary>
    public static int PaddedLength(int originalLength)
    
    /// <summary>
    /// Return if the data block has length in multiples of 8.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptyOrNotPadded(Span<byte> data)
    
    /// <summary>
    /// Return same array if its length is multiple of 8; otherwise create new array with adjusted length
    /// and copy original array at the beginning.
    /// </summary>
    public static byte[] CopyAndPadIfNotAlreadyPadded(this byte[] data)
    
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
}
```

### Init

```csharp
internal static class Init
{
    /// <summary>
    /// The 18-entry P-array.
    /// </summary>
    internal static uint[] P()
    
    /// <summary>
    /// The 256-entry S0 box.
    /// </summary>
    internal static uint[] S0()
    
    /// <summary>
    /// The 256-entry S1 box.
    /// </summary>
    internal static uint[] S1()
    
    /// <summary>
    /// The 256-entry S2 box.
    /// </summary>
    internal static uint[] S2()
    
    /// <summary>
    /// The 256-entry S3 box.
    /// </summary>
    internal static uint[] S3()
}
```

## MD5

### MD5 (class)

```csharp
/// <summary>
/// Provides methods for calculating MD5 hashes.
/// </summary>
public class MD5
{
    /// <summary>
    /// Computes the MD5 hash of the given string and returns it as a byte array.
    /// </summary>
    /// <param name="data">The input string to hash.</param>
    /// <returns>The MD5 hash as a byte array.</returns>
    public static byte[] ToMD5Binary(string data)
    
    /// <summary>
    /// Computes the MD5 hash of the given string and returns it as a hexadecimal string.
    /// </summary>
    /// <param name="data">The input string to hash.</param>
    /// <returns>The MD5 hash as a hexadecimal string.</returns>
    public static string ToMD5String(string data)
    
    /// <summary>
    /// Computes the MD5 hash of the given byte array and returns it as a hexadecimal string.
    /// </summary>
    /// <param name="data">The input byte array to hash.</param>
    /// <returns>The MD5 hash as a hexadecimal string.</returns>
    public static string ToMD5(byte[] data)
}
```
