# DevBase.Cryptography.BouncyCastle - AI Agent Guide

This guide helps AI agents effectively use DevBase.Cryptography.BouncyCastle for advanced cryptographic operations.

## Overview

DevBase.Cryptography.BouncyCastle provides enterprise-grade cryptography using BouncyCastle:
- **AES-GCM** - Authenticated encryption
- **ECDH** - Key exchange
- **Token Verification** - JWT and signature verification
- **Secure Random** - Cryptographically secure RNG

**Target Framework:** .NET 9.0

## Core Components

### AESBuilderEngine

```csharp
public class AESBuilderEngine
{
    public AESBuilderEngine();
    public byte[] Encrypt(byte[] buffer);
    public byte[] Decrypt(byte[] buffer);
    public void SetKey(byte[] key);
}
```

### EcdhEngineBuilder

```csharp
public class EcdhEngineBuilder
{
    public (AsymmetricKeyParameter Public, AsymmetricKeyParameter Private) GenerateKeyPair();
    public byte[] DeriveSharedSecret(AsymmetricKeyParameter privateKey, AsymmetricKeyParameter publicKey);
    public byte[] ExportPublicKey(AsymmetricKeyParameter publicKey);
    public AsymmetricKeyParameter ImportPublicKey(byte[] publicKeyBytes);
}
```

### Token Verifiers

```csharp
RsTokenVerifier  // RSA signatures (RS256, RS384, RS512)
EsTokenVerifier  // ECDSA signatures (ES256, ES384, ES512)
PsTokenVerifier  // RSA-PSS signatures (PS256, PS384, PS512)
ShaTokenVerifier // HMAC signatures (HS256, HS384, HS512)
```

## Usage Patterns for AI Agents

### Pattern 1: AES-GCM Encryption

```csharp
using DevBase.Cryptography.BouncyCastle.AES;

// Create engine (auto-generates key)
var aes = new AESBuilderEngine();

// Encrypt
byte[] plaintext = Encoding.UTF8.GetBytes("Secret message");
byte[] encrypted = aes.Encrypt(plaintext);

// Decrypt
byte[] decrypted = aes.Decrypt(encrypted);
string message = Encoding.UTF8.GetString(decrypted);
```

### Pattern 2: ECDH Key Exchange

```csharp
using DevBase.Cryptography.BouncyCastle.ECDH;

// Alice generates key pair
var alice = new EcdhEngineBuilder();
var (alicePublic, alicePrivate) = alice.GenerateKeyPair();

// Bob generates key pair
var bob = new EcdhEngineBuilder();
var (bobPublic, bobPrivate) = bob.GenerateKeyPair();

// Alice derives shared secret
byte[] aliceShared = alice.DeriveSharedSecret(alicePrivate, bobPublic);

// Bob derives shared secret
byte[] bobShared = bob.DeriveSharedSecret(bobPrivate, alicePublic);

// Both secrets are identical
// Use shared secret as encryption key
var aes = new AESBuilderEngine();
aes.SetKey(aliceShared);
```

### Pattern 3: JWT Verification

```csharp
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;

string token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...";
string publicKey = "-----BEGIN PUBLIC KEY-----\n...";

// Verify RSA signature
var rsVerifier = new RsTokenVerifier();
bool isValid = rsVerifier.Verify(token, publicKey);

// Verify ECDSA signature
var esVerifier = new EsTokenVerifier();
bool isValid = esVerifier.Verify(token, publicKey);

// Verify HMAC signature
var shaVerifier = new ShaTokenVerifier();
bool isValid = shaVerifier.Verify(token, secret);
```

### Pattern 4: Secure Random Generation

```csharp
using DevBase.Cryptography.BouncyCastle.Random;

var random = new Random();

// Generate random bytes
byte[] randomBytes = random.GenerateRandom(32);

// Generate random string
string randomString = random.GenerateRandomString(16);

// Generate random number
int randomNumber = random.GenerateRandomInt(1, 100);
```

### Pattern 5: Secure Communication

