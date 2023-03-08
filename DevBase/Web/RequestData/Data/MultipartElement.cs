using System.IO.Pipes;

namespace DevBase.Web.RequestData.Data;

public class MultipartElement
{
    private string _key;
    private object _data;

    public MultipartElement(string key, object data)
    {
        _key = key;
        _data = data;
    }

    public string Key
    {
        get => _key;
        set => _key = value;
    }

    public object Data
    {
        get => _data;
        set => _data = value;
    }
}