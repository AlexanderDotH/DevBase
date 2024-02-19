using System.Collections;
using System.Text;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;

namespace DevBase.Requests.Abstract;

public abstract class HttpFormBuilder<T, KeyK, KeyV> where T : HttpFormBuilder<T, KeyK, KeyV>
{
    public Memory<byte> Buffer { get; protected set; }
    public bool AlreadyBuilt { get; protected set; }

    protected List<KeyValuePair<KeyK, KeyV>> FormData { get; private set; }
    
    protected HttpFormBuilder()
    {
        FormData = new List<KeyValuePair<KeyK, KeyV>>();
        
        AlreadyBuilt = false;
    }

    protected void AddFormElement(KeyK key, KeyV value) => 
        FormData.Add(KeyValuePair.Create(key, value));

    protected void RemoveFormElement(int index) => FormData.RemoveAt(index);
    protected void RemoveFormElement(KeyK key) => this.FormData.RemoveAll((kv) => kv.Key.Equals(key));
    protected void RemoveFormElement(KeyV value) => this.FormData.RemoveAll((kv) => kv.Value.Equals(value));
    
    protected abstract Action BuildAction { get; }

    public T Build()
    {
        if (this.AlreadyBuilt)
            throw new HttpHeaderException(EnumHttpHeaderExceptionTypes.AlreadyBuilt);
        
        BuildAction.Invoke();
        
        this.AlreadyBuilt = true;
        return (T)this;
    }

    public override string ToString()
    {
        return Encoding.UTF8.GetString(Buffer.ToArray());
    }
}