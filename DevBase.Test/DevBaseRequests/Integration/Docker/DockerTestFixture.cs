using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests.Integration.Docker;

/// <summary>
/// Test fixture that manages Docker containers for integration tests using Testcontainers.
/// Containers are automatically started before tests and stopped after.
/// </summary>
[SetUpFixture]
public class DockerTestFixture
{
    private static INetwork? _network;
    private static IContainer? _mockApiContainer;
    private static IContainer? _httpProxyContainer;
    private static IContainer? _httpProxyNoAuthContainer;
    private static IContainer? _socks5ProxyContainer;
    private static IContainer? _socks5ProxyNoAuthContainer;
    private static IFutureDockerImage? _mockApiImage;
    private static IFutureDockerImage? _httpProxyAuthImage;
    private static IFutureDockerImage? _httpProxyNoAuthImage;
    
    private static bool _containersStarted;
    private static bool _setupCompleted;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    // Dynamic ports assigned by Testcontainers
    public static int MockApiPort { get; private set; }
    public static int HttpProxyPort { get; private set; }
    public static int HttpProxyNoAuthPort { get; private set; }
    public static int Socks5ProxyPort { get; private set; }
    public static int Socks5ProxyNoAuthPort { get; private set; }
    
    public static string MockApiBaseUrl => $"http://localhost:{MockApiPort}";
    
