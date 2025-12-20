using DevBase.Requests.Security.Token;
using Dumpify;

namespace DevBase.Test.DevBaseRequests.Security;

public class AuthenticationTokenTest
{
    private string JwtToken { get; set; }
    private string JwtSecret { get; set; }

    [SetUp]
    public void SetUp()
    {
        this.JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwic3ViIjoiVGVzdCBKV1QiLCJhdWQiOiJUZXN0IEF1ZGllbmNlIiwiZXhwIjoxOTAzMjYyNDAwLCJuYmYiOjY5LCJpYXQiOjE3MTM5NjAwMDAsImp0aSI6ImNlZDllZjFhLWJiZjctNDkzNC1hOThkLWUxMzk4ZDYxYTcyMiIsInNjb3BlIjoidW5pdC10ZXN0IiwiY3VzdG9tLWNsYWltIjoiVGhlIGNha2UgaXMgYSBsaWUifQ.D-RI3C3U01EDJsYO15PRUNnR8KauhktoSZ1fGPX6qes";
        this.JwtSecret = "i-like-jwt";
    }

    [Test]
    public void FromStringTest()
    {
        AuthenticationToken? authenticationToken = AuthenticationToken.FromString(this.JwtToken!);
        
        Assert.That(authenticationToken?.Payload.Issuer, Is.EquivalentTo("AlexanderDotH"));
        Assert.That(authenticationToken?.Payload.Subject, Is.EquivalentTo("Test JWT"));
        Assert.That(authenticationToken?.Payload.Audience, Is.EquivalentTo("Test Audience"));
        
        Assert.That(authenticationToken?.Payload.IssuedAt.Year, Is.EqualTo(2024));
        Assert.That(authenticationToken?.Payload.IssuedAt.Month, Is.EqualTo(4));
        Assert.That(authenticationToken?.Payload.IssuedAt.Day, Is.EqualTo(24));
        
        Assert.That(authenticationToken?.Payload.NotBefore, Is.EqualTo(69));
        
        Assert.That(authenticationToken?.Payload.ExpiresAt.Year, Is.EqualTo(2030));
        Assert.That(authenticationToken?.Payload.ExpiresAt.Month, Is.EqualTo(4));
        Assert.That(authenticationToken?.Payload.ExpiresAt.Day, Is.EqualTo(24));
        
        Assert.That(authenticationToken?.Payload.Id, Is.EquivalentTo("ced9ef1a-bbf7-4934-a98d-e1398d61a722"));
        
        Assert.That(authenticationToken?.Payload.Claims?["scope"], Is.EquivalentTo("unit-test"));
        Assert.That(authenticationToken?.Payload.Claims?["custom-claim"], Is.EquivalentTo("The cake is a lie"));

        authenticationToken.DumpConsole();
    }
}
