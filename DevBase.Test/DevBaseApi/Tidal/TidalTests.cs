using System.Collections.ObjectModel;
using DevBase.Api.Apis.Tidal;
using DevBase.Api.Apis.Tidal.Structure.Json;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace DevBase.Test.DevBaseApi.Tidal;

public class TidalTests
{
    private string _authToken;
    private string _deviceCode;
    private string _accessToken;
    private string _refreshToken;
    
    [SetUp]
    public void Setup()
    {
        this._authToken = "";
        this._deviceCode = "";
        this._accessToken = "";
        this._refreshToken = "";
    }
    
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

    [Test]
    public async Task RegisterDevice()
    {
        Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
        var deviceRegister = await client.RegisterDevice();
        
        Assert.That(deviceRegister.VerificationUri, Is.EqualTo("link.tidal.com"));
    }

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

    [Test]
    public async Task Search()
    {
        Api.Apis.Tidal.Tidal client = new Api.Apis.Tidal.Tidal();
        var search = await client.Search("Hero");
        
        Assert.That(search.Items, Is.Not.Null);
    }

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