    /// <summary>
    /// Internal URL for proxy tests - uses Docker network alias.
    /// </summary>
    public static string MockApiInternalUrl => "http://mock-api:5080";

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_setupCompleted) return;
            
            TestContext.Progress.WriteLine("=== Testcontainers Integration Test Setup ===");
            
            var dockerDir = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "..", "..", "..",
                DockerTestConstants.DockerComposeDirectory);

            // Create network
            TestContext.Progress.WriteLine("Creating Docker network...");
            _network = new NetworkBuilder()
                .WithName($"devbase-test-{Guid.NewGuid():N}")
                .Build();
            await _network.CreateAsync();

            // Build Mock API image
            TestContext.Progress.WriteLine("Building Mock API image...");
            _mockApiImage = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory(Path.Combine(dockerDir, "MockApi"))
                .WithDockerfile("Dockerfile")
                .WithName($"devbase-mockapi-test:{Guid.NewGuid():N}")
                .WithCleanUp(true)
                .Build();
            await _mockApiImage.CreateAsync();

            // Start Mock API container
            TestContext.Progress.WriteLine("Starting Mock API container...");
            _mockApiContainer = new ContainerBuilder()
                .WithImage(_mockApiImage)
                .WithNetwork(_network)
                .WithNetworkAliases("mock-api")
                .WithPortBinding(5080, true)
                .WithEnvironment("ASPNETCORE_URLS", "http://+:5080")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(5080).ForPath("/health")))
                .Build();
            await _mockApiContainer.StartAsync();
            MockApiPort = _mockApiContainer.GetMappedPublicPort(5080);
            TestContext.Progress.WriteLine($"  Mock API running on port {MockApiPort}");

            // Build HTTP Proxy with auth image
            TestContext.Progress.WriteLine("Building HTTP Proxy (with auth) image...");
            _httpProxyAuthImage = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory(Path.Combine(dockerDir, "Proxies"))
                .WithDockerfile("Dockerfile.tinyproxy-auth")
                .WithName($"devbase-httpproxy-auth:{Guid.NewGuid():N}")
                .WithCleanUp(true)
                .Build();
            await _httpProxyAuthImage.CreateAsync();

            // Start HTTP Proxy with auth
            TestContext.Progress.WriteLine("Starting HTTP Proxy (with auth)...");
            _httpProxyContainer = new ContainerBuilder()
                .WithImage(_httpProxyAuthImage)
                .WithNetwork(_network)
                .WithNetworkAliases("http-proxy")
                .WithPortBinding(8888, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8888))
                .Build();
            await _httpProxyContainer.StartAsync();
            HttpProxyPort = _httpProxyContainer.GetMappedPublicPort(8888);
            TestContext.Progress.WriteLine($"  HTTP Proxy (auth) running on port {HttpProxyPort}");

            // Build HTTP Proxy without auth image
            TestContext.Progress.WriteLine("Building HTTP Proxy (no auth) image...");
            _httpProxyNoAuthImage = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory(Path.Combine(dockerDir, "Proxies"))
                .WithDockerfile("Dockerfile.tinyproxy-noauth")
                .WithName($"devbase-httpproxy-noauth:{Guid.NewGuid():N}")
                .WithCleanUp(true)
                .Build();
            await _httpProxyNoAuthImage.CreateAsync();

            // Start HTTP Proxy without auth
            TestContext.Progress.WriteLine("Starting HTTP Proxy (no auth)...");
            _httpProxyNoAuthContainer = new ContainerBuilder()
                .WithImage(_httpProxyNoAuthImage)
                .WithNetwork(_network)
                .WithNetworkAliases("http-proxy-noauth")
                .WithPortBinding(8888, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8888))
                .Build();
            await _httpProxyNoAuthContainer.StartAsync();
            HttpProxyNoAuthPort = _httpProxyNoAuthContainer.GetMappedPublicPort(8888);
            TestContext.Progress.WriteLine($"  HTTP Proxy (no auth) running on port {HttpProxyNoAuthPort}");

            // Start SOCKS5 Proxy with auth (microsocks)
            TestContext.Progress.WriteLine("Starting SOCKS5 Proxy (with auth)...");
            _socks5ProxyContainer = new ContainerBuilder()
                .WithImage("vimagick/microsocks:latest")
                .WithNetwork(_network)
                .WithNetworkAliases("socks5-proxy")
                .WithPortBinding(1080, true)
                .WithCommand("-u", DockerTestConstants.Socks5ProxyUsername, "-P", DockerTestConstants.Socks5ProxyPassword)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1080))
                .Build();
            await _socks5ProxyContainer.StartAsync();
            Socks5ProxyPort = _socks5ProxyContainer.GetMappedPublicPort(1080);
            TestContext.Progress.WriteLine($"  SOCKS5 Proxy (auth) running on port {Socks5ProxyPort}");

            // Start SOCKS5 Proxy without auth
            TestContext.Progress.WriteLine("Starting SOCKS5 Proxy (no auth)...");
            _socks5ProxyNoAuthContainer = new ContainerBuilder()
                .WithImage("vimagick/microsocks:latest")
                .WithNetwork(_network)
                .WithNetworkAliases("socks5-proxy-noauth")
                .WithPortBinding(1080, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1080))
                .Build();
            await _socks5ProxyNoAuthContainer.StartAsync();
            Socks5ProxyNoAuthPort = _socks5ProxyNoAuthContainer.GetMappedPublicPort(1080);
            TestContext.Progress.WriteLine($"  SOCKS5 Proxy (no auth) running on port {Socks5ProxyNoAuthPort}");

            _containersStarted = true;
            _setupCompleted = true;
            TestContext.Progress.WriteLine("=== All containers started successfully ===");
        }
        catch (System.Exception ex)
        {
            TestContext.Progress.WriteLine($"ERROR: Failed to start containers: {ex.Message}");
            TestContext.Progress.WriteLine(ex.StackTrace ?? "");
            _setupCompleted = true;
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        TestContext.Progress.WriteLine("=== Testcontainers Teardown ===");
        
        // Stop and dispose containers
        if (_socks5ProxyNoAuthContainer != null)
        {
            await _socks5ProxyNoAuthContainer.DisposeAsync();
            TestContext.Progress.WriteLine("  SOCKS5 Proxy (no auth) stopped.");
        }
        
        if (_socks5ProxyContainer != null)
        {
            await _socks5ProxyContainer.DisposeAsync();
            TestContext.Progress.WriteLine("  SOCKS5 Proxy (auth) stopped.");
        }
        
        if (_httpProxyNoAuthContainer != null)
        {
            await _httpProxyNoAuthContainer.DisposeAsync();
            TestContext.Progress.WriteLine("  HTTP Proxy (no auth) stopped.");
        }
        
        if (_httpProxyContainer != null)
        {
            await _httpProxyContainer.DisposeAsync();
            TestContext.Progress.WriteLine("  HTTP Proxy (auth) stopped.");
        }
        
        if (_mockApiContainer != null)
        {
            await _mockApiContainer.DisposeAsync();
            TestContext.Progress.WriteLine("  Mock API stopped.");
        }

        if (_mockApiImage != null)
        {
            await _mockApiImage.DisposeAsync();
            TestContext.Progress.WriteLine("  Mock API image cleaned up.");
        }

        if (_httpProxyAuthImage != null)
        {
            await _httpProxyAuthImage.DisposeAsync();
            TestContext.Progress.WriteLine("  HTTP Proxy (auth) image cleaned up.");
        }

        if (_httpProxyNoAuthImage != null)
        {
            await _httpProxyNoAuthImage.DisposeAsync();
            TestContext.Progress.WriteLine("  HTTP Proxy (no auth) image cleaned up.");
        }

        if (_network != null)
        {
            await _network.DeleteAsync();
            await _network.DisposeAsync();
            TestContext.Progress.WriteLine("  Network removed.");
        }
        
        TestContext.Progress.WriteLine("=== Teardown complete ===");
    }

    /// <summary>
    /// Returns true if containers were started by this fixture.
    /// </summary>
    public static bool ContainersStarted => _containersStarted;

    /// <summary>
    /// Checks if Docker services are available for tests.
    /// </summary>
    public static async Task<bool> AreServicesAvailable()
    {
        if (!_containersStarted)
        {
            TestContext.Progress.WriteLine("AreServicesAvailable: _containersStarted is false");
            return false;
        }
        
        if (MockApiPort == 0)
        {
            TestContext.Progress.WriteLine("AreServicesAvailable: MockApiPort is 0");
            return false;
        }
        
        try
        {
            var url = $"{MockApiBaseUrl}/health";
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            var response = await client.GetAsync(url);
            var result = response.IsSuccessStatusCode;
            if (!result)
            {
                TestContext.Progress.WriteLine($"AreServicesAvailable: Health check to {url} returned {response.StatusCode}");
            }
            return result;
        }
        catch (System.Exception ex)
        {
            TestContext.Progress.WriteLine($"AreServicesAvailable: Exception checking {MockApiBaseUrl}/health - {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Gets the HTTP proxy URL with authentication.
    /// </summary>
    public static string HttpProxyUrlWithAuth => 
        $"http://{DockerTestConstants.HttpProxyUsername}:{DockerTestConstants.HttpProxyPassword}@localhost:{HttpProxyPort}";
    
    /// <summary>
    /// Gets the HTTP proxy URL without authentication.
    /// </summary>
    public static string HttpProxyNoAuthUrl => $"http://localhost:{HttpProxyNoAuthPort}";
    
    /// <summary>
    /// Gets the SOCKS5 proxy URL with authentication.
    /// </summary>
    public static string Socks5ProxyUrlWithAuth => 
        $"socks5://{DockerTestConstants.Socks5ProxyUsername}:{DockerTestConstants.Socks5ProxyPassword}@localhost:{Socks5ProxyPort}";
    
    /// <summary>
    /// Gets the SOCKS5 proxy URL without authentication.
    /// </summary>
    public static string Socks5ProxyNoAuthUrl => $"socks5://localhost:{Socks5ProxyNoAuthPort}";
}

/// <summary>
/// Base class for Docker-based integration tests.
/// The Docker containers are automatically started by DockerTestFixture when tests run.
/// </summary>
public abstract class DockerIntegrationTestBase
{
    [SetUp]
    public async Task CheckDockerServices()
    {
        if (!await DockerTestFixture.AreServicesAvailable())
        {
            Assert.Ignore("Docker services are not available. Container startup may have failed.");
        }
    }

    protected static string ApiUrl(string path) => $"{DockerTestFixture.MockApiBaseUrl}{path}";
    
    /// <summary>
    /// URL for proxy tests - uses Docker network internal address.
    /// </summary>
    protected static string ProxyApiUrl(string path) => $"{DockerTestFixture.MockApiInternalUrl}{path}";
}
