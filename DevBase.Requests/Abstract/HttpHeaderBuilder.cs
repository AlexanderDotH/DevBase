using System.Text;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;

namespace DevBase.Requests.Abstract;

public abstract class HttpHeaderBuilder<T> where T : HttpHeaderBuilder<T>
{
    protected StringBuilder HeaderStringBuilder { get; private set; }
    private bool AlreadyBuilt { get; set; }

    protected HttpHeaderBuilder()
    {
        HeaderStringBuilder = new StringBuilder();
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