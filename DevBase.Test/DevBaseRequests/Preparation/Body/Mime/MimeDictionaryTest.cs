using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Net.Data.Body.Mime;

namespace DevBase.Test.DevBaseRequests.Preparation.Body.Mime;

public class MimeDictionaryTest
{
    [Test]
    public void GetMimeTypeTest()
    {
        int amount = 1_000_000;

        MimeDictionary dictionary = new MimeDictionary();

        ReadOnlySpan<char> mimeType = string.Empty;
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        for (int i = 0; i < amount; i++)
            mimeType = dictionary.GetMimeTypeAsSpan(".png");
        
        stopwatch.Stop();
        
        Console.WriteLine($"Got the mime type {amount}times for .png({mimeType.ToString()})");
        Console.WriteLine(stopwatch.GetTimeTable());
        
        Assert.That(mimeType.ToString(), Is.EqualTo("image/png"));
    }

    [Test]
    public void GetResolveMimeType()
    {
        MimeDictionary dictionary = new MimeDictionary();

        ReadOnlyMemory<char> resolvedValue = "*/*".AsMemory();
        ReadOnlyMemory<char> defaultValue = "application/octet-stream".AsMemory();
        
        Assert.That(dictionary.GetMimeTypeAsMemory("*"), Is.EqualTo(resolvedValue));
        Assert.That(dictionary.GetMimeTypeAsMemory("*/*"), Is.EqualTo(resolvedValue));
        Assert.That(dictionary.GetMimeTypeAsMemory("application/joe-mama"), Is.EqualTo(defaultValue));
    }
}
