using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Data.Header.Authorization;
using DevBase.Test.Test;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.Authorization;

public class AuthorizationHeaderBuilderTest
{
    [Test]
    public void UseBasicAuthorizationTest()
    {
        int count = 1_000_000;
        
        string token;

        Stopwatch stopwatch = PenetrationTest.RunWithLast<string>(() =>
        {
            return new AuthorizationHeaderBuilder().UseBearerAuthorization("token").Build().Token.ToString();
        }, out token);

        Console.WriteLine($"Calculated bearer token {count} times");
        
        Assert.That(token, Is.EqualTo("token"));
        
        stopwatch.PrintTimeTable();
    }
}