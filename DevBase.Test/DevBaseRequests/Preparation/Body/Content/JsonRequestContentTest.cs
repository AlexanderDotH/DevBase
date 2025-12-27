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
    
    [Test]
    public void IsValidArrayTest()
    {
        JsonRequestContent jsonRequestContent = new JsonRequestContent(Encoding.UTF8);
        
        // Test simple array
        string jsonArray = @"[1, 2, 3]";
        byte[] arrayBytes = Encoding.UTF8.GetBytes(jsonArray);
        Assert.That(jsonRequestContent.IsValid(arrayBytes), Is.True);
        
        // Test array of objects
        string jsonArrayOfObjects = @"[{""id"":1,""name"":""Item1""},{""id"":2,""name"":""Item2""}]";
        byte[] arrayOfObjectsBytes = Encoding.UTF8.GetBytes(jsonArrayOfObjects);
        Assert.That(jsonRequestContent.IsValid(arrayOfObjectsBytes), Is.True);
        
        // Test nested arrays
        string nestedArray = @"[[1,2],[3,4]]";
        byte[] nestedArrayBytes = Encoding.UTF8.GetBytes(nestedArray);
        Assert.That(jsonRequestContent.IsValid(nestedArrayBytes), Is.True);
        
        // Test empty array
        string emptyArray = @"[]";
        byte[] emptyArrayBytes = Encoding.UTF8.GetBytes(emptyArray);
        Assert.That(jsonRequestContent.IsValid(emptyArrayBytes), Is.True);
        
        Console.WriteLine("All JSON array validation tests passed");
    }
    
    [Test]
    public void IsValidArrayPerformanceTest()
    {
        JsonRequestContent jsonRequestContent = new JsonRequestContent(Encoding.UTF8);
        string jsonArray = @"[{""position"":0,""questionId"":""abc123"",""textAnswer"":{""response"":""Hello""}}]";
        byte[] arrayBytes = Encoding.UTF8.GetBytes(jsonArray);
        
        Stopwatch stopwatch = PenetrationTest.Run(() =>
        {
            Assert.That(jsonRequestContent.IsValid(arrayBytes), Is.True);
        }, Count);
        
        Console.WriteLine($"Validated json array {Count} times");
        stopwatch.PrintTimeTable();
    }
    
    [Test]
    public void IsInvalidJsonTest()
    {
        JsonRequestContent jsonRequestContent = new JsonRequestContent(Encoding.UTF8);
        
        // Test invalid JSON
        string invalidJson = @"not valid json";
        byte[] invalidBytes = Encoding.UTF8.GetBytes(invalidJson);
        Assert.That(jsonRequestContent.IsValid(invalidBytes), Is.False);
        
        // Test malformed array
        string malformedArray = @"[1, 2, 3";
        byte[] malformedBytes = Encoding.UTF8.GetBytes(malformedArray);
        Assert.That(jsonRequestContent.IsValid(malformedBytes), Is.False);
        
        Console.WriteLine("Invalid JSON detection tests passed");
    }
}
