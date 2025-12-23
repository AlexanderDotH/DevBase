namespace DevBase.Net.Exceptions;

public class BuilderException : System.Exception
{
    public BuilderException() : base("Failed to use the builder, because the builder is null") { }
}