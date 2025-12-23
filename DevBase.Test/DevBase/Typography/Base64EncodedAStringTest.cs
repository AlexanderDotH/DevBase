using DevBase.Exception;
using DevBase.Typography.Encoded;
using Dumpify;

namespace DevBase.Test.DevBase.Typography;

public class Base64EncodedAStringTest
{
    
    [Test]
    public void DecodeTest()
    {
        Base64EncodedAString encodedAString = new Base64EncodedAString("HLtXQ7fbXS7RF6pJy-LkjZgfWBU_BE_85F8-A1o1efA=").UrlDecoded();
        Assert.That(encodedAString.Value, Is.EqualTo("HLtXQ7fbXS7RF6pJy+LkjZgfWBU/BE/85F8+A1o1efA="));
        encodedAString.DumpConsole();
    }
    
    [Test]
    public void EncodeTest()
    {
        Base64EncodedAString encodedAString = new Base64EncodedAString("HLtXQ7fbXS7RF6pJy+LkjZgfWBU/BE/85F8+A1o1efA=").UrlEncoded();
        Assert.That(encodedAString.Value, Is.EqualTo("HLtXQ7fbXS7RF6pJy-LkjZgfWBU_BE_85F8-A1o1efA="));
        encodedAString.DumpConsole();
    }
    
    [Test]
    public void InvalidStringTest()
    {
        Assert.Throws<EncodingException>(() =>
        {
            Base64EncodedAString encodedAString = new Base64EncodedAString("not valid string");
        });
        
        Console.WriteLine("EncodingException was thrown");
    }
}
