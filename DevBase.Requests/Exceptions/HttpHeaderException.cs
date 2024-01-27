using System.Dynamic;
using DevBase.Requests.Enums;

namespace DevBase.Requests.Exceptions;

public class HttpHeaderException : System.Exception
{
    public HttpHeaderException(HttpHeaderExceptionTypes exceptionTypes) : base(GetMessage(exceptionTypes)) { }

    private static string GetMessage(HttpHeaderExceptionTypes exceptionTypes)
    {
        switch (exceptionTypes)
        {
            case HttpHeaderExceptionTypes.AlreadyBuilt:
            {
                return "The HTTP builder has already been created.";
            }
        }

        return string.Empty;
    }
}