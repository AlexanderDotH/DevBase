# DevBase.Cryptography - AI Agent Guide

This guide helps AI agents effectively use the DevBase.Cryptography library for encryption and hashing operations.

## Overview

DevBase.Cryptography provides:
- **Blowfish** - Symmetric block cipher (CBC mode)
- **MD5** - Fast hashing for checksums

**Target Framework:** .NET 9.0

## Core Components

### Blowfish Class

```csharp
public sealed class Blowfish
{
    public Blowfish(byte[] key);
    public bool Encrypt(Span<byte> data, ReadOnlySpan<byte> initVector);
    public bool Decrypt(Span<byte> data, ReadOnlySpan<byte> initVector);
}
```

### Codec Class

```csharp
public sealed class Codec
{
    public Codec(byte[] key);
    public void Encrypt(Span<byte> block);
    public void Decrypt(Span<byte> block);
}
```

### MD5 Class

```csharp
public static class MD5
{
    public static string Hash(string input);
    public static string Hash(byte[] data);
}
```

## Usage Patterns for AI Agents

### Pattern 1: Basic Blowfish Encryption

```csharp
using DevBase.Cryptography.Blowfish;
using System.Security.Cryptography;

// Prepare key
byte[] key = Encoding.UTF8.GetBytes("my-secret-key");

// Prepare data (MUST be multiple of 8 bytes)
string plaintext = "Hello World!";
byte[] data = Encoding.UTF8.GetBytes(plaintext);

// Pad to multiple of 8
int paddedLength = (data.Length + 7) / 8 * 8;
byte[] paddedData = new byte[paddedLength];
Array.Copy(data, paddedData, data.Length);

// Generate IV (MUST be 8 bytes)
byte[] iv = new byte[8];
RandomNumberGenerator.Fill(iv);

// Encrypt
var blowfish = new Blowfish(key);
bool success = blowfish.Encrypt(paddedData, iv);

if (success)
{
    Console.WriteLine("Encrypted successfully");
}
```

### Pattern 2: Basic Blowfish Decryption

```csharp
// Decrypt (using same key and IV)
var blowfish = new Blowfish(key);
bool success = blowfish.Decrypt(paddedData, iv);

if (success)
{
    // Remove padding
    string decrypted = Encoding.UTF8.GetString(paddedData).TrimEnd('\0');
    Console.WriteLine($"Decrypted: {decrypted}");
}
```

### Pattern 3: Complete Encryption/Decryption Flow

```csharp
public byte[] EncryptData(string plaintext, string password)
{
    // Generate key
    byte[] key = Encoding.UTF8.GetBytes(password);
    
    // Pad data
    byte[] data = Encoding.UTF8.GetBytes(plaintext);
    int paddedLength = (data.Length + 7) / 8 * 8;
    byte[] paddedData = new byte[paddedLength];
    Array.Copy(data, paddedData, data.Length);
    
    // Generate IV
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

public string DecryptData(byte[] encrypted, string password)
{
    // Extract IV
    byte[] iv = new byte[8];
    Array.Copy(encrypted, 0, iv, 0, 8);
    
    // Extract data
    byte[] data = new byte[encrypted.Length - 8];
    Array.Copy(encrypted, 8, data, 0, data.Length);
    
    // Decrypt
    byte[] key = Encoding.UTF8.GetBytes(password);
    var blowfish = new Blowfish(key);
    blowfish.Decrypt(data, iv);
    
    // Remove padding
    return Encoding.UTF8.GetString(data).TrimEnd('\0');
}
```

### Pattern 4: MD5 Hashing

```csharp
using DevBase.Cryptography.MD5;

// Hash string
string input = "Hello World";
string hash = MD5.Hash(input);
Console.WriteLine(hash); // "b10a8db164e0754105b7a99be72e3fe5"

// Hash bytes
byte[] data = Encoding.UTF8.GetBytes("Hello World");
string hash = MD5.Hash(data);

// Verify integrity
bool isValid = MD5.Hash(data) == expectedHash;
```

### Pattern 5: Deezer Audio Decryption

```csharp
using DevBase.Cryptography.Blowfish;
using DevBase.Cryptography.MD5;

public byte[] DecryptDeezerAudio(byte[] encryptedData, string trackId)
{
    // Generate Deezer key
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
        
        // Only decrypt if chunk is multiple of 8
        if (chunkSize % 8 == 0)
        {
            Span<byte> chunk = encryptedData.AsSpan(i, chunkSize);
            blowfish.Decrypt(chunk, iv);
        }
    }
    
    return encryptedData;
}
```

### Pattern 6: File Encryption

```csharp
public void EncryptFile(string inputPath, string outputPath, string password)
{
    // Read file
    byte[] fileData = File.ReadAllBytes(inputPath);
    
    // Encrypt
    byte[] encrypted = EncryptData(
        Encoding.UTF8.GetString(fileData), 
        password
    );
    
    // Write encrypted file
    File.WriteAllBytes(outputPath, encrypted);
}

public void DecryptFile(string inputPath, string outputPath, string password)
{
    // Read encrypted file
    byte[] encrypted = File.ReadAllBytes(inputPath);
    
    // Decrypt
    string decrypted = DecryptData(encrypted, password);
    
    // Write decrypted file
    File.WriteAllText(outputPath, decrypted);
}
```

## Important Concepts

### 1. Blowfish Requirements

**Critical Rules:**
- Data MUST be multiple of 8 bytes
- IV MUST be exactly 8 bytes
- Key can be any length (1-56 bytes recommended)

