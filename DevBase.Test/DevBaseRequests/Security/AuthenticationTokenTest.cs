namespace DevBase.Test.DevBaseRequests.Security;

public class AuthenticationTokenTest
{
    private string JwtToken { get; set; }
    private string JwtSecret { get; set; }

    [SetUp]
    public void SetUp()
    {
        this.JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjQyMCJ9.eyJpc3MiOiJBbGV4YW5kZXJEb3RIIiwiaWF0IjoxNzEzOTYwMDAwLCJleHAiOjE5MDMyNjI0MDAsInNjb3BlIjoidW5pdC10ZXN0In0.HLtXQ7fbXS7RF6pJy-LkjZgfWBU_BE_85F8-A1o1efA";
        this.JwtSecret = "i-like-jwt";
    }
}