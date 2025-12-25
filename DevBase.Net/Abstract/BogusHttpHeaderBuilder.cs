using System.Text;
using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Abstract;

/// <summary>
/// Abstract base class for HTTP header builders that support "bogus" (random/fake) generation.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class BogusHttpHeaderBuilder<T> where T : BogusHttpHeaderBuilder<T>
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
    /// Initializes a new instance of the <see cref="BogusHttpHeaderBuilder{T}"/> class.
    /// </summary>
    protected BogusHttpHeaderBuilder()
    {
        HeaderStringBuilder = new StringBuilder();
        AlreadyBuilt = false;
    }

    /// <summary>
    /// Gets the action to perform when building the header normally.
    /// </summary>
    protected abstract Action BuildAction { get; }
    
    /// <summary>
    /// Gets the action to perform when building a bogus header.
    /// </summary>
    protected abstract Action BogusBuildAction { get; }

    /// <summary>
    /// Builds the header using the standard build action.
    /// </summary>
    /// <returns>The builder instance.</returns>
    /// <exception cref="HttpHeaderException">Thrown if the header has already been built.</exception>
    public T Build()
    {
        if (!TryBuild())
            throw new HttpHeaderException(EnumHttpHeaderExceptionTypes.AlreadyBuilt);
        
        return (T)this;
    }

    /// <summary>
    /// Attempts to build the header using the standard build action.
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
    
    /// <summary>
    /// Builds the header using the bogus build action.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public T BuildBogus()
    {
        BogusBuildAction.Invoke();
        this.AlreadyBuilt = true;
        return (T)this;
    }
}