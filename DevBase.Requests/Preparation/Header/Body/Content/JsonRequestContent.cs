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

        string stringContent = this.Encoding.GetString(content);
        JObject jObject = JObject.Parse(stringContent);
        
        return jObject.HasValues;
    }
}