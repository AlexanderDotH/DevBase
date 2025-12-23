# DevBase.Cryptography.BouncyCastle

**DevBase.Cryptography.BouncyCastle** provides advanced cryptographic operations using the BouncyCastle library, including AES-GCM encryption, ECDH key exchange, token verification, and secure random generation.

## Features

- **AES-GCM Encryption** - Authenticated encryption with GCM mode
- **ECDH Key Exchange** - Elliptic Curve Diffie-Hellman
- **Token Verification** - Multiple signature algorithms (RSA, ECDSA, HMAC)
- **Secure Random** - Cryptographically secure random generation
- **Key Management** - Asymmetric key pair generation and storage
- **Data Sealing** - Encrypt and seal data with authentication

## Installation

```bash
dotnet add package DevBase.Cryptography.BouncyCastle
```

## AES-GCM Encryption

AES-GCM provides authenticated encryption with associated data (AEAD).

### Basic Usage

```csharp
using DevBase.Cryptography.BouncyCastle.AES;

// Create engine (generates random key)
var aes = new AESBuilderEngine();

// Encrypt
byte[] plaintext = Encoding.UTF8.GetBytes("Secret message");
byte[] encrypted = aes.Encrypt(plaintext);

// Decrypt
byte[] decrypted = aes.Decrypt(encrypted);
string message = Encoding.UTF8.GetString(decrypted);
```

### With Custom Key

```csharp
var aes = new AESBuilderEngine();

// Set custom key (32 bytes for AES-256)
byte[] customKey = new byte[32];
RandomNumberGenerator.Fill(customKey);
aes.SetKey(customKey);

byte[] encrypted = aes.Encrypt(data);
```

## ECDH Key Exchange

Elliptic Curve Diffie-Hellman for secure key exchange.

### Generate Key Pair

```csharp
using DevBase.Cryptography.BouncyCastle.ECDH;

var ecdh = new EcdhEngineBuilder();

// Generate key pair
var (publicKey, privateKey) = ecdh.GenerateKeyPair();

// Export public key for sharing
byte[] publicKeyBytes = ecdh.ExportPublicKey(publicKey);
```

### Derive Shared Secret

```csharp
// Alice generates key pair
var alice = new EcdhEngineBuilder();
var (alicePublic, alicePrivate) = alice.GenerateKeyPair();

// Bob generates key pair
var bob = new EcdhEngineBuilder();
var (bobPublic, bobPrivate) = bob.GenerateKeyPair();

// Alice derives shared secret using Bob's public key
byte[] aliceShared = alice.DeriveSharedSecret(alicePrivate, bobPublic);

// Bob derives shared secret using Alice's public key
byte[] bobShared = bob.DeriveSharedSecret(bobPrivate, alicePublic);

// Both shared secrets are identical
Assert.Equal(aliceShared, bobShared);
```

## Token Verification

Verify JWT and other signed tokens.

### RSA Token Verification

```csharp
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;

var verifier = new RsTokenVerifier();

string token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...";
string publicKey = "-----BEGIN PUBLIC KEY-----\n...";

bool isValid = verifier.Verify(token, publicKey);
```

### ECDSA Token Verification

```csharp
var verifier = new EsTokenVerifier();
bool isValid = verifier.Verify(token, publicKey);
```

### HMAC Token Verification

```csharp
var verifier = new ShaTokenVerifier();
string secret = "your-secret-key";
bool isValid = verifier.Verify(token, secret);
```

### PSS Token Verification

```csharp
var verifier = new PsTokenVerifier();
bool isValid = verifier.Verify(token, publicKey);
```

## Secure Random Generation

Cryptographically secure random number generation.

### Generate Random Bytes

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

## Data Sealing

Encrypt and seal data with authentication.

### Seal Data

```csharp
using DevBase.Cryptography.BouncyCastle.Sealing;

var sealing = new Sealing();

byte[] data = Encoding.UTF8.GetBytes("Sensitive data");
byte[] key = new byte[32];
RandomNumberGenerator.Fill(key);

// Seal data (encrypt + authenticate)
byte[] sealed = sealing.Seal(data, key);

// Unseal data (verify + decrypt)
byte[] unsealed = sealing.Unseal(sealed, key);
```

## Usage Examples

### Secure Communication

```csharp
using DevBase.Cryptography.BouncyCastle.ECDH;
using DevBase.Cryptography.BouncyCastle.AES;

public class SecureChannel
{
    public async Task<byte[]> SendSecureMessage(string message, byte[] recipientPublicKey)
    {
        // Generate ephemeral key pair
        var ecdh = new EcdhEngineBuilder();
        var (ephemeralPublic, ephemeralPrivate) = ecdh.GenerateKeyPair();
        
        // Derive shared secret
        byte[] sharedSecret = ecdh.DeriveSharedSecret(ephemeralPrivate, recipientPublicKey);
        
        // Encrypt message with shared secret
        var aes = new AESBuilderEngine();
        aes.SetKey(sharedSecret);
        byte[] encrypted = aes.Encrypt(Encoding.UTF8.GetBytes(message));
        
        // Include ephemeral public key with encrypted message
        byte[] publicKeyBytes = ecdh.ExportPublicKey(ephemeralPublic);
        
        using var ms = new MemoryStream();
        ms.Write(BitConverter.GetBytes(publicKeyBytes.Length), 0, 4);
        ms.Write(publicKeyBytes, 0, publicKeyBytes.Length);
        ms.Write(encrypted, 0, encrypted.Length);
        
        return ms.ToArray();
    }
    
    public string ReceiveSecureMessage(byte[] encryptedPackage, byte[] privateKey)
    {
        using var ms = new MemoryStream(encryptedPackage);
        
        // Read ephemeral public key
        byte[] lengthBytes = new byte[4];
        ms.Read(lengthBytes, 0, 4);
        int publicKeyLength = BitConverter.ToInt32(lengthBytes, 0);
        
        byte[] ephemeralPublicKey = new byte[publicKeyLength];
        ms.Read(ephemeralPublicKey, 0, publicKeyLength);
        
        // Read encrypted message
        byte[] encrypted = new byte[ms.Length - ms.Position];
        ms.Read(encrypted, 0, encrypted.Length);
        
        // Derive shared secret
        var ecdh = new EcdhEngineBuilder();
        var publicKey = ecdh.ImportPublicKey(ephemeralPublicKey);
        byte[] sharedSecret = ecdh.DeriveSharedSecret(privateKey, publicKey);
        
        // Decrypt message
        var aes = new AESBuilderEngine();
        aes.SetKey(sharedSecret);
        byte[] decrypted = aes.Decrypt(encrypted);
        
        return Encoding.UTF8.GetString(decrypted);
    }
}
```

