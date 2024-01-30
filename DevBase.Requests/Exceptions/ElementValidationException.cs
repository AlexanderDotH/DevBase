﻿using DevBase.Requests.Enums;

namespace DevBase.Requests.Exceptions;

public class ElementValidationException : System.Exception
{
    public ElementValidationException(EnumValidationReason exceptionTypes) : base(GetMessage(exceptionTypes)) { }

    private static string GetMessage(EnumValidationReason exceptionTypes)
    {
        switch (exceptionTypes)
        {
            case EnumValidationReason.Empty:
                return "Some or more fields/objects are empty";
            
        }

        return string.Empty;
    }
}