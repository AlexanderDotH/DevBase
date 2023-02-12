using System.Diagnostics;
using System.Text;
using DevBase.Enums;
using DevBase.Generics;

namespace DevBase.Web.RequestData.Types;

public class ContentTypeHolder
{
    private string _contentType;
    private ATupleList<EnumContentType, string> _contentTypeDictionary;
    
    public ContentTypeHolder()
    {
        this._contentType = "text/html";

        this._contentTypeDictionary = new ATupleList<EnumContentType, string>();
        this._contentTypeDictionary.Add(EnumContentType.APPLICATION_JSON, "application/json");
        this._contentTypeDictionary.Add(EnumContentType.TEXT_PLAIN, "text/plain");
        this._contentTypeDictionary.Add(EnumContentType.TEXT_HTML, "text/html");
        this._contentTypeDictionary.Add(EnumContentType.APPLICATION_FORM_URLENCODED, "application/x-www-form-urlencoded");
    }

    public ContentTypeHolder Set(EnumContentType contentType)
    {
        this._contentType = GetContentType(contentType);
        return this;
    }

    public string ContentType
    {
        get => _contentType;
        set => _contentType = value;
    }

    public string GetContentType(EnumContentType contentType)
    {
        return this._contentTypeDictionary.FindEntry(contentType);
    }

    public ATupleList<EnumContentType, string> ContentTypeDictionary
    {
        get => _contentTypeDictionary;
    }
}