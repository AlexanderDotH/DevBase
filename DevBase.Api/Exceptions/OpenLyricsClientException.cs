using DevBase.Api.Enums;
using DevBase.Exception;

namespace DevBase.Api.Exceptions;

public class OpenLyricsClientException : System.Exception
{
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