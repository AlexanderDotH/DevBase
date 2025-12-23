using System.Text;
using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Abstract;

public abstract class HttpHeaderBuilder<T> where T : HttpHeaderBuilder<T>
{
    protected StringBuilder HeaderStringBuilder { get; private set; }
    private bool AlreadyBuilt { get; set; }
    public bool Usable => this.AlreadyBuilt || this.HeaderStringBuilder.Length > 0;

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