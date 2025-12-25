using System.Text;
using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Abstract;

/// <summary>
/// Abstract base class for HTTP header builders.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class HttpHeaderBuilder<T> where T : HttpHeaderBuilder<T>
{
    /// <summary>
    /// Gets the StringBuilder used to construct the header.
    /// </summary>
    protected StringBuilder HeaderStringBuilder { get; private set; }
    
    private bool AlreadyBuilt { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the builder result is usable (built or has content).
    /// </summary>
    public bool Usable => this.AlreadyBuilt || this.HeaderStringBuilder.Length > 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpHeaderBuilder{T}"/> class.
    /// </summary>
    protected HttpHeaderBuilder()
    {
        HeaderStringBuilder = new StringBuilder();
        AlreadyBuilt = false;
    }

    /// <summary>
    /// Gets the action to perform when building the header.
    /// </summary>
    protected abstract Action BuildAction { get; }

    /// <summary>
    /// Builds the HTTP header.
    /// </summary>
    /// <returns>The builder instance.</returns>
    /// <exception cref="HttpHeaderException">Thrown if the header has already been built.</exception>
    public T Build()
    {
        if (this.AlreadyBuilt)
            throw new HttpHeaderException(EnumHttpHeaderExceptionTypes.AlreadyBuilt);
        
        BuildAction.Invoke();
        
        this.AlreadyBuilt = true;
        return (T)this;
    }
}