```csharp
// ✅ Correct - 16 bytes (multiple of 8)
byte[] data = new byte[16];

// ❌ Wrong - 15 bytes (not multiple of 8)
byte[] data = new byte[15];
blowfish.Encrypt(data, iv); // Returns false!

// ✅ Correct - pad to 16 bytes
byte[] data = new byte[15];
byte[] padded = new byte[16];
Array.Copy(data, padded, data.Length);
blowfish.Encrypt(padded, iv);
```

### 2. Initialization Vector (IV)

```csharp
// ✅ Correct - random IV
byte[] iv = new byte[8];
RandomNumberGenerator.Fill(iv);

// ❌ Wrong - zero IV (predictable)
byte[] iv = new byte[8]; // All zeros

// ✅ Correct - store IV with encrypted data
byte[] result = new byte[8 + encryptedData.Length];
Array.Copy(iv, result, 8);
Array.Copy(encryptedData, 0, result, 8, encryptedData.Length);
```

### 3. Padding Strategies

```csharp
// Zero padding (simplest)
int paddedLength = (data.Length + 7) / 8 * 8;
byte[] padded = new byte[paddedLength];
Array.Copy(data, padded, data.Length);
// Remaining bytes are zero

// PKCS7 padding (standard)
int paddingLength = 8 - (data.Length % 8);
byte[] padded = new byte[data.Length + paddingLength];
Array.Copy(data, padded, data.Length);
for (int i = data.Length; i < padded.Length; i++)
{
    padded[i] = (byte)paddingLength;
}
```

### 4. MD5 Usage

```csharp
// ✅ Good - checksums, integrity verification
string fileHash = MD5.Hash(fileData);
bool isValid = fileHash == expectedHash;

// ❌ Bad - password hashing (use bcrypt/Argon2 instead)
string passwordHash = MD5.Hash(password); // NOT SECURE!
```

## Common Mistakes to Avoid

### ❌ Mistake 1: Data Not Multiple of 8

```csharp
// Wrong
byte[] data = Encoding.UTF8.GetBytes("Hello"); // 5 bytes
blowfish.Encrypt(data, iv); // Returns false!

// Correct
byte[] data = Encoding.UTF8.GetBytes("Hello");
byte[] padded = new byte[8];
Array.Copy(data, padded, data.Length);
blowfish.Encrypt(padded, iv);
```

### ❌ Mistake 2: Wrong IV Size

```csharp
// Wrong
byte[] iv = new byte[16]; // Too large!
blowfish.Encrypt(data, iv); // Returns false!

// Correct
byte[] iv = new byte[8];
RandomNumberGenerator.Fill(iv);
```

### ❌ Mistake 3: Not Storing IV

```csharp
// Wrong - IV lost, can't decrypt!
byte[] encrypted = EncryptData(plaintext, password);
// IV not included in result

// Correct - IV prepended to encrypted data
byte[] result = new byte[8 + encrypted.Length];
Array.Copy(iv, result, 8);
Array.Copy(encrypted, 0, result, 8, encrypted.Length);
```

### ❌ Mistake 4: Reusing Same IV

```csharp
// Wrong - same IV for multiple encryptions
byte[] iv = new byte[8];
RandomNumberGenerator.Fill(iv);
blowfish.Encrypt(data1, iv);
blowfish.Encrypt(data2, iv); // Security risk!

// Correct - new IV for each encryption
byte[] iv1 = new byte[8];
RandomNumberGenerator.Fill(iv1);
blowfish.Encrypt(data1, iv1);

byte[] iv2 = new byte[8];
RandomNumberGenerator.Fill(iv2);
blowfish.Encrypt(data2, iv2);
```

### ❌ Mistake 5: Using MD5 for Passwords

```csharp
// Wrong - MD5 is not secure for passwords
string passwordHash = MD5.Hash(password);

// Correct - use proper password hashing
// (not in this library, use BCrypt.Net or similar)
```

## Security Best Practices

1. **Never hardcode keys** - Load from secure storage
2. **Use random IVs** - Generate new IV for each encryption
3. **Store IV with data** - Prepend IV to encrypted data
4. **Secure key derivation** - Use PBKDF2 for password-based keys
5. **MD5 for checksums only** - Not for passwords or security

## Performance Tips

1. **Reuse Blowfish instances** for multiple operations with same key
2. **Use Span<byte>** for zero-allocation operations
3. **Batch operations** when encrypting multiple blocks
4. **Pre-allocate buffers** for padding operations

## Integration with DevBase.Api

Used by Deezer API for audio decryption:

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Cryptography.Blowfish;

var deezer = new Deezer("arl-token");
byte[] encryptedAudio = await deezer.DownloadTrack("123456");

// Decrypt using Blowfish
byte[] decrypted = DecryptDeezerAudio(encryptedAudio, "123456");
```

## Quick Reference

| Task | Code |
|------|------|
| Create Blowfish | `new Blowfish(keyBytes)` |
| Encrypt | `blowfish.Encrypt(data, iv)` |
| Decrypt | `blowfish.Decrypt(data, iv)` |
| Hash string | `MD5.Hash(text)` |
| Hash bytes | `MD5.Hash(bytes)` |
| Generate IV | `RandomNumberGenerator.Fill(iv)` |
| Pad data | `(length + 7) / 8 * 8` |

## Testing Considerations

- Test with various data sizes
- Test with edge cases (empty, single byte, exact multiples of 8)
- Test IV generation randomness
- Test encryption/decryption round-trip
- Test with different key sizes
- Verify MD5 hash consistency

## Credits

Blowfish implementation based on [jdvor/encryption-blowfish](https://github.com/jdvor/encryption-blowfish) (MIT License)

## Version

Current version: **1.0.0**  
Target framework: **.NET 9.0**

## Dependencies

None (pure .NET)
