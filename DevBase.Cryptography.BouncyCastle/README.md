# DevBase.Cryptography.BouncyCastle

A comprehensive cryptography wrapper library built on BouncyCastle, providing advanced encryption, key exchange, and token verification capabilities.

## Features

- **AES Encryption** - Advanced Encryption Standard with builder pattern
- **ECDH Key Exchange** - Elliptic Curve Diffie-Hellman key agreement
- **Token Verification** - JWT and cryptographic token validation
  - HMAC (HS256, HS384, HS512)
  - RSA (RS256, RS384, RS512)
  - ECDSA (ES256, ES384, ES512)
  - RSA-PSS (PS256, PS384, PS512)
- **Secure Random** - Cryptographically secure random number generation
- **Data Sealing** - Secure data packaging

## Installation

```xml
<PackageReference Include="DevBase.Cryptography.BouncyCastle" Version="x.x.x" />
```

Or via NuGet CLI:

```bash
dotnet add package DevBase.Cryptography.BouncyCastle
```

## Usage Examples

### AES Encryption

```csharp
using DevBase.Cryptography.BouncyCastle.AES;

// Create AES engine with builder
AESBuilderEngine aes = new AESBuilderEngine()
    .WithKey(keyBytes)
    .WithIV(ivBytes)
    .Build();

// Encrypt
byte[] encrypted = aes.Encrypt(plaintext);

// Decrypt
byte[] decrypted = aes.Decrypt(encrypted);
```

### ECDH Key Exchange

```csharp
using DevBase.Cryptography.BouncyCastle.ECDH;

// Create key exchange builder
EcdhEngineBuilder ecdh = new EcdhEngineBuilder();

// Generate key pair
var keyPair = ecdh.GenerateKeyPair();

// Derive shared secret
byte[] sharedSecret = ecdh.DeriveSharedSecret(
    privateKey, 
    otherPartyPublicKey
);
```

### Token Verification (JWT)

```csharp
using DevBase.Cryptography.BouncyCastle.Hashing;

// Symmetric verification (HMAC)
SymmetricTokenVerifier verifier = new SymmetricTokenVerifier();
bool isValid = verifier.Verify(tokenData, signature, secretKey, "HS256");

// Asymmetric verification (RSA/ECDSA)
AsymmetricTokenVerifier asymVerifier = new AsymmetricTokenVerifier();
bool isValid = asymVerifier.Verify(tokenData, signature, publicKey, "RS256");
```

### Secure Random

```csharp
using DevBase.Cryptography.BouncyCastle.Random;

// Generate random bytes
byte[] randomBytes = Random.GenerateBytes(32);

// Generate random number
int randomNumber = Random.GenerateInt(1, 100);
```

### Data Sealing

```csharp
using DevBase.Cryptography.BouncyCastle.Sealing;

// Seal data
Sealing sealer = new Sealing();
byte[] sealed = sealer.Seal(data, key);

// Unseal data
byte[] unsealed = sealer.Unseal(sealed, key);
```

## Architecture

```
DevBase.Cryptography.BouncyCastle/
├── AES/
│   └── AESBuilderEngine.cs      # AES encryption builder
├── ECDH/
│   └── EcdhEngineBuilder.cs     # ECDH key exchange
├── Hashing/
│   ├── SymmetricTokenVerifier.cs   # HMAC verification
│   ├── AsymmetricTokenVerifier.cs  # RSA/ECDSA verification
│   └── Verification/
│       ├── ShaTokenVerifier.cs  # SHA-based HMAC
│       ├── RsTokenVerifier.cs   # RSA signature
│       ├── EsTokenVerifier.cs   # ECDSA signature
│       └── PsTokenVerifier.cs   # RSA-PSS signature
├── Random/
│   └── Random.cs                # Secure random generator
├── Sealing/
│   └── Sealing.cs               # Data sealing
├── Identifier/
│   └── Identification.cs        # Key identification
└── Extensions/
    └── AsymmetricKeyParameterExtension.cs
```

## Supported Algorithms

### Symmetric
- AES (128, 192, 256 bit)
- HMAC-SHA256, HMAC-SHA384, HMAC-SHA512

### Asymmetric
- RSA (RS256, RS384, RS512)
- RSA-PSS (PS256, PS384, PS512)
- ECDSA (ES256, ES384, ES512)

### Key Exchange
- ECDH (various curves)

## License

MIT License - see LICENSE file for details.
