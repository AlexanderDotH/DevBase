using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Abstract;

/// <summary>
/// Abstract base class for HTTP field builders (key-value pair).
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class HttpFieldBuilder<T> where T : HttpFieldBuilder<T>
{
    /// <summary>
    /// Gets or sets the field entry (Key-Value pair).
    /// </summary>
    protected KeyValuePair<string, string> FieldEntry { get; set; }

    private bool AlreadyBuilt { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the builder result is usable (built or has valid entry).
    /// </summary>
    public bool Usable => this.AlreadyBuilt || 
                          !string.IsNullOrEmpty(this.FieldEntry.Key) && !string.IsNullOrEmpty(this.FieldEntry.Value);

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpFieldBuilder{T}"/> class.
    /// </summary>
    protected HttpFieldBuilder()
    {
        AlreadyBuilt = false;

        FieldEntry = new KeyValuePair<string, string>();
    }

    /// <summary>
    /// Gets the action to perform when building the field.
    /// </summary>
    protected abstract Action BuildAction { get; }

    /// <summary>
    /// Builds the HTTP field.
    /// </summary>
    /// <returns>The builder instance.</returns>
    /// <exception cref="HttpHeaderException">Thrown if the field has already been built.</exception>
    public T Build()
    {
        if (!TryBuild())
            throw new HttpHeaderException(EnumHttpHeaderExceptionTypes.AlreadyBuilt);
        
        return (T)this;
    }
    
    /// <summary>
    /// Attempts to build the HTTP field.
    /// </summary>
    /// <returns>True if the build was successful; otherwise, false (if already built).</returns>
    public bool TryBuild()
    {
        if (this.AlreadyBuilt)
            return false;
        
        BuildAction.Invoke();
        
        this.AlreadyBuilt = true;
        return true;
    }
}