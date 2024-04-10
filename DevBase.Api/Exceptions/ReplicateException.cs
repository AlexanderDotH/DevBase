using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

public class ReplicateException : System.Exception
{
    public ReplicateException(EnumReplicateExceptionType type) : base(GetMessage(type)) { }

    private static string GetMessage(EnumReplicateExceptionType type)
    {
        switch (type)
        {
            case EnumReplicateExceptionType.TokenNotProvided:
                return "The api token is missing";
        }

        throw new ErrorStatementException();
    }
}