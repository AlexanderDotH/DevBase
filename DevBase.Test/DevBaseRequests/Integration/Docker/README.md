# Docker Integration Tests for DevBase.Net

This directory contains Docker-based integration tests using **Testcontainers** that simulate real-world network conditions including proxies, rate limits, and various HTTP scenarios.

## Prerequisites

- Docker Desktop installed and running
- .NET 9.0 SDK
- Sufficient disk space for container images

## How It Works

The tests use [Testcontainers for .NET](https://dotnet.testcontainers.org/) to automatically:
1. **Create a Docker network** for container communication
2. **Build the Mock API image** from the Dockerfile
3. **Start all containers** with dynamic port allocation
4. **Wait for services** to be healthy before running tests
5. **Clean up** all containers and networks after tests complete

No manual Docker commands needed - just run the tests!

## Quick Start

```powershell
# Navigate to Docker test directory
cd DevBase.Test/DevBaseRequests/Integration/Docker

# Run all Docker integration tests
.\run-docker-tests.ps1

# Run with options
.\run-docker-tests.ps1 -SkipBuild           # Skip rebuilding containers
.\run-docker-tests.ps1 -KeepContainers      # Don't stop containers after tests
.\run-docker-tests.ps1 -Filter "Proxy"      # Run only tests matching pattern
```

## Manual Docker Management

```powershell
# Start containers
docker compose up -d --build

# View logs
docker compose logs -f

# Stop containers
docker compose down -v --remove-orphans

# Check container status
docker compose ps
```

## Architecture

### Services

| Service | Port | Description |
|---------|------|-------------|
| `mock-api` | 5080 | ASP.NET mock API server |
| `http-proxy` | 8888 | HTTP proxy with authentication |
| `http-proxy-noauth` | 8889 | HTTP proxy without authentication |
| `socks5-proxy` | 1080 | SOCKS5 proxy with authentication |
| `socks5-proxy-noauth` | 1081 | SOCKS5 proxy without authentication |

### Proxy Credentials

- **HTTP Proxy (8888)**: `testuser:testpass`
- **SOCKS5 Proxy (1080)**: `testuser:testpass`

## Test Categories

### 1. HTTP Fundamentals (`HttpFundamentalsDockerTest`)
- GET, POST, PUT, DELETE, PATCH requests
- Query parameters and headers
- File uploads (single and multiple)
- Basic and Bearer authentication
- Error responses
- Large responses
- Timeout handling

### 2. Retry & Rate Limiting (`RetryAndRateLimitDockerTest`)
- Exponential backoff
- Linear backoff
- Fixed backoff
- Max retries exceeded
- Rate limit handling (429)
- Timeout with retry

### 3. Proxy Protocols (`ProxyProtocolDockerTest`)
- HTTP proxy with/without authentication
- SOCKS5 proxy with/without authentication
- SOCKS5h (remote DNS) proxy
- Proxy failure tracking
- File upload through proxy
- Concurrent requests through proxy

### 4. Batch Processing (`BatchProcessingDockerTest`)
- Basic batch execution
- Concurrency control
- Rate limiting
- Progress callbacks
- Response/error callbacks
- Multiple batches
- Requeue behavior
- Stop and resume

### 5. Proxy Rotation (`ProxyRotationDockerTest`)
- Round-robin rotation
- Random rotation
- Least-failures rotation
- Sticky rotation
- Dynamic proxy addition
- Max proxy retries
- Concurrent proxy access
- Mixed proxy types

## Mock API Endpoints

### Basic HTTP Methods
```
GET  /api/get                 - Simple GET response
POST /api/post                - Echo POST body
PUT  /api/put                 - Echo PUT body
DELETE /api/delete/{id}       - Delete by ID
PATCH /api/patch/{id}         - Partial update
```

### Query & Headers
```
GET /api/query                - Echo query parameters
GET /api/headers              - Echo request headers
GET /api/echo-header/{name}   - Echo specific header
```

### File Upload
```
POST /api/upload              - Multiple file upload
POST /api/upload-single       - Single file upload
```

### Rate Limiting
```
GET /api/rate-limited         - Rate limited (5 req/10s)
GET /api/rate-limit-strict    - Returns 429 first 2 times
POST /api/rate-limit/reset    - Reset rate limit state
```

### Retry Simulation
```
GET /api/retry-eventually     - Fails first 3 times, then succeeds
GET /api/fail-once            - Fails first time only
GET /api/always-fail          - Always returns 500
```

### Delays
```
GET /api/delay/{ms}           - Delayed response
GET /api/timeout              - 5-minute delay (for timeout testing)
```

### Errors
```
GET /api/error/{code}         - Return specific HTTP status code
GET /api/error/random         - Random error status
```

### Authentication
```
GET /api/auth/basic           - Basic auth (testuser:testpass)
GET /api/auth/bearer          - Bearer token (valid-test-token)
```

### Other
```
GET /api/cookies/set          - Set test cookies
GET /api/cookies/get          - Echo received cookies
GET /api/large/{count}        - Large JSON response
GET /api/stream/{chunks}      - Streaming response
GET /api/proxy-check          - Detect proxy headers
GET /api/batch/{id}           - Batch test endpoint
POST /api/batch/submit        - Batch submission
```

## Running Individual Test Classes

```bash
# Run specific test class
dotnet test --filter "FullyQualifiedName~HttpFundamentalsDockerTest"
dotnet test --filter "FullyQualifiedName~RetryAndRateLimitDockerTest"
dotnet test --filter "FullyQualifiedName~ProxyProtocolDockerTest"
dotnet test --filter "FullyQualifiedName~BatchProcessingDockerTest"
dotnet test --filter "FullyQualifiedName~ProxyRotationDockerTest"

# Run specific test
dotnet test --filter "FullyQualifiedName~Get_SimpleRequest_ReturnsOk"
```

## Troubleshooting

### Containers won't start
```powershell
# Check for port conflicts
netstat -an | findstr "5080 8888 8889 1080 1081"

# Force rebuild
docker compose down -v
docker compose up -d --build --force-recreate
```

### Tests fail with "Docker services not available"
```powershell
# Verify containers are running
docker compose ps

# Check container health
docker compose logs mock-api

# Test connectivity manually
curl http://localhost:5080/health
```

### Proxy authentication failures
- Verify credentials in `DockerTestConstants.cs` match `tinyproxy.conf`
- Check proxy container logs: `docker compose logs http-proxy`

## Adding New Tests

1. Create test class in `Docker/` directory
2. Inherit from `DockerIntegrationTestBase`
3. Add `[Category("Docker")]` attribute
4. Use `ApiUrl()` helper for Mock API URLs
5. Use constants from `DockerTestConstants`

Example:
```csharp
[TestFixture]
[Category("Integration")]
[Category("Docker")]
public class MyNewDockerTest : DockerIntegrationTestBase
{
    [Test]
    public async Task MyTest_Scenario_ExpectedResult()
    {
        var response = await new Request(ApiUrl("/api/get"))
            .SendAsync();
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
```

## Files

```
Docker/
├── docker-compose.yml           # Container orchestration
├── DockerTestConstants.cs       # Ports, URLs, credentials
├── DockerTestFixture.cs         # Test setup/teardown
├── run-docker-tests.ps1         # Test runner script
├── README.md                    # This file
├── MockApi/
│   ├── Dockerfile               # Mock API container
│   ├── MockApi.csproj           # Project file
│   └── Program.cs               # API endpoints
├── Proxies/
│   ├── tinyproxy.conf           # HTTP proxy with auth
│   └── tinyproxy-noauth.conf    # HTTP proxy without auth
└── Tests/
    ├── HttpFundamentalsDockerTest.cs
    ├── RetryAndRateLimitDockerTest.cs
    ├── ProxyProtocolDockerTest.cs
    ├── BatchProcessingDockerTest.cs
    └── ProxyRotationDockerTest.cs
```
