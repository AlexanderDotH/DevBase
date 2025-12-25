# DevBase.Cryptography.BouncyCastle Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Cryptography.BouncyCastle project.

## Table of Contents

- [AES](#aes)
  - [AESBuilderEngine](#aesbuilderengine)
- [ECDH](#ecdh)
  - [EcdhEngineBuilder](#ecdhenginebuilder)
- [Exception](#exception)
  - [KeypairNotFoundException](#keypairnotfoundexception)
- [Extensions](#extensions)
  - [AsymmetricKeyParameterExtension](#asymmetrickeyparameterextension)
- [Hashing](#hashing)
  - [AsymmetricTokenVerifier](#asymmetrictokenverifier)
  - [SymmetricTokenVerifier](#symmetrictokenverifier)
  - [Verification](#verification)
    - [EsTokenVerifier](#estokenverifier)
    - [PsTokenVerifier](#pstokenverifier)
    - [RsTokenVerifier](#rstokenverifier)
    - [ShaTokenVerifier](#shatokenverifier)
- [Identifier](#identifier)
  - [Identification](#identification)
- [Random](#random)
  - [Random](#random-class)
- [Sealing](#sealing)
  - [Sealing](#sealing-class)

## AES

### AESBuilderEngine

```csharp
/// <summary>
/// Provides AES encryption and decryption functionality using GCM mode.
/// </summary>
public class AESBuilderEngine
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AESBuilderEngine"/> class with a random key.
    /// </summary>
    public AESBuilderEngine()
    
    /// <summary>
    /// Encrypts the specified buffer using AES-GCM.
    /// </summary>
    /// <param name="buffer">The data to encrypt.</param>
    /// <returns>A byte array containing the nonce followed by the encrypted data.</returns>
    public byte[] Encrypt(byte[] buffer)
    
    /// <summary>
    /// Decrypts the specified buffer using AES-GCM.
    /// </summary>
    /// <param name="buffer">The data to decrypt, expected to contain the nonce followed by the ciphertext.</param>
    /// <returns>The decrypted data.</returns>
    public byte[] Decrypt(byte[] buffer)
    
    /// <summary>
    /// Encrypts the specified string using AES-GCM and returns the result as a Base64 string.
    /// </summary>
    /// <param name="data">The string to encrypt.</param>
    /// <returns>The encrypted data as a Base64 string.</returns>
    public string EncryptString(string data)
    
    /// <summary>
    /// Decrypts the specified Base64 encoded string using AES-GCM.
    /// </summary>
    /// <param name="encryptedData">The Base64 encoded encrypted data.</param>
    /// <returns>The decrypted string.</returns>
    public string DecryptString(string encryptedData)
    
    /// <summary>
    /// Sets the encryption key.
    /// </summary>
    /// <param name="key">The key as a byte array.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetKey(byte[] key)
    
    /// <summary>
    /// Sets the encryption key from a Base64 encoded string.
    /// </summary>
    /// <param name="key">The Base64 encoded key.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetKey(string key)
    
    /// <summary>
    /// Sets a random encryption key.
    /// </summary>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetRandomKey()
    
    /// <summary>
    /// Sets the seed for the random number generator.
    /// </summary>
    /// <param name="seed">The seed as a byte array.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetSeed(byte[] seed)
    
    /// <summary>
    /// Sets the seed for the random number generator from a string.
    /// </summary>
    /// <param name="seed">The seed string.</param>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetSeed(string seed)
    
    /// <summary>
    /// Sets a random seed for the random number generator.
    /// </summary>
    /// <returns>The current instance of <see cref="AESBuilderEngine"/>.</returns>
    public AESBuilderEngine SetRandomSeed()
}
```

## ECDH

### EcdhEngineBuilder

```csharp
/// <summary>
/// Provides functionality for building and managing ECDH (Elliptic Curve Diffie-Hellman) key pairs and shared secrets.
/// </summary>
public class EcdhEngineBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EcdhEngineBuilder"/> class.
    /// </summary>
    public EcdhEngineBuilder()
    
    /// <summary>
    /// Generates a new ECDH key pair using the secp256r1 curve.
    /// </summary>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    public EcdhEngineBuilder GenerateKeyPair()
    
    /// <summary>
    /// Loads an existing ECDH key pair from byte arrays.
    /// </summary>
    /// <param name="publicKey">The public key bytes.</param>
    /// <param name="privateKey">The private key bytes.</param>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    public EcdhEngineBuilder FromExistingKeyPair(byte[] publicKey, byte[] privateKey)
    
    /// <summary>
    /// Loads an existing ECDH key pair from Base64 encoded strings.
    /// </summary>
    /// <param name="publicKey">The Base64 encoded public key.</param>
    /// <param name="privateKey">The Base64 encoded private key.</param>
    /// <returns>The current instance of <see cref="EcdhEngineBuilder"/>.</returns>
    public EcdhEngineBuilder FromExistingKeyPair(string publicKey, string privateKey)
    
    /// <summary>
    /// Derives a shared secret from the current private key and the provided public key.
    /// </summary>
    /// <param name="publicKey">The other party's public key.</param>
    /// <returns>The derived shared secret as a byte array.</returns>
    /// <exception cref="KeypairNotFoundException">Thrown if no key pair has been generated or loaded.</exception>
    public byte[] DeriveKeyPairs(AsymmetricKeyParameter publicKey)
    
    /// <summary>
    /// Gets the public key of the current key pair.
    /// </summary>
    public AsymmetricKeyParameter PublicKey { get; }
    
    /// <summary>
    /// Gets the private key of the current key pair.
    /// </summary>
    public AsymmetricKeyParameter PrivateKey { get; }
}
```

## Exception

### KeypairNotFoundException

```csharp
/// <summary>
/// Exception thrown when a key pair operation is attempted but no key pair is found.
/// </summary>
public class KeypairNotFoundException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeypairNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KeypairNotFoundException(string message)
}
```

## Extensions

### AsymmetricKeyParameterExtension

```csharp
/// <summary>
/// Provides extension methods for converting asymmetric key parameters to and from byte arrays.
/// </summary>
public static class AsymmetricKeyParameterExtension
{
    /// <summary>
    /// Converts an asymmetric public key parameter to its DER encoded byte array representation.
    /// </summary>
    /// <param name="keyParameter">The public key parameter.</param>
    /// <returns>The DER encoded byte array.</returns>
    /// <exception cref="ArgumentException">Thrown if the public key type is not supported.</exception>
    public static byte[] PublicKeyToArray(this AsymmetricKeyParameter keyParameter)
    
    /// <summary>
    /// Converts an asymmetric private key parameter to its unsigned byte array representation.
    /// </summary>
    /// <param name="keyParameter">The private key parameter.</param>
    /// <returns>The unsigned byte array representation of the private key.</returns>
    /// <exception cref="ArgumentException">Thrown if the private key type is not supported.</exception>
    public static byte[] PrivateKeyToArray(this AsymmetricKeyParameter keyParameter)
    
    /// <summary>
    /// Converts a byte array to an ECDH public key parameter using the secp256r1 curve.
    /// </summary>
    /// <param name="keySequence">The byte array representing the public key.</param>
    /// <returns>The ECDH public key parameter.</returns>
    /// <exception cref="ArgumentException">Thrown if the byte array is invalid.</exception>
    public static AsymmetricKeyParameter ToEcdhPublicKey(this byte[] keySequence)
    
    /// <summary>
    /// Converts a byte array to an ECDH private key parameter using the secp256r1 curve.
    /// </summary>
    /// <param name="keySequence">The byte array representing the private key.</param>
    /// <returns>The ECDH private key parameter.</returns>
    /// <exception cref="ArgumentException">Thrown if the byte array is invalid.</exception>
    public static AsymmetricKeyParameter ToEcdhPrivateKey(this byte[] keySequence)
}
```

## Hashing

### AsymmetricTokenVerifier

```csharp
/// <summary>
/// Abstract base class for verifying asymmetric signatures of tokens.
/// </summary>
public abstract class AsymmetricTokenVerifier
{
    /// <summary>
    /// Gets or sets the encoding used for the token parts. Defaults to UTF-8.
    /// </summary>
    public Encoding Encoding { get; set; }
    
    /// <summary>
    /// Verifies the signature of a token.
    /// </summary>
    /// <param name="header">The token header.</param>
    /// <param name="payload">The token payload.</param>
    /// <param name="signature">The token signature (Base64Url encoded).</param>
    /// <param name="publicKey">The public key to use for verification.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    public bool VerifySignature(string header, string payload, string signature, string publicKey)
    
    /// <summary>
    /// Verifies the signature of the content bytes using the provided public key.
    /// </summary>
    /// <param name="content">The content bytes (header + "." + payload).</param>
    /// <param name="signature">The signature bytes.</param>
    /// <param name="publicKey">The public key.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    protected abstract bool VerifySignature(byte[] content, byte[] signature, string publicKey);
}
```

### SymmetricTokenVerifier

```csharp
/// <summary>
/// Abstract base class for verifying symmetric signatures of tokens.
/// </summary>
public abstract class SymmetricTokenVerifier
{
    /// <summary>
    /// Gets or sets the encoding used for the token parts. Defaults to UTF-8.
    /// </summary>
    public Encoding Encoding { get; set; }
    
    /// <summary>
    /// Verifies the signature of a token.
    /// </summary>
    /// <param name="header">The token header.</param>
    /// <param name="payload">The token payload.</param>
    /// <param name="signature">The token signature (Base64Url encoded).</param>
    /// <param name="secret">The shared secret used for verification.</param>
    /// <param name="isSecretEncoded">Indicates whether the secret string is Base64Url encoded.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    public bool VerifySignature(string header, string payload, string signature, string secret, bool isSecretEncoded = false)
    
    /// <summary>
    /// Verifies the signature of the content bytes using the provided secret.
    /// </summary>
    /// <param name="content">The content bytes (header + "." + payload).</param>
    /// <param name="signature">The signature bytes.</param>
    /// <param name="secret">The secret bytes.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    protected abstract bool VerifySignature(byte[] content, byte[] signature, byte[] secret);
}
```

### Verification

#### EsTokenVerifier

```csharp
/// <summary>
/// Verifies ECDSA signatures for tokens.
/// </summary>
/// <typeparam name="T">The digest algorithm to use (e.g., SHA256).</typeparam>
public class EsTokenVerifier<T> : AsymmetricTokenVerifier where T : IDigest
{
    /// <inheritdoc />
    protected override bool VerifySignature(byte[] content, byte[] signature, string publicKey)
    
    /// <summary>
    /// Converts a P1363 signature format to ASN.1 DER format.
    /// </summary>
    /// <param name="p1363Signature">The P1363 signature bytes.</param>
    /// <returns>The ASN.1 DER encoded signature.</returns>
    private byte[] ToAsn1Der(byte[] p1363Signature)
}
```

#### PsTokenVerifier

```csharp
/// <summary>
/// Verifies RSASSA-PSS signatures for tokens.
/// </summary>
/// <typeparam name="T">The digest algorithm to use (e.g., SHA256).</typeparam>
public class PsTokenVerifier<T> : AsymmetricTokenVerifier where T : IDigest
{
    /// <inheritdoc />
    protected override bool VerifySignature(byte[] content, byte[] signature, string publicKey)
}
```

#### RsTokenVerifier

```csharp
/// <summary>
/// Verifies RSASSA-PKCS1-v1_5 signatures for tokens.
/// </summary>
/// <typeparam name="T">The digest algorithm to use (e.g., SHA256).</typeparam>
public class RsTokenVerifier<T> : AsymmetricTokenVerifier where T : IDigest
{
    /// <inheritdoc />
    protected override bool VerifySignature(byte[] content, byte[] signature, string publicKey)
}
```

#### ShaTokenVerifier

```csharp
/// <summary>
/// Verifies HMAC-SHA signatures for tokens.
/// </summary>
/// <typeparam name="T">The digest algorithm to use (e.g., SHA256).</typeparam>
public class ShaTokenVerifier<T> : SymmetricTokenVerifier where T : IDigest
{
    /// <inheritdoc />
    protected override bool VerifySignature(byte[] content, byte[] signature, byte[] secret)
}
```

## Identifier

### Identification

```csharp
/// <summary>
/// Provides methods for generating random identification strings.
/// </summary>
public class Identification
{
    /// <summary>
    /// Generates a random hexadecimal ID string.
    /// </summary>
    /// <param name="size">The number of bytes to generate for the ID. Defaults to 20.</param>
    /// <param name="seed">Optional seed for the random number generator.</param>
    /// <returns>A random hexadecimal string.</returns>
    public static string GenerateRandomId(int size = 20, byte[] seed = null)
}
```

## Random

### Random (class)

```csharp
/// <summary>
/// Provides secure random number generation functionality.
/// </summary>
public class Random
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Random"/> class.
    /// </summary>
    public Random()
    
    /// <summary>
    /// Generates a specified number of random bytes.
    /// </summary>
    /// <param name="size">The number of bytes to generate.</param>
    /// <returns>An array containing the random bytes.</returns>
    public byte[] GenerateRandomBytes(int size)
    
    /// <summary>
    /// Generates a random string of the specified length using a given character set.
    /// </summary>
    /// <param name="length">The length of the string to generate.</param>
    /// <param name="charset">The character set to use. Defaults to alphanumeric characters and some symbols.</param>
    /// <returns>The generated random string.</returns>
    public string RandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_")
    
    /// <summary>
    /// Generates a random Base64 string of a specified byte length.
    /// </summary>
    /// <param name="length">The number of random bytes to generate before encoding.</param>
    /// <returns>A Base64 encoded string of random bytes.</returns>
    public string RandomBase64(int length)
    
    /// <summary>
    /// Generates a random integer.
    /// </summary>
    /// <returns>A random integer.</returns>
    public int RandomInt()
    
    /// <summary>
    /// Sets the seed for the random number generator using a long value.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    /// <returns>The current instance of <see cref="Random"/>.</returns>
    public Random SetSeed(long seed)
    
    /// <summary>
    /// Sets the seed for the random number generator using a byte array.
    /// </summary>
    /// <param name="seed">The seed bytes.</param>
    /// <returns>The current instance of <see cref="Random"/>.</returns>
    public Random SetSeed(byte[] seed)
}
```

## Sealing

### Sealing (class)

```csharp
/// <summary>
/// Provides functionality for sealing and unsealing messages using hybrid encryption (ECDH + AES).
/// </summary>
public class Sealing
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for sealing messages to a recipient.
    /// </summary>
    /// <param name="othersPublicKey">The recipient's public key.</param>
    public Sealing(byte[] othersPublicKey)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for sealing messages to a recipient using Base64 encoded public key.
    /// </summary>
    /// <param name="othersPublicKey">The recipient's Base64 encoded public key.</param>
    public Sealing(string othersPublicKey)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for unsealing messages.
    /// </summary>
    /// <param name="publicKey">The own public key.</param>
    /// <param name="privateKey">The own private key.</param>
    public Sealing(byte[] publicKey, byte[] privateKey)
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Sealing"/> class for unsealing messages using Base64 encoded keys.
    /// </summary>
    /// <param name="publicKey">The own Base64 encoded public key.</param>
    /// <param name="privateKey">The own Base64 encoded private key.</param>
    public Sealing(string publicKey, string privateKey)
    
    /// <summary>
    /// Seals (encrypts) a message.
    /// </summary>
    /// <param name="unsealedMessage">The message to seal.</param>
    /// <returns>A byte array containing the sender's public key length, public key, and the encrypted message.</returns>
    public byte[] Seal(byte[] unsealedMessage)
    
    /// <summary>
    /// Seals (encrypts) a string message.
    /// </summary>
    /// <param name="unsealedMessage">The string message to seal.</param>
    /// <returns>A Base64 string containing the sealed message.</returns>
    public string Seal(string unsealedMessage)
    
    /// <summary>
    /// Unseals (decrypts) a message.
    /// </summary>
    /// <param name="sealedMessage">The sealed message bytes.</param>
    /// <returns>The unsealed (decrypted) message bytes.</returns>
    public byte[] UnSeal(byte[] sealedMessage)
    
    /// <summary>
    /// Unseals (decrypts) a Base64 encoded message string.
    /// </summary>
    /// <param name="sealedMessage">The Base64 encoded sealed message.</param>
    /// <returns>The unsealed (decrypted) string message.</returns>
    public string UnSeal(string sealedMessage)
}
```
