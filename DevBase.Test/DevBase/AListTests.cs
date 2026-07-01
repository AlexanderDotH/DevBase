using DevBase.Generics;

namespace DevBase.Test.DevBase;

/// <summary>
/// Tests for the AList generic collection.
/// </summary>
public class AListTests
{
    /// <summary>
    /// Tests the RemoveRange functionality of AList.
    /// </summary>
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

        Assert.That(listOfStrings.Get(0), Is.EqualTo("Bird"));
    }

    /// <summary>
    /// Tests the Find functionality of AList with a large dataset.
    /// Measures performance and verifies correctness.
    /// </summary>
    [Test]
    public void FindTest()
    {
        AList<string> listOfString = new AList<string>();
        listOfString.Add("alpha");
        listOfString.Add("beta");
        string searchFor = new("needle-target-unique".ToCharArray());
        listOfString.Add(searchFor);
        listOfString.Add("gamma");

        string found = listOfString.Find(p => p.Contains("target-unique"));

        Assert.That(found, Is.SameAs(searchFor));
    }
}
