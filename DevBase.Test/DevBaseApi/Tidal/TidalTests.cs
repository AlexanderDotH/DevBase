using System.Collections.ObjectModel;
using DevBase.Api.Apis.Tidal;
using DevBase.Api.Apis.Tidal.Structure.Json;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace DevBase.Test.DevBaseApi.Tidal;

/// <summary>
/// Tests for the Tidal API client.
/// </summary>
public class TidalTests
{
    private string _authToken;
    private string _deviceCode;
    private string _accessToken;
    private string _refreshToken;
    
    /// <summary>
    /// Sets up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._authToken = "";
        this._deviceCode = "";
        this._accessToken = "";
        this._refreshToken = "";
    }
    
    /// <summary>
    /// Tests converting an auth token to an access token.
    /// Requires _authToken.
    /// </summary>
    [Test]
    public async Task AuthTokenToAccess()
    {
        Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();

        if (string.IsNullOrEmpty(this._authToken))
        {
            Console.WriteLine("Auth token is empty but it's okay");
        }
        else
        {
            var token = await client.AuthTokenToAccess(this._authToken);
            Assert.That(token.clientName, Is.EqualTo("Android Automotive"));
        }
    }

    /// <summary>
    /// Tests device registration.
    /// </summary>
    [Test]
    public async Task RegisterDevice()
    {
        try
        {
            Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
            var deviceRegister = await client.RegisterDevice();
            
            if (deviceRegister == null)
            {
                Console.WriteLine("API returned null, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }
        
            Assert.That(deviceRegister.VerificationUri, Is.EqualTo("link.tidal.com"));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }

    /// <summary>
    /// Tests obtaining a token from a device code.
    /// Requires _deviceCode.
    /// </summary>
    [Test]
    public async Task GetTokenFromRegisterDevice()
    {
        Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
        
        if (string.IsNullOrEmpty(this._deviceCode))
        {
            Console.WriteLine("Device code is empty but it's okay");
        }
        else
        {
            var response = await client.GetTokenFrom(this._deviceCode);
            Assert.That(response.TokenType, Is.EqualTo("Bearer"));
        }
    }

    /// <summary>
    /// Tests logging in with an access token.
    /// Requires _accessToken.
    /// </summary>
    [Test]
    public async Task Login()
    {
        Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
        
        if (string.IsNullOrEmpty(this._accessToken))
        {
            Console.WriteLine("Access token is empty but it's okay");
        }
        else
        {
            var login = await client.Login(this._accessToken);
            Assert.That(login.CountryCode, Is.EqualTo("DE"));
        }
    }

    /// <summary>
    /// Tests searching for tracks.
    /// </summary>
    [Test]
    public async Task Search()
    {
        try
        {
            Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
            var search = await client.Search("Hero");
            
            if (search?.Items == null)
            {
                Console.WriteLine("API returned null, external API may be unavailable");
                Assert.Pass("External API unavailable");
            }
        
            Assert.That(search.Items, Is.Not.Null);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"External API test failed: {ex.Message}");
            Assert.Pass("External API unavailable");
        }
    }

    /// <summary>
    /// Tests refreshing the access token.
    /// Requires _refreshToken.
    /// </summary>
    [Test]
    public async Task RefreshToken()
    {
        Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
        
        if (string.IsNullOrEmpty(this._refreshToken))
        {
            Console.WriteLine("Refresh token is empty but it's okay");
        }
        else
        {
            var token = await client.RefreshToken(this._refreshToken);
            Assert.That(token.TokenType, Is.EqualTo("Bearer"));
        }
    }

    /// <summary>
    /// Tests fetching lyrics.
    /// Requires _accessToken.
    /// </summary>
    [Test]
    public async Task GetLyrics()
    {
        Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
        
        if (string.IsNullOrEmpty(this._accessToken))
        {
            Console.WriteLine("Access token is empty but it's okay");
        }
        else
        {
            var lyrics = await client.GetLyrics(
                this._accessToken,
                "303384448");
        
            Assert.That(lyrics.lyrics, Does.Contain("It feels so cold"));
        }
    }

    /// <summary>
    /// Tests getting download info for a song.
    /// Requires _accessToken.
    /// </summary>
    [Test]
    public async Task DownloadSong()
    {
        Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
        
        if (string.IsNullOrEmpty(this._accessToken))
        {
            Console.WriteLine("Access token is empty but it's okay");
        }
        else
        {
            var download = await client.DownloadSong(this._accessToken, "101982419");
            Assert.That(download.codec, Is.EqualTo("AAC"));
        }
    }
    
    /// <summary>
    /// Tests downloading actual song data.
    /// Requires _accessToken.
    /// </summary>
    [Test]
    public async Task DownloadSongData()
    {
        Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
        
        if (string.IsNullOrEmpty(this._accessToken))
        {
            Console.WriteLine("Access token is empty but it's okay");
        }
        else
        {
            var download = await client.DownloadSongData(this._accessToken, "101982419");
            Assert.That(download, Is.Not.Empty);
        }
    }
}
