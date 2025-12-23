using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

public class TidalException : System.Exception
{
    public TidalException(EnumTidalExceptionType type) : base(GetMessage(type)) { }

    private static string GetMessage(EnumTidalExceptionType type)
    {
        switch (type)
        {
            case EnumTidalExceptionType.NotOk:
                return "Response response code mismatch";
            case EnumTidalExceptionType.AuthorizationPending:
                return "Authorization is still pending";
            case EnumTidalExceptionType.ParsingError:
                return "Failed to parse data";
        }

        throw new ErrorStatementException();
    }
}