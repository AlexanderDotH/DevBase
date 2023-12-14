using System.Diagnostics;
using DevBase.Generics;
using DevBase.Utilities;
using Dumpify;

namespace DevBase.Test.DevBase;

public class AListTests
{
    [Test]
    public void RemoveRangeTest()
    {
        AList<string> listOfStrings = new AList<string>();
        listOfStrings.Add("Cat");
        listOfStrings.Add("Mouse");
        listOfStrings.Add("Dog");
        listOfStrings.Add("Bird");
        listOfStrings.Add("Bat");
        listOfStrings.Add("Joe");

        listOfStrings.RemoveRange(0, 2);

        listOfStrings.DumpConsole();
        
        Assert.IsTrue(listOfStrings.Get(0).Equals("Bird"));
    }

    [Test]
    public void FindTest()
    {
        AList<string> listOfString = new AList<string>();

        for (int i = 0; i < 40000; i++)
        {
            listOfString.Add(StringUtils.RandomString(900));
        }

        Stopwatch sw = new Stopwatch();
        sw.Start();
        
        string searchFor = listOfString.Get((int)Math.Floor(listOfString.Length / 2.0));

        string found = listOfString.Find(p => p.Contains(searchFor.Substring(0, 5)));

        sw.Stop();
        
        Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms or {sw.ElapsedTicks}ticks to find the string");
        
        found.DumpConsole();
        
        Assert.AreSame(searchFor, found);
    }
}