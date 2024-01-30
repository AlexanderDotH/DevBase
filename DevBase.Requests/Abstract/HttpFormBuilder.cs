using System.Collections;
using System.Text;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;

namespace DevBase.Requests.Abstract;

public abstract class HttpFormBuilder<T, KeyK, KeyV> where T : HttpFormBuilder<T, KeyK, KeyV>
{
    protected StringBuilder FormStringBuilder { get; private set; }
    public bool AlreadyBuilt { get; protected set; }

    private List<KeyValuePair<KeyK, KeyV>> FormData { get; }
    
    protected HttpFormBuilder()
    {
        FormStringBuilder = new StringBuilder();
        FormData = new List<KeyValuePair<KeyK, KeyV>>();
        
        AlreadyBuilt = false;
    }

    protected void AddFormElement(KeyK key, KeyV value) => 
        FormData.Add(KeyValuePair.Create(key, value));
    
    protected abstract Action BuildAction { get; }

    public T Build()
    {
        if (this.AlreadyBuilt)
            throw new HttpHeaderException(EnumHttpHeaderExceptionTypes.AlreadyBuilt);
        
        BuildAction.Invoke();
        
        this.AlreadyBuilt = true;
        return (T)this;
    }
}