using System.Text;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;

namespace DevBase.Requests.Abstract;

public abstract class BogusHttpHeaderBuilder<T> where T : BogusHttpHeaderBuilder<T>
{
    protected StringBuilder HeaderStringBuilder { get; private set; }
    protected bool AlreadyBuilded { get; set; }

    protected BogusHttpHeaderBuilder()
    {
        HeaderStringBuilder = new StringBuilder();
        AlreadyBuilded = false;
    }

    protected abstract Action BuildAction { get; }
    protected abstract Action BogusBuildAction { get; }

    public T Build()
    {
        if (this.AlreadyBuilded)
            throw new HttpHeaderException(HttpHeaderExceptionTypes.AlreadyBuilt);
        
        BuildAction.Invoke();
        
        this.AlreadyBuilded = true;
        return (T)this;
    }
    
    public T BuildBogus()
    {
        BogusBuildAction.Invoke();
        this.AlreadyBuilded = true;
        return (T)this;
    }
}