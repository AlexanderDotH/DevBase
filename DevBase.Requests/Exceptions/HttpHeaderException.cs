using System.Dynamic;
using DevBase.Requests.Enums;

namespace DevBase.Requests.Exceptions;

public class HttpHeaderException : System.Exception
{
    public HttpHeaderException(EnumHttpHeaderExceptionTypes exceptionTypes) : base(GetMessage(exceptionTypes)) { }

    private static string GetMessage(EnumHttpHeaderExceptionTypes exceptionTypes)
    {
        switch (exceptionTypes)
        {
            case EnumHttpHeaderExceptionTypes.AlreadyBuilt:
                return "The HTTP builder has already been created.";
            
            case EnumHttpHeaderExceptionTypes.Incomplete:
                return "The HTTP builder is incomplete, lacking essential elements or entries required for its functionality.";
        }

        return string.Empty;
    }
}