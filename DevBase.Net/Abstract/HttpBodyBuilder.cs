using System.Text;
using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Abstract;

public abstract class HttpBodyBuilder<T> where T : HttpBodyBuilder<T>
{
    public Memory<byte> Buffer { get; protected set; }
    private bool AlreadyBuilt { get; set; }
    public bool Usable => this.AlreadyBuilt || !this.Buffer.IsEmpty;

    protected HttpBodyBuilder()
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

    public override string ToString()
    {
        return Encoding.UTF8.GetString(Buffer.ToArray());
    }
    
}