using DevBase.Api.Objects.Token;
using Dumpify;

namespace DevBase.Test.DevBaseApi.Authentication;

public class AuthenticationTokenTests
{
    [Test]
    public void FromStringTest()
    {
        string token =
            "eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IldlYlBsYXlLaWQifQ.eyJpc3MiOiJBTVBXZWJQbGF5IiwiaWF0IjoxNzAyNTAyMjM0LCJleHAiOjE3MDk3NTk4MzQsInJvb3RfaHR0cHNfb3JpZ2luIjpbImFwcGxlLmNvbSJdfQ.zzeMLmez71PLinP9GozYSQnF7NYyCiXHB9tKL3-cyu3LzyeRnYz0ejLj4CrNJs0dlNkFg9_mwKmMLueUAR-KRg";
        
        AuthenticationToken authenticationToken = AuthenticationToken.FromString(token);

        authenticationToken.DumpConsole();
        
        Assert.AreEqual("JWT", authenticationToken.Header.Type);
    }
}