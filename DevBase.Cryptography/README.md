# DevBase.Cryptography

A lightweight cryptography library for .NET providing common encryption algorithms and hashing utilities.

## Features

- **Blowfish Encryption** - Symmetric block cipher implementation
- **MD5 Hashing** - MD5 hash generation utilities
- **Simple API** - Easy-to-use cryptographic operations

## Installation

```xml
<PackageReference Include="DevBase.Cryptography" Version="x.x.x" />
```

Or via NuGet CLI:

```bash
dotnet add package DevBase.Cryptography
```

## Usage Examples

### Blowfish Encryption

```csharp
using DevBase.Cryptography.Blowfish;

// Create Blowfish instance with key
BlowfishEngine blowfish = new BlowfishEngine("your-secret-key");

// Encrypt data
byte[] plaintext = Encoding.UTF8.GetBytes("Hello World");
byte[] encrypted = blowfish.Encrypt(plaintext);

// Decrypt data
byte[] decrypted = blowfish.Decrypt(encrypted);
string result = Encoding.UTF8.GetString(decrypted);
```

### MD5 Hashing

```csharp
using DevBase.Cryptography.MD5;

// Generate MD5 hash
string input = "data to hash";
string hash = MD5Utils.ComputeHash(input);

// Verify hash
bool isValid = MD5Utils.VerifyHash(input, hash);
```

## API Reference

### Blowfish

| Class | Description |
|-------|-------------|
| `BlowfishEngine` | Blowfish encryption/decryption engine |

### MD5

| Class | Description |
|-------|-------------|
| `MD5Utils` | MD5 hashing utilities |

## Security Note

This library provides basic cryptographic primitives. For production security-critical applications, consider using well-audited libraries and consult security experts.

## License

MIT License - see LICENSE file for details.