```csharp
public class SecureMessaging
{
    public byte[] EncryptMessage(string message, byte[] recipientPublicKey)
    {
        // Generate ephemeral key pair
        var ecdh = new EcdhEngineBuilder();
        var (ephemeralPublic, ephemeralPrivate) = ecdh.GenerateKeyPair();
        
        // Derive shared secret
        byte[] sharedSecret = ecdh.DeriveSharedSecret(ephemeralPrivate, recipientPublicKey);
        
        // Encrypt with AES-GCM
        var aes = new AESBuilderEngine();
        aes.SetKey(sharedSecret);
        byte[] encrypted = aes.Encrypt(Encoding.UTF8.GetBytes(message));
        
        // Package: [ephemeral public key length][ephemeral public key][encrypted data]
        byte[] publicKeyBytes = ecdh.ExportPublicKey(ephemeralPublic);
        
        using var ms = new MemoryStream();
        ms.Write(BitConverter.GetBytes(publicKeyBytes.Length), 0, 4);
        ms.Write(publicKeyBytes, 0, publicKeyBytes.Length);
        ms.Write(encrypted, 0, encrypted.Length);
        
        return ms.ToArray();
    }
    
    public string DecryptMessage(byte[] package, byte[] privateKey)
    {
        using var ms = new MemoryStream(package);
        
        // Extract ephemeral public key
        byte[] lengthBytes = new byte[4];
        ms.Read(lengthBytes, 0, 4);
        int keyLength = BitConverter.ToInt32(lengthBytes, 0);
        
        byte[] ephemeralPublicKey = new byte[keyLength];
        ms.Read(ephemeralPublicKey, 0, keyLength);
        
        // Extract encrypted data
        byte[] encrypted = new byte[ms.Length - ms.Position];
        ms.Read(encrypted, 0, encrypted.Length);
        
        // Derive shared secret
        var ecdh = new EcdhEngineBuilder();
        var publicKey = ecdh.ImportPublicKey(ephemeralPublicKey);
        byte[] sharedSecret = ecdh.DeriveSharedSecret(privateKey, publicKey);
        
        // Decrypt
        var aes = new AESBuilderEngine();
        aes.SetKey(sharedSecret);
        byte[] decrypted = aes.Decrypt(encrypted);
        
        return Encoding.UTF8.GetString(decrypted);
    }
}
```

## Important Concepts

### 1. AES-GCM Features

- **Authenticated Encryption** - Provides both confidentiality and authenticity
- **Nonce** - Automatically generated and prepended to ciphertext
- **Tag** - Authentication tag included in output
- **No Padding** - GCM mode doesn't require padding

```csharp
// AES-GCM automatically handles:
// - Nonce generation
// - Tag computation
// - Nonce prepending to ciphertext

var aes = new AESBuilderEngine();
byte[] encrypted = aes.Encrypt(data); // Nonce + encrypted + tag
byte[] decrypted = aes.Decrypt(encrypted); // Verifies tag, extracts nonce
```

### 2. ECDH Key Exchange

```csharp
// Generate key pairs
var alice = new EcdhEngineBuilder();
var (alicePublic, alicePrivate) = alice.GenerateKeyPair();

var bob = new EcdhEngineBuilder();
var (bobPublic, bobPrivate) = bob.GenerateKeyPair();

// Exchange public keys (can be sent over insecure channel)
byte[] alicePublicBytes = alice.ExportPublicKey(alicePublic);
byte[] bobPublicBytes = bob.ExportPublicKey(bobPublic);

// Derive shared secrets (identical on both sides)
byte[] aliceShared = alice.DeriveSharedSecret(alicePrivate, bobPublic);
byte[] bobShared = bob.DeriveSharedSecret(bobPrivate, alicePublic);

// Use shared secret as encryption key
var aes = new AESBuilderEngine();
aes.SetKey(aliceShared);
```

### 3. Token Verification Algorithms

| Verifier | Algorithms | Key Type |
|----------|-----------|----------|
| RsTokenVerifier | RS256, RS384, RS512 | RSA Public Key |
| EsTokenVerifier | ES256, ES384, ES512 | ECDSA Public Key |
| PsTokenVerifier | PS256, PS384, PS512 | RSA Public Key (PSS) |
| ShaTokenVerifier | HS256, HS384, HS512 | Shared Secret |

```csharp
// Select verifier based on algorithm
string algorithm = ExtractAlgorithmFromToken(token);

bool isValid = algorithm switch
{
    "RS256" or "RS384" or "RS512" => new RsTokenVerifier().Verify(token, publicKey),
    "ES256" or "ES384" or "ES512" => new EsTokenVerifier().Verify(token, publicKey),
    "PS256" or "PS384" or "PS512" => new PsTokenVerifier().Verify(token, publicKey),
    "HS256" or "HS384" or "HS512" => new ShaTokenVerifier().Verify(token, secret),
    _ => false
};
```

## Common Mistakes to Avoid

### ❌ Mistake 1: Reusing Nonces

```csharp
// Wrong - AES-GCM handles nonces automatically
// Don't try to manage nonces manually

// Correct - let AESBuilderEngine handle it
var aes = new AESBuilderEngine();
byte[] encrypted1 = aes.Encrypt(data1); // New nonce
byte[] encrypted2 = aes.Encrypt(data2); // New nonce
```

