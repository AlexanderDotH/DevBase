using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;

namespace DevBase.Requests.Abstract;



public abstract class HttpFieldBuilder<T> where T : HttpFieldBuilder<T>
{
    public KeyValuePair<string, string> FieldEntry { get; protected set; }

    private protected bool AlreadyBuilt { get; set; }

    protected HttpFieldBuilder()
    {
        AlreadyBuilt = false;
    }

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