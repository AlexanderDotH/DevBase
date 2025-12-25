using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

/// <summary>
/// Exception thrown for Replicate API related errors.
/// </summary>
public class ReplicateException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReplicateException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
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