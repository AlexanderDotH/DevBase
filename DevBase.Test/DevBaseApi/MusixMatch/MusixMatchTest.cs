using MusixMatchClient = global::DevBase.Api.Apis.Musixmatch.MusixMatch;

namespace DevBase.Test.DevBaseApi.MusixMatch;

/// <summary>
/// Tests for the MusixMatch API client.
/// </summary>
public class MusixMatchTest
{
    private string _username;
    private string _password;
    
    /// <summary>
    /// Sets up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._username = Environment.GetEnvironmentVariable("DEVBASE_MUSIXMATCH_EMAIL") ?? "";
        this._password = Environment.GetEnvironmentVariable("DEVBASE_MUSIXMATCH_PASSWORD") ?? "";
    }
    
    /// <summary>
    /// Tests the login functionality.
    /// Requires username and password.
    /// </summary>
    [Test]
    public async Task LoginTest()
    {
        if (string.IsNullOrEmpty(this._username) && 
            string.IsNullOrEmpty(this._password))
        {
            Assert.Ignore("DEVBASE_MUSIXMATCH_EMAIL and DEVBASE_MUSIXMATCH_PASSWORD are empty; skipping live Musixmatch login.");
        }

        MusixMatchClient musixMatch = new MusixMatchClient("offline-test-token");
        var auth = await musixMatch.Login(this._username, this._password);

        Assert.That(auth?.Tokens?.WebDesktopAppV10, Is.Not.Null);
    }
}
