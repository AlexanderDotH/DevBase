# DevBase.Cryptography

DevBase offers two cryptography libraries:
1. **DevBase.Cryptography**: Contains legacy or specific implementations (Blowfish, MD5).
2. **DevBase.Cryptography.BouncyCastle**: A modern wrapper around the BouncyCastle library, providing high-level abstractions for AES-GCM, Token Verification, and more.

## Table of Contents
- [DevBase.Cryptography](#devbasecryptography)
  - [Blowfish](#blowfish)
  - [MD5](#md5)
- [DevBase.Cryptography.BouncyCastle](#devbasecryptographybouncycastle)
  - [AES (GCM)](#aes-gcm)
  - [Token Verification](#token-verification)

---

## DevBase.Cryptography

### Blowfish
An implementation of the Blowfish algorithm in CBC (Cipher Block Chaining) mode.

**Features:**
- Encrypts and decrypts byte spans.
- Requires an 8-byte initialization vector (IV).

**Example:**
```csharp
using DevBase.Cryptography.Blowfish;

byte[] key = ...; // Your key
var blowfish = new Blowfish(key);

byte[] iv = ...; // 8 bytes IV
Span<byte> data = ...; // Data to encrypt (multiple of 8 bytes)

// Encrypt
blowfish.Encrypt(data, iv);

// Decrypt
blowfish.Decrypt(data, iv);
```

### MD5
Simple helper class for MD5 hashing.

**Example:**
```csharp
using DevBase.Cryptography.MD5;

// To Hex String
string hash = MD5.ToMD5String("Hello World");

// To Binary
byte[] hashBytes = MD5.ToMD5Binary("Hello World");
```

---

## DevBase.Cryptography.BouncyCastle

### AES (GCM)
`AESBuilderEngine` provides a secure, easy-to-use wrapper for AES encryption in GCM (Galois/Counter Mode). It handles nonce generation and management automatically.

**Key Features:**
- **Automatic Nonce Handling**: Generates a 12-byte nonce for every encryption and prepends it to the output.
- **Secure Random**: Uses BouncyCastle's `SecureRandom` for key and nonce generation.
- **Base64 Helpers**: Methods to encrypt/decrypt strings directly to/from Base64.

**Example:**
```csharp
using DevBase.Cryptography.BouncyCastle.AES;

// Initialize
var aes = new AESBuilderEngine();
aes.SetRandomKey(); // Or .SetKey(yourKey)

// Encrypt String
string secret = "My Secret Data";
string encrypted = aes.EncryptString(secret);

// Decrypt String
string decrypted = aes.DecryptString(encrypted);
```

### Token Verification
Abstract base classes and implementations for verifying cryptographic signatures/tokens, similar to JWT verification logic.

**Classes:**
- `SymmetricTokenVerifier`: Base class for verifying signatures where the secret is shared.

**Example (Conceptual):**
```csharp
// Assuming an implementation exists or you extend SymmetricTokenVerifier
verifier.VerifySignature(
    header: "...",
    payload: "...",
    signature: "...",
    secret: "my-secret-key"
);
```
