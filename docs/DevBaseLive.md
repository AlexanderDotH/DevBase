# DevBaseLive

DevBaseLive is a console application designed as a comprehensive test suite and performance benchmark for the `DevBase.Requests` library. It verifies functional correctness, measures performance, and compares results against the standard .NET `HttpClient`.

## Overview

The application performs three main categories of tests:

1.  **Functional Tests**: Verifies that individual features work as expected (GET/POST, Headers, Auth, Parsing).
2.  **Performance Tests**: Measures the execution time of different parsing strategies (JsonPath vs. Full Deserialization).
3.  **Comparison Tests**: Benchmarks `DevBase.Requests` against `HttpClient` to ensure competitive performance and data integrity.

## Test Suite Details

### Functional Tests
Ensures the core reliability of the library.
- **GET/POST Requests**: Basic connectivity and payload transmission.
- **JSON Parsing**: Verifies generic deserialization.
- **JsonPath**: Tests the streaming JsonPath extractor.
- **Header Validation**: Checks valid Accept/Content-Type headers.
- **JWT Parsing**: Validates token structure and expiration logic.
- **Timeouts**: Confirms that request timeouts throw appropriate exceptions.

### Performance Tests
Runs multiple iterations to calculate average response times for:
1.  **DevBase + JsonPath**: Extracting specific fields without full object deserialization.
2.  **HttpClient + JsonSerializer**: Standard approach.
3.  **DevBase + Full Deserialization**: Full object mapping using DevBase.

### Comparison Tests
- **Response Time**: Checks if DevBase is within an acceptable margin of HttpClient (aims for <= 1.5x of HttpClient's raw speed, usually competitive).
- **Data Integrity**: Ensures all methods return identical data sets.

## Usage

To run the test suite:

```bash
cd DevBaseLive
dotnet run
```

**Sample Output:**
```text
╔══════════════════════════════════════════════════════════╗
║  DevBase.Requests Test Suite                             ║
╚══════════════════════════════════════════════════════════╝

  Warming up HTTP connections...

╔══════════════════════════════════════════════════════════╗
║  Functional Tests                                        ║
╚══════════════════════════════════════════════════════════╝

  GET Request - Basic                          PASS  (150ms)
  GET Request - JSON Parsing                   PASS  (200ms)
  ...
```
