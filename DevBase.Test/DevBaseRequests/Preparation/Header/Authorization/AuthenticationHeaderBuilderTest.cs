using System.Diagnostics;
using System.Text;
using DevBase.Extensions;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Data.Header.Authentication;
using DevBase.Test.Test;
using Dumpify;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.Authentication;

public class AuthenticationHeaderBuilderTest
{
    [Test]
    public void UseBasicAuthenticationTest()
    {
        int count = 1_000_000;
        
        string authContent;

        Stopwatch stopwatch = PenetrationTest.RunWithLast<string>(
            
            () => 
                new AuthenticationHeaderBuilder()
            .UseBasicAuthentication("username", "password")
            .Build()
            .AuthenticationValue.ToString()
            
            , out authContent, count);

        Assert.That(authContent[..5], Is.EquivalentTo("Basic"));
        
        byte[] buffer = Convert.FromBase64String(authContent[6..]);

        byte[] eUsername = buffer.Take(..8).ToArray();
        byte[] ePassword = buffer.TakeLast(8).ToArray();
        
        string dUsername = Encoding.UTF8.GetString(eUsername);
        string dPassword= Encoding.UTF8.GetString(ePassword);

        Assert.That(dUsername, Is.EquivalentTo("username"));
        Assert.That(dPassword, Is.EquivalentTo("password"));
        
        Console.WriteLine($"Calculated basic token {count} times");
        Console.WriteLine($"\n{"-".Repeat(10)}");
        
        stopwatch.PrintTimeTable();
        
        Console.WriteLine($"\n{"-".Repeat(10)}");
        Console.WriteLine("Content");

        dUsername.DumpConsole();
        dPassword.DumpConsole();
    }
    
    [Test]
    public void UseBearerAuthenticationTest()
    {
        int count = 1_000_000;
        
        string token;

        Stopwatch stopwatch = PenetrationTest.RunWithLast<string>(
            
            () => new AuthenticationHeaderBuilder()
                .UseBearerAuthentication("token")
                .Build()
                .AuthenticationValue.ToString(),
            
            out token, count);

        Console.WriteLine($"Calculated bearer token {count} times");
        
        Assert.That(token, Is.EqualTo("Bearer token"));
        
        stopwatch.PrintTimeTable();
    }
}
