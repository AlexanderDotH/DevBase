using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Abstract;

public abstract class HttpFieldBuilder<T> where T : HttpFieldBuilder<T>
{
    protected KeyValuePair<string, string> FieldEntry { get; set; }

    private bool AlreadyBuilt { get; set; }
    
    public bool Usable => this.AlreadyBuilt || 
                          !string.IsNullOrEmpty(this.FieldEntry.Key) && !string.IsNullOrEmpty(this.FieldEntry.Value);

    protected HttpFieldBuilder()
    {
        AlreadyBuilt = false;

        FieldEntry = new KeyValuePair<string, string>();
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