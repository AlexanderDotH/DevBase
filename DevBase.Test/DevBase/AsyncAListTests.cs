using System.Diagnostics;
using DevBase.Generics;
using DevBase.Utilities;
using Dumpify;

namespace DevBase.Test.DevBase;

public class AsyncAListTests
{
    [Test]
    public async Task FindTest()
    {
        AsyncAList<string> asyncAList = new AsyncAList<string>(500);

        for (int i = 0; i < 400000; i++)
        {
            asyncAList.Add(StringUtils.RandomString(9));
        }

        Stopwatch sw = new Stopwatch();
        sw.Start();
        
        string searchFor = asyncAList.Get((int)Math.Floor(asyncAList.Length / 2.0));
        
        string found = await asyncAList.Find(p => p.Contains(searchFor.Substring(0, 5)), 100);

        sw.Stop();
        
        Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms or {sw.ElapsedTicks}ticks to find the string");
        
        found.DumpConsole();
        
        Assert.AreSame(searchFor, found);
    }
}