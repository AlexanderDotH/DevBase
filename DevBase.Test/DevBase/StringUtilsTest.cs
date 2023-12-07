using DevBase.Utilities;
using Dumpify;

namespace DevBase.Test.DevBase;

public class StringUtilsTest
{
    [Test]
    public void SeparateTest()
    {
        string[] elements = new string[] { "Apple", "Banana", "Sausage" };

        string separated = StringUtils.Separate(elements, ", ");

        separated.DumpConsole();
        Assert.AreEqual(separated, "Apple, Banana, Sausage");
    }
    
    [Test]
    public void DeSeparateTest()
    {
        string separated = "Apple, Banana, Sausage";
        
        string[] elements = new string[] { "Apple", "Banana", "Sausage" };

        string[] deseperated = StringUtils.DeSeparate(separated, ", ");

        deseperated.DumpConsole();
        Assert.AreEqual(deseperated, elements);
    }
}