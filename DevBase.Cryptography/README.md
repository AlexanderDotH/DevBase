# DevBase.Cryptography

**DevBase.Cryptography** provides cryptographic implementations for .NET 9.0, including Blowfish encryption and MD5 hashing. The library focuses on performance and ease of use.

## Features

- **Blowfish Encryption** - CBC mode implementation
- **MD5 Hashing** - Fast MD5 hash generation
- **Span<T> Support** - Memory-efficient operations
- **No External Dependencies** - Pure .NET implementation

## Installation

```bash
dotnet add package DevBase.Cryptography
```

## Blowfish Encryption

Blowfish is a symmetric block cipher that encrypts data in 8-byte blocks using CBC (Cipher Block Chaining) mode.

### Basic Usage

```csharp
using DevBase.Cryptography.Blowfish;

// Create cipher with key
byte[] key = Encoding.UTF8.GetBytes("my-secret-key");
var blowfish = new Blowfish(key);

// Prepare data (must be multiple of 8 bytes)
byte[] data = Encoding.UTF8.GetBytes("Hello World!"); // 12 bytes
byte[] paddedData = PadData(data); // Pad to 16 bytes

// Initialization vector (must be 8 bytes)
byte[] iv = new byte[8];
RandomNumberGenerator.Fill(iv);

// Encrypt
bool success = blowfish.Encrypt(paddedData, iv);

// Decrypt
bool decrypted = blowfish.Decrypt(paddedData, iv);
string result = Encoding.UTF8.GetString(paddedData).TrimEnd('\0');
```

### Encryption Example

```csharp
using DevBase.Cryptography.Blowfish;
using System.Security.Cryptography;

public byte[] EncryptData(string plaintext, string password)
{
    // Generate key from password
    byte[] key = Encoding.UTF8.GetBytes(password);
    
    // Pad data to multiple of 8
    byte[] data = Encoding.UTF8.GetBytes(plaintext);
    int paddedLength = (data.Length + 7) / 8 * 8;
    byte[] paddedData = new byte[paddedLength];
    Array.Copy(data, paddedData, data.Length);
    
    // Generate random IV
    byte[] iv = new byte[8];
    RandomNumberGenerator.Fill(iv);
    
    // Encrypt
    var blowfish = new Blowfish(key);
    blowfish.Encrypt(paddedData, iv);
    
    // Combine IV + encrypted data
    byte[] result = new byte[8 + paddedData.Length];
    Array.Copy(iv, 0, result, 0, 8);
    Array.Copy(paddedData, 0, result, 8, paddedData.Length);
    
    return result;
}
```

### Decryption Example

```csharp
public string DecryptData(byte[] encrypted, string password)
{
    // Extract IV and data
    byte[] iv = new byte[8];
    Array.Copy(encrypted, 0, iv, 0, 8);
    
    byte[] data = new byte[encrypted.Length - 8];
    Array.Copy(encrypted, 8, data, 0, data.Length);
    
    // Decrypt
    byte[] key = Encoding.UTF8.GetBytes(password);
    var blowfish = new Blowfish(key);
    blowfish.Decrypt(data, iv);
    
    // Remove padding and convert to string
    string result = Encoding.UTF8.GetString(data).TrimEnd('\0');
    return result;
}
```

### Using Codec Directly

```csharp
using DevBase.Cryptography.Blowfish;

// Create codec
byte[] key = Encoding.UTF8.GetBytes("secret");
var codec = new Codec(key);

// Encrypt single block (8 bytes)
Span<byte> block = stackalloc byte[8];
// ... fill block with data
codec.Encrypt(block);

// Decrypt
codec.Decrypt(block);
```

## MD5 Hashing

Fast MD5 hash generation for data integrity checks.

### Basic Usage

```csharp
using DevBase.Cryptography.MD5;

// Hash string
string input = "Hello World";
string hash = MD5.Hash(input);
Console.WriteLine(hash); // "b10a8db164e0754105b7a99be72e3fe5"

// Hash bytes
byte[] data = Encoding.UTF8.GetBytes("Hello World");
string hash = MD5.Hash(data);
```

### File Hashing

```csharp
using DevBase.Cryptography.MD5;
using DevBase.IO;

// Hash file
byte[] fileData = AFile.ReadFile("document.pdf").ToArray();
string fileHash = MD5.Hash(fileData);
Console.WriteLine($"File hash: {fileHash}");
```

### Verify Data Integrity

```csharp
public bool VerifyIntegrity(byte[] data, string expectedHash)
{
    string actualHash = MD5.Hash(data);
    return actualHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
}
```

## Advanced Usage

### Blowfish with Custom Padding

