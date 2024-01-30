using System.Text;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;

namespace DevBase.Requests.Abstract;

public abstract class BogusHttpHeaderBuilder<T> where T : BogusHttpHeaderBuilder<T>
{
    protected StringBuilder HeaderStringBuilder { get; private set; }
    public bool AlreadyBuilt { get; protected set; }

    protected BogusHttpHeaderBuilder()
    {
        HeaderStringBuilder = new StringBuilder();
        AlreadyBuilt = false;
    }

    protected abstract Action BuildAction { get; }
    protected abstract Action BogusBuildAction { get; }

    public T Build()
    {
        if (this.AlreadyBuilt)
            throw new HttpHeaderException(EnumHttpHeaderExceptionTypes.AlreadyBuilt);
        
        BuildAction.Invoke();
        
        this.AlreadyBuilt = true;
        return (T)this;
    }
    
    public T BuildBogus()
    {
        BogusBuildAction.Invoke();
        this.AlreadyBuilt = true;
        return (T)this;
    }
}