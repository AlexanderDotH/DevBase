using System.Diagnostics;
using Avalonia.Controls.Documents;
using DevBase.Requests.Preparation.Parameters;
using Dumpify;

namespace DevBase.Test.DevBaseRequests.Builder;

public class ParameterBuilderTest
{

    [Test]
    public void ParameterBuilderAppendTest()
    {
        var lorem = new Bogus.DataSets.Lorem("en");

        int count = 1_000_000;
        
        string[] keys = Enumerable.Range(0, count).Select(s => lorem.Word()).ToArray();
        string[] values = Enumerable.Range(0, count).Select(s => lorem.Word()).ToArray();

        Stopwatch sw = new Stopwatch();
        
        sw.Start();
        
        ParameterBuilder builder = new ParameterBuilder();

        for (int i = 0; i < count; i++)
            builder.AddParameter(keys[i], values[i]);

        builder.Build();
        
        sw.Stop();
        
        Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms or {sw.ElapsedTicks}ts to append all elements to the parameter list");
    }
}