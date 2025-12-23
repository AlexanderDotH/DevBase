namespace DevBase.Exception;

public class ErrorStatementException : System.Exception
{
    public ErrorStatementException() : base("Exception state not present") { }
}