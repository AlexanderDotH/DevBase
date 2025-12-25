using System.Diagnostics;
using DevBase.Logging.Enums;

namespace DevBase.Logging.Logger;

/// <summary>
/// A generic logger class that provides logging functionality scoped to a specific type context.
/// </summary>
/// <typeparam name="T">The type of the context object associated with this logger.</typeparam>
public class Logger<T>
{
    /// <summary>
    /// The context object used to identify the source of the log messages.
    /// </summary>
    private T _type;
         
    /// <summary>
    /// Initializes a new instance of the <see cref="Logger{T}"/> class.
    /// </summary>
    /// <param name="type">The context object associated with this logger instance.</param>
    public Logger(T type)
    {
        this._type = type;
    }

    /// <summary>
    /// Logs an exception with <see cref="LogType.ERROR"/> severity.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    public void Write(Exception exception)
    {
        this.Write(exception.Message, LogType.ERROR);
    }

    /// <summary>
    /// Logs a message with the specified severity level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="debugType">The severity level of the log message.</param>
    public void Write(string message, LogType debugType)
    {
        Print(message, debugType);
    }

    /// <summary>
    /// Formats and writes the log message to the debug listeners.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="debugType">The severity level of the log message.</param>
    private void Print(string message, LogType debugType)
    {
        Debug.WriteLine(string.Format(
            "{3} : {0} : {2} : {1}", 
            this._type.GetType().Name, 
            message, 
            debugType.ToString(), 
            DateTime.Now.TimeOfDay.ToString()));
    }
}