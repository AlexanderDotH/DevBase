using System.Net;
using DevBase.Net.Configuration;
using DevBase.Net.Configuration.Enums;
using DevBase.Net.Core;
using DevBase.Net.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Test.DevBaseRequests.Integration;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
[Category("Unit")]
public class BrowserSpoofingTest
{
    private MockHttpServer _server = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _server = new MockHttpServer();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _server.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        _server.ResetCounters();
    }

    #region Browser Profile Application

    [Test]
    public async Task WithScrapingBypass_ChromeProfile_AppliesChromeHeaders()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Chrome should have User-Agent, sec-ch-ua headers
        Assert.That(content, Does.Contain("User-Agent"));
        Assert.That(content, Does.Contain("Chrome"));
    }

    [Test]
    public async Task WithScrapingBypass_FirefoxProfile_AppliesFirefoxHeaders()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Firefox
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Firefox should have User-Agent with Firefox
        Assert.That(content, Does.Contain("User-Agent"));
        Assert.That(content, Does.Contain("Firefox"));
    }

    [Test]
    public async Task WithScrapingBypass_EdgeProfile_AppliesEdgeHeaders()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Edge
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Edge should have User-Agent with Edg
        Assert.That(content, Does.Contain("User-Agent"));
        Assert.That(content, Does.Contain("Edg"));
    }

    [Test]
    public async Task WithScrapingBypass_SafariProfile_AppliesSafariHeaders()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Safari
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Safari should have User-Agent
        Assert.That(content, Does.Contain("User-Agent"));
    }

    [Test]
    public async Task WithScrapingBypass_NoneProfile_DoesNotApplyHeaders()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.None
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region User Header Priority

    [Test]
    public async Task WithScrapingBypass_CaseInsensitiveHeaderOverwrite_UserValueWins()
    {
        // Regression test: Firefox sets "Upgrade-Insecure-Requests":"1" (capitalized)
        // User sets "upgrade-insecure-requests":"0" (lowercase)
        // Should result in single header with value "0", not "0, 1"
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Firefox
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config)
            .WithHeader("upgrade-insecure-requests", "0");

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Should contain "0" but NOT "0, 1" (concatenated) or just "1"
        Assert.That(content, Does.Contain("\"Upgrade-Insecure-Requests\":\"0\"").Or.Contain("\"upgrade-insecure-requests\":\"0\""));
        Assert.That(content, Does.Not.Contain("0, 1"));
        Assert.That(content, Does.Not.Contain("1, 0"));
    }

    [Test]
    public async Task WithScrapingBypass_UserHeaderSetBefore_UserHeaderTakesPriority()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithHeader("X-Custom-Header", "CustomValue")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // User-set headers via WithHeader() should be preserved
        Assert.That(content, Does.Contain("X-Custom-Header"));
        Assert.That(content, Does.Contain("CustomValue"));
    }

    [Test]
    public async Task WithScrapingBypass_UserHeaderSetAfter_UserHeaderTakesPriority()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config)
            .WithHeader("X-Custom-Header", "CustomValue");

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // User-set headers via WithHeader() should be preserved
        Assert.That(content, Does.Contain("X-Custom-Header"));
        Assert.That(content, Does.Contain("CustomValue"));
    }

    [Test]
    public async Task WithScrapingBypass_CustomAcceptHeader_UserHeaderTakesPriority()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithAccept("application/custom")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // User-set Accept should override Chrome's
        Assert.That(content, Does.Contain("application/custom"));
    }

    [Test]
    public async Task WithScrapingBypass_MultipleCustomHeaders_AllUserHeadersTakePriority()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Firefox
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithHeader("X-Custom-1", "Value1")
            .WithHeader("X-Custom-2", "Value2")
            .WithHeader("Accept-Language", "en-US")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // All user headers via WithHeader() should be preserved
        Assert.That(content, Does.Contain("X-Custom-1"));
        Assert.That(content, Does.Contain("Value1"));
        Assert.That(content, Does.Contain("X-Custom-2"));
        Assert.That(content, Does.Contain("Value2"));
        Assert.That(content, Does.Contain("en-US"));
    }

    [Test]
    public async Task WithScrapingBypass_WithUserAgent_UserAgentTakesPriority()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithUserAgent("MyCustomUserAgent/1.0")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // User-set User-Agent via WithUserAgent() should override Chrome's
        Assert.That(content, Does.Contain("MyCustomUserAgent/1.0"));
        Assert.That(content, Does.Not.Contain("Chrome/"));
    }

    [Test]
    public async Task WithScrapingBypass_WithBogusUserAgentGeneric_BogusUserAgentTakesPriority()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithBogusUserAgent<BogusFirefoxUserAgentGenerator>()
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // User-set Firefox bogus User-Agent should override Chrome spoofing
        Assert.That(content, Does.Contain("Firefox"));
    }

    [Test]
    public async Task WithScrapingBypass_WithBogusUserAgent_BogusUserAgentTakesPriority()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Firefox
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithBogusUserAgent()
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // User-set bogus User-Agent should be present
        Assert.That(content, Does.Contain("User-Agent"));
    }

    [Test]
    public async Task WithScrapingBypass_WithUserAgentAfterConfig_UserAgentTakesPriority()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome
        };

        // User-Agent set AFTER WithScrapingBypass - should still take priority
        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config)
            .WithUserAgent("MyCustomUserAgent/2.0");

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // User-set User-Agent should override Chrome's even when set after config
        Assert.That(content, Does.Contain("MyCustomUserAgent/2.0"));
    }

    #endregion

    #region Referer Strategy

    [Test]
    public async Task WithScrapingBypass_BaseHostReferer_AppliesBaseHostReferer()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            RefererStrategy = EnumRefererStrategy.BaseHost
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Should have Referer header with base host
        Assert.That(content, Does.Contain("Referer"));
    }

    [Test]
    public async Task WithScrapingBypass_SearchEngineReferer_AppliesSearchEngineReferer()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            RefererStrategy = EnumRefererStrategy.SearchEngine
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Should have Referer header with search engine
        Assert.That(content, Does.Contain("Referer"));
    }

    [Test]
    public async Task WithScrapingBypass_NoneReferer_DoesNotApplyReferer()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            RefererStrategy = EnumRefererStrategy.None
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task WithScrapingBypass_UserRefererSet_UserRefererTakesPriority()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            RefererStrategy = EnumRefererStrategy.SearchEngine
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithReferer("https://mycustomreferer.com")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // User-set Referer should override strategy
        Assert.That(content, Does.Contain("mycustomreferer.com"));
    }

    #endregion

    #region Combined Configuration

    [Test]
    public async Task WithScrapingBypass_ChromeWithSearchEngineReferer_AppliesBoth()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome,
            RefererStrategy = EnumRefererStrategy.SearchEngine
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Should have both Chrome headers and Referer
        Assert.That(content, Does.Contain("User-Agent"));
        Assert.That(content, Does.Contain("Chrome"));
        Assert.That(content, Does.Contain("Referer"));
    }

    [Test]
    public async Task WithScrapingBypass_DefaultConfig_AppliesChromeWithPreviousUrlStrategy()
    {
        // Arrange
        var config = ScrapingBypassConfig.Default;

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Default should apply Chrome profile
        Assert.That(content, Does.Contain("User-Agent"));
        Assert.That(content, Does.Contain("Chrome"));
    }

    #endregion

    #region No Configuration

    [Test]
    public async Task WithoutScrapingBypass_NoSpoofingApplied()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/headers");

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        // No spoofing should be applied
    }

    #endregion

    #region Build Idempotency

    [Test]
    public async Task WithScrapingBypass_MultipleBuildCalls_AppliesOnlyOnce()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config);

        // Act - Build multiple times
        request.Build();
        request.Build();
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        // Should work correctly even with multiple build calls
    }

    #endregion

    #region Integration with Other Features

    [Test]
    public async Task WithScrapingBypass_WithAuthentication_BothApplied()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Chrome
        };

        var request = new Request($"{_server.BaseUrl}/api/auth")
            .WithScrapingBypass(config)
            .UseBearerAuthentication("test-token");

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Should have Authorization header
        Assert.That(content, Does.Contain("Bearer test-token"));
    }

    [Test]
    public async Task WithScrapingBypass_WithCustomHeaders_AllApplied()
    {
        // Arrange
        var config = new ScrapingBypassConfig
        {
            BrowserProfile = EnumBrowserProfile.Firefox
        };

        var request = new Request($"{_server.BaseUrl}/api/headers")
            .WithScrapingBypass(config)
            .WithHeader("X-Custom-Header", "CustomValue")
            .WithHeader("X-API-Key", "secret-key");

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        
        // Should have Firefox headers and custom headers
        Assert.That(content, Does.Contain("User-Agent"));
        Assert.That(content, Does.Contain("X-Custom-Header"));
        Assert.That(content, Does.Contain("X-API-Key"));
    }

    #endregion
}
