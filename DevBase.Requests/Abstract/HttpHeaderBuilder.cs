using System.Text;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;

namespace DevBase.Requests.Abstract;

public abstract class HttpHeaderBuilder<T> where T : HttpHeaderBuilder<T>
{
    protected StringBuilder HeaderStringBuilder { get; private set; }
    protected bool AlreadyBuilded { get; set; }

    protected HttpHeaderBuilder()
    {
        HeaderStringBuilder = new StringBuilder();
        AlreadyBuilded = false;
    }

    protected abstract Action BuildAction { get; }

    public T Build()
    {
        if (this.AlreadyBuilded)
            throw new HttpHeaderException(HttpHeaderExceptionTypes.AlreadyBuilt);
        
        BuildAction.Invoke();
        
        this.AlreadyBuilded = true;
        return (T)this;
    }
}