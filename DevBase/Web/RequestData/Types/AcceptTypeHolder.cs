using System.Text;
using DevBase.Enums;
using DevBase.Generic;

namespace DevBase.Web.RequestData.Types;

public class AcceptTypeHolder
{
    private GenericList<string> _acceptTypes;
    private GenericTupleList<EnumCharsetType, string> _charsetTypeDictionary;

    private ContentTypeHolder _contentTypeHolder;
    
    public AcceptTypeHolder(ContentTypeHolder contentTypeHolder)
    {
        this._contentTypeHolder = contentTypeHolder;
        
        this._acceptTypes = new GenericList<string>();

        this._charsetTypeDictionary = new GenericTupleList<EnumCharsetType, string>();
        this._charsetTypeDictionary.Add(EnumCharsetType.UTF8, "UTF-8");
        
        this.AddCharSet(EnumCharsetType.UTF8);
    }

    public void AddRaw(string charSet)
    {
        this._acceptTypes.Add(charSet);
    }

    public void AddCharSet(EnumCharsetType charsetType)
    {
        this._acceptTypes.Add(GetCharSet(charsetType));
    }

    private string GetCharSet(EnumCharsetType charsetType)
    {
        return this._charsetTypeDictionary.FindEntry(charsetType);
    }

    public bool Contains(EnumCharsetType charsetType)
    {
        return this._acceptTypes.Contains(GetCharSet(charsetType));
    }
    
    public string GetAccept()
    {
        StringBuilder stringBuilder = new StringBuilder();
        
        for (int i = 0; i < this._acceptTypes.Length; i++)
        {
            string accept = this._acceptTypes.Get(i);

            if (i != this._acceptTypes.Length - 1)
            {
                stringBuilder.Append(string.Format("{0};{1}, ", this._contentTypeHolder.ContentType, accept));
            }
            else
            {
                stringBuilder.Append(string.Format("{0};{1}", this._contentTypeHolder.ContentType, accept));
            }
        }

        return stringBuilder.ToString();
    }
}