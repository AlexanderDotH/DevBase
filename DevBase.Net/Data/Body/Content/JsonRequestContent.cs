using System.Text;
using Newtonsoft.Json.Linq;

namespace DevBase.Net.Data.Body.Content;

public class JsonRequestContent : StringRequestContent
{
    public JsonRequestContent(Encoding encoding) : base(encoding) { }
    
    public override bool IsValid(ReadOnlySpan<byte> content)
    {
        if (!base.IsValid(content))
            return false;

        string stringContent = this.Encoding.GetString(content);

        try
        {
            // Use JToken.Parse to support both JSON objects {} and arrays []
            JToken.Parse(stringContent);
            return true;
        }
        catch
        {
            return false;
        }
    }
}