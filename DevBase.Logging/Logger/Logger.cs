using System.Diagnostics;
using DevBase.Logging.Enums;

namespace DevBase.Logging.Logger;

public class Logger<T>
{
    private T _type;
         
    public Logger(T type)
    {
        this._type = type;
    }

    public void Write(Exception exception)
    {
        this.Write(exception.Message, LogType.ERROR);
    }

    public void Write(string message, LogType debugType)
    {
        Print(message, debugType);
    }

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