```csharp
public static byte[] PadPKCS7(byte[] data)
{
    int blockSize = 8;
    int paddingLength = blockSize - (data.Length % blockSize);
    byte[] padded = new byte[data.Length + paddingLength];
    
    Array.Copy(data, padded, data.Length);
    
    // Fill padding bytes with padding length value
    for (int i = data.Length; i < padded.Length; i++)
    {
        padded[i] = (byte)paddingLength;
    }
    
    return padded;
}

public static byte[] UnpadPKCS7(byte[] data)
{
    int paddingLength = data[data.Length - 1];
    byte[] unpadded = new byte[data.Length - paddingLength];
    Array.Copy(data, unpadded, unpadded.Length);
    return unpadded;
}
```

### Stream Encryption

```csharp
public void EncryptStream(Stream input, Stream output, byte[] key, byte[] iv)
{
    var blowfish = new Blowfish(key);
    byte[] buffer = new byte[8]; // Blowfish block size
    
    int bytesRead;
    while ((bytesRead = input.Read(buffer, 0, 8)) > 0)
    {
        if (bytesRead < 8)
        {
            // Pad last block
            Array.Clear(buffer, bytesRead, 8 - bytesRead);
        }
        
        blowfish.Encrypt(buffer, iv);
        output.Write(buffer, 0, 8);
        
        // Update IV for CBC mode
        Array.Copy(buffer, iv, 8);
    }
}
```

### Deezer Audio Decryption

Blowfish is used by Deezer for audio file encryption:

```csharp
using DevBase.Cryptography.Blowfish;

public byte[] DecryptDeezerAudio(byte[] encryptedData, string trackId)
{
    // Generate Deezer-specific key
    string secret = "g4el58wc0zvf9na1";
    string idMd5 = MD5.Hash(trackId);
    
    byte[] key = new byte[16];
    for (int i = 0; i < 16; i++)
    {
        key[i] = (byte)(idMd5[i] ^ idMd5[i + 16] ^ secret[i]);
    }
    
    // Decrypt in 2048-byte chunks
    var blowfish = new Blowfish(key);
    byte[] iv = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
    
    for (int i = 0; i < encryptedData.Length; i += 2048)
    {
        int chunkSize = Math.Min(2048, encryptedData.Length - i);
        if (chunkSize % 8 == 0)
        {
            Span<byte> chunk = encryptedData.AsSpan(i, chunkSize);
            blowfish.Decrypt(chunk, iv);
        }
    }
    
    return encryptedData;
}
```

## Performance Considerations

### Blowfish

- **Block Size**: 8 bytes - data must be padded to multiples of 8
- **Key Size**: Variable (1-56 bytes recommended)
- **CBC Mode**: Requires 8-byte initialization vector
- **Span<T>**: Uses `Span<byte>` for zero-allocation operations

### MD5

- **Fast**: Optimized for speed
- **Not Cryptographically Secure**: Use only for checksums, not passwords
- **Output**: 32-character hexadecimal string

## Security Notes

⚠️ **Important Security Considerations:**

1. **MD5 is not secure** for password hashing - use bcrypt, Argon2, or PBKDF2 instead
2. **Blowfish key management** - Store keys securely, never hardcode
3. **IV generation** - Always use cryptographically secure random IVs
4. **Padding** - Implement proper padding to avoid padding oracle attacks
5. **Modern alternatives** - Consider AES for new projects

## Common Use Cases

### Use Case 1: File Encryption

```csharp
public void EncryptFile(string inputPath, string outputPath, string password)
{
    byte[] data = File.ReadAllBytes(inputPath);
    byte[] encrypted = EncryptData(Encoding.UTF8.GetString(data), password);
    File.WriteAllBytes(outputPath, encrypted);
}
```

### Use Case 2: Data Integrity Verification

```csharp
public bool VerifyDownload(string filePath, string expectedMd5)
{
    byte[] fileData = File.ReadAllBytes(filePath);
    string actualMd5 = MD5.Hash(fileData);
    return actualMd5.Equals(expectedMd5, StringComparison.OrdinalIgnoreCase);
}
```

### Use Case 3: Secure Communication

```csharp
public byte[] EncryptMessage(string message, byte[] sharedKey)
{
    byte[] data = Encoding.UTF8.GetBytes(message);
    byte[] paddedData = PadPKCS7(data);
    
    byte[] iv = new byte[8];
    RandomNumberGenerator.Fill(iv);
    
    var blowfish = new Blowfish(sharedKey);
    blowfish.Encrypt(paddedData, iv);
    
    // Prepend IV
    byte[] result = new byte[8 + paddedData.Length];
    Array.Copy(iv, result, 8);
    Array.Copy(paddedData, 0, result, 8, paddedData.Length);
    
    return result;
}
```

## Credits

### Blowfish Implementation

Based on [jdvor/encryption-blowfish](https://github.com/jdvor/encryption-blowfish)

**License:** MIT License

The implementation has been integrated into DevBase for ecosystem consistency.

## Target Framework

- **.NET 9.0**

## Dependencies

- No external dependencies (pure .NET)

## License

MIT License - See LICENSE file for details

## Author

AlexanderDotH

## Repository

https://github.com/AlexanderDotH/DevBase
