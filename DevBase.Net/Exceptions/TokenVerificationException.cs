using DevBase.Net.Enums;

namespace DevBase.Net.Exceptions;

public class TokenVerificationException : System.Exception
{
    public TokenVerificationException(EnumTokenVerificationExceptionType exceptionTypes) : base(GetMessage(exceptionTypes)) { }

    private static string GetMessage(EnumTokenVerificationExceptionType exceptionTypes)
    {
        switch (exceptionTypes)
        {
            
            case EnumTokenVerificationExceptionType.MissingField:
                return "One mandatory field is missing";
            case EnumTokenVerificationExceptionType.InvalidLength:
                return "Provided algorithm has an invalid text length. \nRFC7519: https://datatracker.ietf.org/doc/html/rfc7519#section-8";
            case EnumTokenVerificationExceptionType.AlgorithmNotAvailable:
                return "Requested algorithm is not available";
        }

        return string.Empty;
    }
}