### ❌ Mistake 2: Wrong Verifier for Algorithm

```csharp
// Wrong - using RS verifier for ES token
var verifier = new RsTokenVerifier();
bool isValid = verifier.Verify(es256Token, publicKey); // Will fail!

// Correct - match verifier to algorithm
var verifier = new EsTokenVerifier();
bool isValid = verifier.Verify(es256Token, publicKey);
```

### ❌ Mistake 3: Not Storing Private Keys Securely

```csharp
// Wrong - hardcoding private key
byte[] privateKey = new byte[] { 1, 2, 3, ... };

// Correct - load from secure storage
byte[] privateKey = LoadFromSecureStorage("private-key");
```

### ❌ Mistake 4: Using Same Key for Everything

```csharp
// Wrong - using same key for multiple purposes
var aes = new AESBuilderEngine();
aes.SetKey(masterKey);
byte[] encrypted1 = aes.Encrypt(userData);
byte[] encrypted2 = aes.Encrypt(configData);

// Correct - derive different keys for different purposes
byte[] userDataKey = DeriveKey(masterKey, "user-data");
byte[] configDataKey = DeriveKey(masterKey, "config-data");
```

## Security Best Practices

1. **Key Management**
   - Never hardcode keys
   - Use secure key storage (Azure Key Vault, AWS KMS, etc.)
   - Rotate keys regularly

2. **Algorithm Selection**
   - Use AES-GCM for encryption (not CBC)
   - Use ECDH for key exchange (not RSA)
   - Use ES256 for signatures when possible

3. **Random Generation**
   - Always use `DevBase.Cryptography.BouncyCastle.Random`
   - Never use `System.Random` for cryptographic purposes

4. **Token Verification**
   - Always verify token signatures
   - Check token expiration
   - Validate token claims

## Performance Tips

1. **Reuse instances** - Create AESBuilderEngine once, use multiple times
2. **Key caching** - Cache derived ECDH shared secrets
3. **Async operations** - Use async for large data encryption
4. **Buffer pooling** - Reuse byte arrays when possible

## Integration Examples

### With DevBase.Api

```csharp
using DevBase.Api.Apis;
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;

public class SecureApiClient : ApiClient
{
    private readonly RsTokenVerifier _verifier;
    
    public SecureApiClient()
    {
        _verifier = new RsTokenVerifier();
    }
    
    public async Task<T> GetSecureAsync<T>(string url, string token, string publicKey)
    {
        // Verify token first
        if (!_verifier.Verify(token, publicKey))
            return Throw<T>(new SecurityException("Invalid token"));
        
        // Make request
        var response = await new Request(url)
            .WithHeader("Authorization", $"Bearer {token}")
            .SendAsync();
        
        return await response.ParseJsonAsync<T>();
    }
}
```

### With DevBase.Net

```csharp
using DevBase.Net.Core;
using DevBase.Cryptography.BouncyCastle.AES;

public class EncryptedHttpClient
{
    private readonly AESBuilderEngine _aes;
    
    public EncryptedHttpClient(byte[] encryptionKey)
    {
        _aes = new AESBuilderEngine();
        _aes.SetKey(encryptionKey);
    }
    
    public async Task<Response> PostEncryptedAsync(string url, byte[] data)
    {
        byte[] encrypted = _aes.Encrypt(data);
        
        return await new Request(url)
            .AsPost()
            .WithRawBody(encrypted, "application/octet-stream")
            .SendAsync();
    }
}
```

## Quick Reference

| Task | Code |
|------|------|
| AES encrypt | `new AESBuilderEngine().Encrypt(data)` |
| AES decrypt | `aes.Decrypt(encrypted)` |
| Generate key pair | `new EcdhEngineBuilder().GenerateKeyPair()` |
| Derive shared secret | `ecdh.DeriveSharedSecret(privateKey, publicKey)` |
| Verify RS token | `new RsTokenVerifier().Verify(token, publicKey)` |
| Verify ES token | `new EsTokenVerifier().Verify(token, publicKey)` |
| Verify HMAC token | `new ShaTokenVerifier().Verify(token, secret)` |
| Generate random | `new Random().GenerateRandom(32)` |

## Testing Considerations

- Test encryption/decryption round-trip
- Test key exchange with multiple parties
- Test token verification with valid/invalid tokens
- Test with different key sizes
- Verify random generation quality

## Version

Current version: **1.0.0**  
Target framework: **.NET 9.0**

## Dependencies

- BouncyCastle.Cryptography
