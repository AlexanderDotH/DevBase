# DevBase.Cryptography.BouncyCastle Agent Guide

## Overview
DevBase.Cryptography.BouncyCastle provides modern cryptographic implementations using the BouncyCastle library.

## Key Classes

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `AesGcm` | `DevBase.Cryptography.BouncyCastle.AesGcm` | AES-GCM authenticated encryption |
| `Blowfish` | `DevBase.Cryptography.BouncyCastle.Blowfish` | Enhanced Blowfish |

## Quick Reference

### AES-GCM Encryption
```csharp
using DevBase.Cryptography.BouncyCastle.AesGcm;

byte[] key = new byte[32]; // 256-bit key
byte[] nonce = new byte[12]; // 96-bit nonce
byte[] plaintext = Encoding.UTF8.GetBytes("secret data");

AesGcm aes = new AesGcm(key);
byte[] ciphertext = aes.Encrypt(nonce, plaintext);
byte[] decrypted = aes.Decrypt(nonce, ciphertext);
```

## File Structure
```
DevBase.Cryptography.BouncyCastle/
├── AesGcm/
│   └── AesGcm.cs
└── Blowfish/
    └── Blowfish.cs
```

## Important Notes

1. **Requires BouncyCastle.NetCore** NuGet package
2. **Use unique nonces** for each encryption
3. **AES-GCM provides authenticated encryption** (integrity + confidentiality)
4. **Prefer this over DevBase.Cryptography** for security
