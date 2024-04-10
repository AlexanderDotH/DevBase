using System.Text;
using Newtonsoft.Json.Linq;

namespace DevBase.Requests.Preparation.Header.Body.Content;

public class JsonRequestContent : StringRequestContent
{
    public JsonRequestContent(Encoding encoding) : base(encoding) { }
    
    public override bool IsValid(ReadOnlySpan<byte> content)
    {
        if (!base.IsValid(content))
            return false;

        JObject jObject = JObject.FromObject(content.ToArray());
        
        return jObject.HasValues;
    }
}