### JWT Verification

```csharp
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;

public class JwtValidator
{
    public bool ValidateToken(string token, string publicKey, string algorithm)
    {
        return algorithm switch
        {
            "RS256" or "RS384" or "RS512" => new RsTokenVerifier().Verify(token, publicKey),
            "ES256" or "ES384" or "ES512" => new EsTokenVerifier().Verify(token, publicKey),
            "PS256" or "PS384" or "PS512" => new PsTokenVerifier().Verify(token, publicKey),
            "HS256" or "HS384" or "HS512" => new ShaTokenVerifier().Verify(token, publicKey),
            _ => false
        };
    }
}
```

### Secure File Storage

```csharp
using DevBase.Cryptography.BouncyCastle.AES;
using DevBase.Cryptography.BouncyCastle.Random;

public class SecureFileStorage
{
    private readonly AESBuilderEngine _aes;
    private readonly byte[] _masterKey;
    
    public SecureFileStorage(byte[] masterKey)
    {
        _masterKey = masterKey;
        _aes = new AESBuilderEngine();
        _aes.SetKey(masterKey);
    }
    
    public void SaveSecure(string filePath, byte[] data)
    {
        byte[] encrypted = _aes.Encrypt(data);
        File.WriteAllBytes(filePath, encrypted);
    }
    
    public byte[] LoadSecure(string filePath)
    {
        byte[] encrypted = File.ReadAllBytes(filePath);
        return _aes.Decrypt(encrypted);
    }
}
```

### API Authentication

```csharp
using DevBase.Cryptography.BouncyCastle.Hashing.Verification;

public class ApiAuthenticator
{
    private readonly Dictionary<string, string> _publicKeys;
    
    public bool AuthenticateRequest(string authHeader)
    {
        // Extract token from header
        string token = authHeader.Replace("Bearer ", "");
        
        // Extract key ID from token
        string keyId = ExtractKeyId(token);
        
        if (!_publicKeys.ContainsKey(keyId))
            return false;
        
        // Verify token
        var verifier = new RsTokenVerifier();
        return verifier.Verify(token, _publicKeys[keyId]);
    }
}
```

## Key Management

### Generate and Store Keys

```csharp
using DevBase.Cryptography.BouncyCastle.ECDH;
using DevBase.Cryptography.BouncyCastle.Extensions;

public class KeyManager
{
    public void GenerateAndSaveKeyPair(string publicKeyPath, string privateKeyPath)
    {
        var ecdh = new EcdhEngineBuilder();
        var (publicKey, privateKey) = ecdh.GenerateKeyPair();
        
        // Export keys
        byte[] publicKeyBytes = publicKey.ExportPublicKey();
        byte[] privateKeyBytes = privateKey.ExportPrivateKey();
        
        // Save to files
        File.WriteAllBytes(publicKeyPath, publicKeyBytes);
        File.WriteAllBytes(privateKeyPath, privateKeyBytes);
    }
    
    public (byte[] PublicKey, byte[] PrivateKey) LoadKeyPair(string publicKeyPath, string privateKeyPath)
    {
        byte[] publicKey = File.ReadAllBytes(publicKeyPath);
        byte[] privateKey = File.ReadAllBytes(privateKeyPath);
        
        return (publicKey, privateKey);
    }
}
```

## Security Best Practices

1. **Key Storage** - Never hardcode keys, use secure key storage
2. **Key Rotation** - Regularly rotate encryption keys
3. **Random Generation** - Always use cryptographically secure random
4. **Token Expiration** - Implement token expiration checks
5. **Algorithm Selection** - Use modern algorithms (AES-GCM, ECDH, ES256)

## Algorithm Support

### Encryption
- **AES-GCM** - 128, 192, 256-bit keys

### Key Exchange
- **ECDH** - P-256, P-384, P-521 curves

### Signatures
- **RSA** - RS256, RS384, RS512
- **ECDSA** - ES256, ES384, ES512
- **RSA-PSS** - PS256, PS384, PS512
- **HMAC** - HS256, HS384, HS512

## Performance Considerations

- **AES-GCM** - Fast and secure for bulk encryption
- **ECDH** - Faster than RSA for key exchange
- **Key Caching** - Cache derived keys when possible
- **Async Operations** - Consider async for large data

## Target Framework

- **.NET 9.0**

## Dependencies

- **BouncyCastle.Cryptography** - Cryptographic primitives

## License

MIT License - See LICENSE file for details

## Author

AlexanderDotH

## Repository

https://github.com/AlexanderDotH/DevBase
