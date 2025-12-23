using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Abstract;

public abstract class GenericBuilder<T> where T : GenericBuilder<T>
{
    private bool AlreadyBuilt { get; set; }
    
    public bool Usable => this.AlreadyBuilt;

    protected GenericBuilder()
    {
        AlreadyBuilt = false;
    }

    protected abstract Action BuildAction { get; }

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
}