namespace DevBase.Format.Exceptions;

public class ParsingException : System.Exception
{
    public ParsingException(string message) : base(message) {}
    
    public ParsingException(string message, System.Exception innerException) : base(message, innerException) {}
}