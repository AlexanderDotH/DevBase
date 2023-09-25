﻿using System.Collections.ObjectModel;
using DevBase.Api.Apis.Tidal;
using DevBase.Api.Apis.Tidal.Structure.Json;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace DevBase.Test.DevBaseApi.Tidal;

public class TidalTests
{
    [Test]
    public async Task AuthTokenToAccess()
    {
        TidalClient client = new TidalClient();
        var token = await client.AuthTokenToAccess(Environment.GetEnvironmentVariable("TIDAL_AUTH_TOKEN"));
        
        Assert.AreEqual(token.clientName, "Android Automotive");
    }

    [Test]
    public async Task RegisterDevice()
    {
        TidalClient client = new TidalClient();
        var deviceRegister = await client.RegisterDevice();
        
        Assert.AreEqual(deviceRegister.VerificationUri, "link.tidal.com");
    }

    [Test]
    public async Task GetTokenFromRegisterDevice()
    {
        TidalClient client = new TidalClient();
        var response = await client.GetTokenFrom("36df9fbf-1513-4bcd-a618-49c9c4a04a74");
        
        //Assert.AreEqual(response.TokenType, "Bearer");
    }

    [Test]
    public async Task Login()
    {
        TidalClient client = new TidalClient();
        var login = await client.Login(Environment.GetEnvironmentVariable("TIDAL_ACCESS_TOKEN"));
        
        Assert.AreEqual(login.CountryCode, "DE");
    }

    [Test]
    public async Task Search()
    {
        TidalClient client = new TidalClient();
        var search = await client.Search("Hero");
        
        Assert.NotNull(search.Items);
    }

    [Test]
    public async Task RefreshToken()
    {
        TidalClient client = new TidalClient();
        var token = await client.RefreshToken(Environment.GetEnvironmentVariable("TIDAL_REFRESH_TOKEN"));

        Assert.AreEqual(token.TokenType, "Bearer");
    }

    [Test]
    public async Task GetLyrics()
    {
        TidalClient client = new TidalClient();
        var lyrics = await client.GetLyrics(
            Environment.GetEnvironmentVariable("TIDAL_ACCESS_TOKEN"),
            "303384448");
        
        Assert.IsTrue(lyrics.lyrics.Contains("It feels so cold"));
    }

    [Test]
    public async Task DownloadSong()
    {
        TidalClient client = new TidalClient();
        var download = await client.DownloadSong(Environment.GetEnvironmentVariable("TIDAL_ACCESS_TOKEN"), "101982419");
        
        Assert.AreEqual(download.codec, "AAC");
    }
    
    
    [Test]
    public async Task DownloadSongData()
    {
        TidalClient client = new TidalClient();
        var download = await client.DownloadSongData(Environment.GetEnvironmentVariable("TIDAL_ACCESS_TOKEN"), "101982419");
        
        Assert.IsNotEmpty(download);
    }
}