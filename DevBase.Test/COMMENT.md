# DevBase.Test Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Test project.

## Table of Contents

- [Test Framework](#test-framework)
  - [PenetrationTest](#penetrationtest)
- [DevBase Tests](#devbase-tests)
  - [AListTests](#alisttests)
  - [MultitaskingTest](#multitaskingtest)
  - [StringUtilsTest](#stringutilstest)
  - [Base64EncodedAStringTest](#base64encodedastringtest)
  - [AFileTest](#afiletest)
- [DevBase.Api Tests](#devbaseapi-tests)
  - [AppleMusicTests](#applemusictests)
  - [BeautifulLyricsTests](#beautifullyricstests)
  - [DeezerTests](#deezertests)
  - [MusixMatchTest](#musixmatchtest)
  - [NetEaseTest](#neteasetest)
  - [RefreshTokenTest](#refreshtokentest)
  - [TidalTests](#tidaltests)
- [DevBase.Cryptography.BouncyCastle Tests](#devbasecryptographybouncycastle-tests)
  - [AES Tests](#aes-tests)
  - [Hashing Tests](#hashing-tests)
- [DevBase.Format Tests](#devbaseformat-tests)
  - [FormatTest](#formattest)
  - [AppleXmlTester](#applexmltester)
  - [ElrcTester](#elrctester)
  - [KLyricsTester](#klyricstester)
  - [LrcTester](#lrctester)
  - [RlrcTester](#rlrctester)
  - [RmmlTester](#rmmltester)
  - [SrtTester](#srttester)
- [DevBase.Net Tests](#devbasenet-tests)
  - [BatchRequestsTest](#batchrequeststest)
  - [BrowserSpoofingTest](#browserspoofingtest)
  - [FileUploadTest](#fileuploadtest)
  - [HttpToSocks5ProxyTest](#httotosocks5proxytest)
  - [ParameterBuilderTest](#parameterbuildertest)
  - [UserAgentBuilderTest](#useragentbuildertest)
  - [RequestTest](#requesttest)
  - [RequestBuilderTest](#requestbuildertest)
  - [RequestArchitectureTest](#requestarchitecturetest)
  - [RateLimitRetryTest](#ratelimitretrytest)
  - [ResponseMultiSelectorTest](#responsemultiselectortest)
  - [RetryPolicyTest]((retrypolicytest)
  - [AuthenticationTokenTest](#authenticationtokentest)
  - [BogusUtilsTests](#bogusutilstests)
  - [ContentDispositionUtilsTests](#contentdispositionutilstests)
  - [DockerIntegrationTests](#dockerintegrationtests)
- [DevBaseColor Tests](#devbasecolor-tests)
  - [ColorCalculator](#colorcalculator)

## Test Framework

### PenetrationTest

```csharp
/// <summary>
/// Helper class for performance testing (penetration testing).
/// </summary>
public class PenetrationTest
{
    protected PenetrationTest()
    
    /// <summary>
    /// Runs an action multiple times and measures the total execution time.
    /// </summary>
    /// <param name="runAction">The action to execute.</param>
    /// <param name="count">The number of times to execute the action.</param>
    /// <returns>A Stopwatch instance with the elapsed time.</returns>
    public static Stopwatch Run(Action runAction, int count = 1_000_000)
    
    /// <summary>
    /// Runs a function multiple times and returns the output of the last execution.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="runAction">The function to execute.</param>
    /// <param name="lastActionOutput">The output of the last execution.</param>
    /// <param name="count">The number of times to execute the function.</param>
    /// <returns>A Stopwatch instance with the elapsed time.</returns>
    public static Stopwatch RunWithLast<T>(Func<T> runAction, out T lastActionOutput, int count = 1_000_000)
}
```

## DevBase Tests

### AListTests

```csharp
/// <summary>
/// Tests for the AList generic collection.
/// </summary>
public class AListTests
{
    private int _count;

    /// <summary>
    /// Tests the RemoveRange functionality of AList.
    /// </summary>
    [Test]
    public void RemoveRangeTest()
    
    /// <summary>
    /// Tests the Find functionality of AList with a large dataset.
    /// Measures performance and verifies correctness.
    /// </summary>
    [Test]
    public void FindTest()
}
```

### MultitaskingTest

```csharp
/// <summary>
/// Tests for the Multitasking system.
/// </summary>
public class MultitaskingTest
{
    /// <summary>
    /// Tests task registration and waiting mechanism in Multitasking.
    /// Creates 200 tasks with a capacity of 2 and waits for all to complete.
    /// </summary>
    [Test]
    public async Task MultitaskingRegisterAndWaitTest()
}
```

### StringUtilsTest

```csharp
/// <summary>
/// Tests for StringUtils methods.
/// </summary>
public class StringUtilsTest
{
    private int _count;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    
    /// <summary>
    /// Tests the Separate method for joining string arrays.
    /// Includes a performance test (PenetrationTest).
    /// </summary>
    [Test]
    public void SeparateTest()
    
    /// <summary>
    /// Tests the DeSeparate method for splitting strings.
    /// </summary>
    [Test]
    public void DeSeparateTest()
}
```

### Base64EncodedAStringTest

```csharp
/// <summary>
/// Tests for Base64 encoded string functionality.
/// </summary>
public class Base64EncodedAStringTest
{
    // Test methods for Base64 string encoding/decoding
}
```

### AFileTest

```csharp
/// <summary>
/// Tests for AFile functionality.
/// </summary>
public class AFileTest
{
    // Test methods for file operations
}
```

## DevBase.Api Tests

### AppleMusicTests

```csharp
/// <summary>
/// Tests for the Apple Music API client.
/// </summary>
public class AppleMusicTests
{
    private string _userMediaToken;
    
    /// <summary>
    /// Sets up the test environment.
    /// </summary>
    [SetUp]
    public void SetUp()
    
    /// <summary>
    /// Tests raw search functionality.
    /// </summary>
    [Test]
    public async Task RawSearchTest()
    
    /// <summary>
    /// Tests the simplified Search method.
    /// </summary>
    [Test]
    public async Task SearchTest()
    
    /// <summary>
    /// Tests creation of the AppleMusic object and access token generation.
    /// </summary>
    [Test]
    public async Task CreateObjectTest()
    
    /// <summary>
    /// Tests configuring the user media token from a cookie.
    /// </summary>
    [Test]
    public async Task CreateObjectAndGetUserMediaTokenTest()
    
    /// <summary>
    /// Tests fetching lyrics.
    /// Requires a valid UserMediaToken.
    /// </summary>
    [Test]
    public async Task GetLyricsTest()
}
```

### BeautifulLyricsTests

```csharp
/// <summary>
/// Tests for the Beautiful Lyrics API client.
/// </summary>
public class BeautifulLyricsTests
{
    // Test methods for Beautiful Lyrics API functionality
}
```

### DeezerTests

```csharp
/// <summary>
/// Tests for the Deezer API client.
/// </summary>
public class DeezerTests
{
    // Test methods for Deezer API functionality
}
```

### MusixMatchTest

```csharp
/// <summary>
/// Tests for the MusixMatch API client.
/// </summary>
public class MusixMatchTest
{
    // Test methods for MusixMatch API functionality
}
```

### NetEaseTest

```csharp
/// <summary>
/// Tests for the NetEase API client.
/// </summary>
public class NetEaseTest
{
    // Test methods for NetEase API functionality
}
```

### RefreshTokenTest

```csharp
/// <summary>
/// Tests for token refresh functionality.
/// </summary>
public class RefreshTokenTest
{
    // Test methods for token refresh
}
```

### TidalTests

```csharp
/// <summary>
/// Tests for the Tidal API client.
/// </summary>
public class TidalTests
{
    // Test methods for Tidal API functionality
}
```

## DevBase.Cryptography.BouncyCastle Tests

### AES Tests

```csharp
/// <summary>
/// Tests for AES encryption using BouncyCastle.
/// </summary>
public class AESBuilderEngineTest
{
    // Test methods for AES encryption/decryption
}
```

### Hashing Tests

```csharp
/// <summary>
/// Tests for various hashing algorithms.
/// </summary>
public class Es256TokenVerifierTest
public class Es384TokenVerifierTest
public class Es512TokenVerifierTest
public class Ps256TokenVerifierTest
public class Ps384TokenVerifierTest
public class Ps512TokenVerifierTest
public class Rs256TokenVerifierTest
public class Rs384TokenVerifierTest
public class Rs512TokenVerifierTest
public class Sha256TokenVerifierTest
public class Sha384TokenVerifierTest
public class Sha512TokenVerifierTest
{
    // Test methods for JWT token verification with different algorithms
}
```

## DevBase.Format Tests

### FormatTest

```csharp
/// <summary>
/// Base class for format tests providing helper methods for file access.
/// </summary>
public class FormatTest
{
    /// <summary>
    /// Gets a FileInfo object for a test file located in the DevBaseFormatData directory.
    /// </summary>
    /// <param name="folder">The subfolder name in DevBaseFormatData.</param>
    /// <param name="name">The file name.</param>
    /// <returns>FileInfo object pointing to the test file.</returns>
    public FileInfo GetTestFile(string folder, string name)
}
```

### AppleXmlTester

```csharp
/// <summary>
/// Tests for Apple XML format parsing.
/// </summary>
public class AppleXmlTester : FormatTest
{
    // Test methods for Apple XML format
}
```

### ElrcTester

```csharp
/// <summary>
/// Tests for ELRC (Extended LRC) format parsing.
/// </summary>
public class ElrcTester : FormatTest
{
    // Test methods for ELRC format
}
```

### KLyricsTester

```csharp
/// <summary>
/// Tests for KLyrics format parsing.
/// </summary>
public class KLyricsTester : FormatTest
{
    // Test methods for KLyrics format
}
```

### LrcTester

```csharp
/// <summary>
/// Tests for LRC format parsing.
/// </summary>
public class LrcTester : FormatTest
{
    // Test methods for LRC format
}
```

### RlrcTester

```csharp
/// <summary>
/// Tests for RLRC (Rich LRC) format parsing.
/// </summary>
public class RlrcTester : FormatTest
{
    // Test methods for RLRC format
}
```

### RmmlTester

```csharp
/// <summary>
/// Tests for RMML format parsing.
/// </summary>
public class RmmlTester : FormatTest
{
    // Test methods for RMML format
}
```

### SrtTester

```csharp
/// <summary>
/// Tests for SRT subtitle format parsing.
/// </summary>
public class SrtTester : FormatTest
{
    // Test methods for SRT format
}
```

## DevBase.Net Tests

### BatchRequestsTest

```csharp
/// <summary>
/// Tests for the BatchRequests functionality.
/// </summary>
public class BatchRequestsTest
{
    [Test]
    public void BatchRequests_CreateBatch_ShouldCreateNamedBatch()
    
    [Test]
    public void BatchRequests_CreateBatch_DuplicateName_ShouldThrow()
    
    [Test]
    public void BatchRequests_GetOrCreateBatch_ShouldReturnExistingBatch()
    
    [Test]
    public void BatchRequests_GetOrCreateBatch_ShouldCreateIfNotExists()
    
    [Test]
    public void BatchRequests_RemoveBatch_ShouldRemoveExistingBatch()
    
    [Test]
    public void BatchRequests_RemoveBatch_NonExistent_ShouldReturnFalse()
    
    [Test]
    public void BatchRequests_WithRateLimit_ShouldSetRateLimit()
    
    [Test]
    public void BatchRequests_WithRateLimit_InvalidValue_ShouldThrow()
    
    [Test]
    public void BatchRequests_WithCookiePersistence_ShouldEnable()
    
    [Test]
    public void BatchRequests_WithRefererPersistence_ShouldEnable()
    
    [Test]
    public async Task Batch_Add_ShouldEnqueueRequest()
    
    [Test]
    public void Batch_AddMultiple_ShouldEnqueueAllRequests()
    
    [Test]
    public void Batch_Enqueue_WithConfiguration_ShouldApplyConfiguration()
    
    [Test]
    public void Batch_Clear_ShouldRemoveAllRequests()
    
    [Test]
    public void Batch_EndBatch_ShouldReturnParent()
    
    [Test]
    public void BatchRequests_ClearAllBatches_ShouldClearAllQueues()
    
    [Test]
    public async Task BatchRequests_GetStatistics_ShouldReturnCorrectStats()
    
    [Test]
    public void BatchRequests_ResetCounters_ShouldResetAllCounters()
    
    [Test]
    public void BatchRequests_OnResponse_ShouldRegisterCallback()
    
    [Test]
    public void BatchRequests_OnError_ShouldRegisterCallback()
    
    [Test]
    public void BatchRequests_OnProgress_ShouldRegisterCallback()
    
    [Test]
    public void BatchProgressInfo_PercentComplete_ShouldCalculateCorrectly()
    
    [Test]
    public void BatchProgressInfo_PercentComplete_ZeroTotal_ShouldReturnZero()
    
    [Test]
    public void BatchStatistics_SuccessRate_ShouldCalculateCorrectly()
    
    [Test]
    public void BatchStatistics_SuccessRate_ZeroProcessed_ShouldReturnZero()
    
    [Test]
    public void BatchRequests_FluentApi_ShouldChainCorrectly()
    
    [Test]
    public async Task Batch_FluentApi_ShouldChainCorrectly()
    
    [Test]
    public async Task BatchRequests_MultipleBatches_ShouldTrackTotalQueueCount()
    
    [Test]
    public async Task BatchRequests_Dispose_ShouldCleanupResources()
    
    [Test]
    public void Batch_TryDequeue_ShouldDequeueInOrder()
    
    [Test]
    public void BatchRequests_GetBatch_ExistingBatch_ShouldReturnBatch()
    
    [Test]
    public void BatchRequests_GetBatch_NonExistent_ShouldReturnNull()
    
    [Test]
    public void BatchRequests_ExecuteBatchAsync_NonExistentBatch_ShouldThrow()
    
    [Test]
    public void Batch_EnqueueWithFactory_ShouldUseFactory()
}
```

### BrowserSpoofingTest

```csharp
/// <summary>
/// Tests for browser spoofing functionality.
/// </summary>
public class BrowserSpoofingTest
{
    // Test methods for browser spoofing
}
```

### FileUploadTest

```csharp
/// <summary>
/// Tests for file upload functionality.
/// </summary>
public class FileUploadTest
{
    // Test methods for file uploads
}
```

### HttpToSocks5ProxyTest

```csharp
/// <summary>
/// Tests for HTTP to SOCKS5 proxy conversion.
/// </summary>
public class HttpToSocks5ProxyTest
{
    // Test methods for proxy conversion
}
```

### ParameterBuilderTest

```csharp
/// <summary>
/// Tests for the ParameterBuilder functionality.
/// </summary>
public class ParameterBuilderTest
{
    // Test methods for parameter building
}
```

### UserAgentBuilderTest

```csharp
/// <summary>
/// Tests for the UserAgentBuilder functionality.
/// </summary>
public class UserAgentBuilderTest
{
    // Test methods for user agent building
}
```

### RequestTest

```csharp
/// <summary>
/// Tests for HTTP request functionality.
/// </summary>
public class RequestTest
{
    // Test methods for HTTP requests
}
```

### RequestBuilderTest

```csharp
/// <summary>
/// Tests for the RequestBuilder functionality.
/// </summary>
public class RequestBuilderTest
{
    // Test methods for request building
}
```

### RequestArchitectureTest

```csharp
/// <summary>
/// Tests for request architecture patterns.
/// </summary>
public class RequestArchitectureTest
{
    // Test methods for request architecture
}
```

### RateLimitRetryTest

```csharp
/// <summary>
/// Tests for rate limiting and retry functionality.
/// </summary>
public class RateLimitRetryTest
{
    // Test methods for rate limiting and retries
}
```

### ResponseMultiSelectorTest

```csharp
/// <summary>
/// Tests for response multi-selector functionality.
/// </summary>
public class ResponseMultiSelectorTest
{
    // Test methods for multi-selectors
}
```

### RetryPolicyTest

```csharp
/// <summary>
/// Tests for retry policy functionality.
/// </summary>
public class RetryPolicyTest
{
    // Test methods for retry policies
}
```

### AuthenticationTokenTest

```csharp
/// <summary>
/// Tests for authentication token functionality.
/// </summary>
public class AuthenticationTokenTest
{
    // Test methods for authentication tokens
}
```

### BogusUtilsTests

```csharp
/// <summary>
/// Tests for bogus data generation utilities.
/// </summary>
public class BogusUtilsTests
{
    // Test methods for fake data generation
}
```

### ContentDispositionUtilsTests

```csharp
/// <summary>
/// Tests for Content-Disposition header parsing utilities.
/// </summary>
public class ContentDispositionUtilsTests
{
    // Test methods for Content-Disposition parsing
}
```

### DockerIntegrationTests

```csharp
/// <summary>
/// Integration tests that require Docker.
/// </summary>
public class DockerIntegrationTests
{
    // Docker-based integration test methods
}
```

## DevBaseColor Tests

### ColorCalculator

```csharp
/// <summary>
/// Tests for color calculation utilities.
/// </summary>
public class ColorCalculator
{
    // Test methods for color calculations
}
```

## Test Utilities

The project uses NUnit as the testing framework with the following global usings:

```csharp
global using NUnit.Framework;
```

Test files are organized by the project they test, with each major component having its own test namespace and set of test classes. Tests include unit tests, integration tests, and performance tests using the PenetrationTest helper class.
