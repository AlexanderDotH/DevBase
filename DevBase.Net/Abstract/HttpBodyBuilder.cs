using System.Text;
using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Abstract;

/// <summary>
/// Abstract base class for HTTP body builders.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class HttpBodyBuilder<T> where T : HttpBodyBuilder<T>
{
    /// <summary>
    /// Gets the buffer containing the body content.
    /// </summary>
    public Memory<byte> Buffer { get; protected set; }
    
    private bool AlreadyBuilt { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the builder result is usable (built or has content).
    /// </summary>
    public bool Usable => this.AlreadyBuilt || !this.Buffer.IsEmpty;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpBodyBuilder{T}"/> class.
    /// </summary>
    protected HttpBodyBuilder()
    {
        AlreadyBuilt = false;
    }
    
    /// <summary>
    /// Gets the action to perform when building the body.
    /// </summary>
    protected abstract Action BuildAction { get; }

    /// <summary>
    /// Builds the HTTP body.
    /// </summary>
    /// <returns>The builder instance.</returns>
    /// <exception cref="HttpHeaderException">Thrown if the body has already been built.</exception>
    public T Build()
    {
        if (this.AlreadyBuilt)
            throw new HttpHeaderException(EnumHttpHeaderExceptionTypes.AlreadyBuilt);
        
        BuildAction.Invoke();
        
        this.AlreadyBuilt = true;
        return (T)this;
    }

    /// <summary>
    /// Returns the string representation of the body buffer using UTF-8 encoding.
    /// </summary>
    /// <returns>The body as a string.</returns>
    public override string ToString()
    {
        return Encoding.UTF8.GetString(Buffer.ToArray());
    }
    
}