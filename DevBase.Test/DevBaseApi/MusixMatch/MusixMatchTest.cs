using Dumpify;

namespace DevBase.Test.DevBaseApi.MusixMatch;

public class MusixMatchTest
{
    private string _username;
    private string _password;
    
    [SetUp]
    public void Setup()
    {
        this._username = "";
        this._password = "";
    }
    
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
            Assert.NotNull(auth.message.body.tokens.mxmprowebv10);
        }
    }
}