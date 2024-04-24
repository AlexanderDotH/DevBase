using System.Text;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;

namespace DevBase.Requests.Abstract;

public abstract class BogusHttpHeaderBuilder<T> where T : BogusHttpHeaderBuilder<T>
{
    protected StringBuilder HeaderStringBuilder { get; private set; }
    private bool AlreadyBuilt { get; set; }
    public bool Usable => this.AlreadyBuilt || this.HeaderStringBuilder.Length > 0;

    protected BogusHttpHeaderBuilder()
    {
        HeaderStringBuilder = new StringBuilder();
        AlreadyBuilt = false;
    }

    protected abstract Action BuildAction { get; }
    protected abstract Action BogusBuildAction { get; }

    public T Build()
    {
        if (!TryBuild())
            throw new HttpHeaderException(EnumHttpHeaderExceptionTypes.AlreadyBuilt);
        
        return (T)this;
    }

    public bool TryBuild()
    {
        if (this.AlreadyBuilt)
            return false;
        
        BuildAction.Invoke();
        
        this.AlreadyBuilt = true;
        return true;
    }
    
    public T BuildBogus()
    {
        BogusBuildAction.Invoke();
        this.AlreadyBuilt = true;
        return (T)this;
    }
}