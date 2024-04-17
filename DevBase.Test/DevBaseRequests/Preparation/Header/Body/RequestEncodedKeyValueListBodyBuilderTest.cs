using System.Diagnostics;
using System.Text;
using Bogus.DataSets;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Preparation.Header.Body;
using DevBase.Test.Test;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.Body;

public class RequestEncodedKeyValueListBodyBuilderTest
{
    private const int _count = 1_000_000;
    private string[] _keys;
    private string[] _values;

    [SetUp]
    public void Setup()
    {
        Lorem lorem = new Lorem("en");

        _keys = Enumerable.Range(0, _count).Select(s => lorem.Word()).ToArray();
        _values = Enumerable.Range(0, _count).Select(s => lorem.Word()).ToArray();        
    }

    [Test]
    public void BuildTest()
    {
        Stopwatch sw = new Stopwatch();
        
        sw.Start();

        RequestEncodedKeyValueListBodyBuilder builder = new RequestEncodedKeyValueListBodyBuilder();

        for (int i = 0; i < _count; i++)
        {
            builder = new RequestEncodedKeyValueListBodyBuilder();
            
            for (int j = 0; j < 100; j++)
            {
                builder.AddText(_keys[i], _values[i]);
            }

            builder.Build();
        }
        
        sw.Stop();

        Console.WriteLine($"Generated url encoded header content {_count} * 100 times");
        Console.WriteLine(sw.GetTimeTable());
        Console.WriteLine($"The Content: {Encoding.UTF8.GetString(builder.Buffer.ToArray())}"); 
    }
}