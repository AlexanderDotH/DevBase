using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Abstract;

/// <summary>
/// Abstract base class for generic builders.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class GenericBuilder<T> where T : GenericBuilder<T>
{
    private bool AlreadyBuilt { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the builder result is usable (already built).
    /// </summary>
    public bool Usable => this.AlreadyBuilt;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericBuilder{T}"/> class.
    /// </summary>
    protected GenericBuilder()
    {
        AlreadyBuilt = false;
    }

    /// <summary>
    /// Gets the action to perform when building.
    /// </summary>
    protected abstract Action BuildAction { get; }

    /// <summary>
    /// Builds the object.
    /// </summary>
    /// <returns>The builder instance.</returns>
    /// <exception cref="HttpHeaderException">Thrown if the object has already been built.</exception>
    public T Build()
    {
        if (!TryBuild())
            throw new HttpHeaderException(EnumHttpHeaderExceptionTypes.AlreadyBuilt);
        
        return (T)this;
    }
    
    /// <summary>
    /// Attempts to build the object.
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