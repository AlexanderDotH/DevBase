using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

/// <summary>
/// Exception thrown for OpenLyricsClient API related errors.
/// </summary>
public class OpenLyricsClientException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenLyricsClientException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public OpenLyricsClientException(EnumOpenLyricsClientExceptionType type) : base(GetMessage(type)) { }

    private static string GetMessage(EnumOpenLyricsClientExceptionType type)
    {
        switch (type)
        {
            case EnumOpenLyricsClientExceptionType.PredictionInProgress:
                return "The prediction is still running, try again later";
            case EnumOpenLyricsClientExceptionType.PredictionUnavailable:
                return "Could not find the prediction, please resubmit the prediction or check if the id is correct";
            case EnumOpenLyricsClientExceptionType.SealingNotInitialized:
                return "You have to initialize the sealing";
        }

        throw new ErrorStatementException();
    }
}