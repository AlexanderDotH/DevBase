﻿using DevBase.Typography.Encoded;

namespace DevBase.Extensions;

public static class Base64EncodedAStringExtension
{
    public static Base64EncodedAString ToBase64(this string content)
    {
        return new Base64EncodedAString(content);
    }
}