# DevBase.Cryptography Agent Guide

## Overview
DevBase.Cryptography provides basic cryptographic implementations including Blowfish and MD5.

## Key Classes

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `Blowfish` | `DevBase.Cryptography.Blowfish` | Blowfish encryption/decryption |
| `MD5` | `DevBase.Cryptography.Hash` | MD5 hashing |

## Quick Reference

### Blowfish Encryption
```csharp
using DevBase.Cryptography.Blowfish;

byte[] key = Encoding.UTF8.GetBytes("secret-key");
byte[] data = Encoding.UTF8.GetBytes("data to encrypt");

Blowfish blowfish = new Blowfish(key);
byte[] encrypted = blowfish.Encrypt(data);
byte[] decrypted = blowfish.Decrypt(encrypted);
```

### MD5 Hashing
```csharp
using DevBase.Cryptography.Hash;

string hash = MD5.ComputeHash("input string");
```

## File Structure
```
DevBase.Cryptography/
├── Blowfish/
│   └── Blowfish.cs
└── Hash/
    └── MD5.cs
```

## Important Notes

1. **MD5 is not cryptographically secure** - use only for checksums
2. **Blowfish requires key setup** before encryption
3. **For modern crypto, use DevBase.Cryptography.BouncyCastle**
