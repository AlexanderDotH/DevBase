using System.Diagnostics;
using System.Text;
using DevBase.Extensions.Stopwatch;
using DevBase.Net.Data.Body.Content;
using DevBase.Test.Test;

namespace DevBase.Test.DevBaseRequests.Preparation.Body.Content;

public class JsonRequestContentTest
{
    private byte[] Value { get; set; }
    private const int Count = 1_000_000;
    
    [SetUp]
    public void Setup()
    {
        string jsonData = @"{""firstname"":""John"",""lastname"":""Doe"",""age"":35}";
        
        this.Value = Encoding.UTF8.GetBytes(jsonData);
    }

    [Test]
    public void IsValidTest()
    {
        JsonRequestContent jsonRequestContent = new JsonRequestContent(Encoding.UTF8);
        
        Stopwatch stopwatch = PenetrationTest.Run(() =>
        {
            Assert.That(jsonRequestContent.IsValid(this.Value), Is.True);
        }, Count);
        
        Console.WriteLine($"Validated json {Convert.ToHexString(this.Value)} {Count}times");
        
        stopwatch.PrintTimeTable();
    }
}
