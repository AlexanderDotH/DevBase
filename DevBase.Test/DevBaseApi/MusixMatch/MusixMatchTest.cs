using Dumpify;

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
        this._username = "";
        this._password = "";
    }
    
    /// <summary>
    /// Tests the login functionality.
    /// Requires username and password.
    /// </summary>
    [Test]
    public async Task LoginTest()
    {
        Api.Apis.Musixmatch.MusixMatch musixMatch = new Api.Apis.Musixmatch.MusixMatch();

        if (string.IsNullOrEmpty(this._username) && 
            string.IsNullOrEmpty(this._password))
        {
            Console.WriteLine("The username or password is empty and that is okay");
        }
        else
        {
            var auth = await musixMatch.Login(this._username, this._password);
            Assert.That(auth.message.body.tokens.mxmprowebv10, Is.Not.Null);
        }
    }
}
