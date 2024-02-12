using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Preparation.Header.Authorization;
using DevBase.Test.Test;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.Authorization;

public class AuthorizationHeaderBuilderTest
    
{
    [Test]
    public void UseBasicAuthorizationTest()
    {
        Stopwatch stopwatch = PenetrationTest.Run(() =>
            new AuthorizationHeaderBuilder().UseBearerAuthorization("token").Build()
        );
        
        stopwatch.PrintTimeTable();
    }

}