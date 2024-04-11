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

        string jsonContent = this.Encoding.GetString(content.ToArray());
        
        JObject jObject = JObject.Parse(jsonContent);
        
        return jObject.HasValues;
    }
}