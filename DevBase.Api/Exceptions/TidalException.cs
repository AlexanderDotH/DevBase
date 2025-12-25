using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

/// <summary>
/// Exception thrown for Tidal API related errors.
/// </summary>
public class TidalException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TidalException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
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