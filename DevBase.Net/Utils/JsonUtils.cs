using DevBase.Net.Enums;
using DevBase.Net.Exceptions;
using Newtonsoft.Json.Linq;

namespace DevBase.Net.Utils;

public class JsonUtils
{
    protected JsonUtils() { }

    public static bool TryGetEntries(JObject parsedDocument, out Dictionary<string, dynamic> jsonEntries, params string[] except)
    {
        HashSet<string>? exceptSet = except.Length > 0 ? new HashSet<string>(except, StringComparer.Ordinal) : null;
        
        Dictionary<string, dynamic> jsonEntriesList = new Dictionary<string, dynamic>(parsedDocument.Count);
        
        foreach (KeyValuePair<string, JToken?> current in parsedDocument)
        {
            if (exceptSet != null && exceptSet.Contains(current.Key))
                continue;

            KeyValuePair<string, dynamic> dynamic = GetDynamic(current.Key, current.Value!);
            jsonEntriesList.Add(dynamic.Key, dynamic.Value);
        }

        jsonEntries = jsonEntriesList;
        return true;
    }
    
    public static void TryGetString(JObject jObject, string fieldName, out KeyValuePair<string, string> convertedToken)
    {
        JToken? token;

        if (!jObject.TryGetValue(fieldName, out token))
        {
            convertedToken = ToPair(fieldName, string.Empty);
            return;
        }

        switch (token.Type)
        {
            case JTokenType.String:
            {
                string rawToken = token.Value<string>()!;

                if (string.IsNullOrEmpty(rawToken))
                {
                    convertedToken = ToPair(fieldName, string.Empty);
                    return;
                }
                
                convertedToken = ToPair(fieldName, rawToken);
                return;
            }
        }

        convertedToken = ToPair(fieldName, string.Empty);
    }

    public static void TryGetNumber(JObject jObject, string fieldName, out KeyValuePair<string, int> convertedToken)
    {
        JToken? token;

        if (!jObject.TryGetValue(fieldName, out token))
        {
            convertedToken = ToPair(fieldName, 0);
            return;
        }

        switch (token.Type)
        {
            case JTokenType.Integer:
            {
                int rawToken = token.Value<int>()!;
                convertedToken = ToPair(fieldName, rawToken);
                return;
            }
        }

        convertedToken = ToPair(fieldName, 0);
    }
    
    public static void TryGetDateTime(JObject jObject, string fieldName, out KeyValuePair<string, DateTime> convertedDateTime)
    {
        JToken? token = null;
        jObject.TryGetValue(fieldName, out token);

        if (token == null)
        {
            convertedDateTime = ToPair(fieldName, DateTime.MinValue);
            return;
        }
        
        convertedDateTime = GetTimeDate(token, fieldName);
    }

    private static KeyValuePair<string, dynamic> GetDynamic(string fieldName, JToken token)
    {
        KeyValuePair<string, dynamic> dynamicData = new KeyValuePair<string, dynamic>(fieldName, default);
        
        switch (token.Type)
        {
            case JTokenType.String: 
                return new KeyValuePair<string, dynamic>(fieldName, token.Value<string>()!);
            case JTokenType.Integer: 
                return new KeyValuePair<string, dynamic>(fieldName, token.Value<int>()!);
            case JTokenType.Float: 
                return new KeyValuePair<string, dynamic>(fieldName, token.Value<float>()!);
            case JTokenType.Guid: 
                return new KeyValuePair<string, dynamic>(fieldName, token.Value<Guid>()!);
            case JTokenType.Boolean: 
                return new KeyValuePair<string, dynamic>(fieldName, token.Value<bool>()!);
            case JTokenType.Date:
                return new KeyValuePair<string, dynamic>(fieldName, GetTimeDate(token, fieldName).Value);
            case JTokenType.Object: 
                return new KeyValuePair<string, dynamic>(fieldName, token.Value<Object>()!);
        }

        return dynamicData;
    }
    
    private static KeyValuePair<string, DateTime> GetTimeDate(JToken jObject, string fieldName)
    {
        switch (jObject.Type)
        {
            case JTokenType.String:
            {
                string rawToken = jObject.Value<string>()!;

                if (string.IsNullOrEmpty(rawToken))
                    return ToPair(fieldName, DateTime.MinValue);

                if (long.TryParse(rawToken, out long convertedLong))
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(convertedLong);
                    return ToPair(fieldName, dateTimeOffset.Date);
                }
                
                return ToPair(fieldName, DateTime.MinValue);
            }

            case JTokenType.Date:
            {
                DateTime rawToken = jObject.Value<DateTime>();
                return ToPair(fieldName, rawToken);
            }

            case JTokenType.Integer:
            {
                int rawToken = jObject.Value<int>();
                
                long convertedLong = (long)rawToken;
                
                DateTimeOffset convertedDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(convertedLong);
                return ToPair(fieldName, convertedDateTimeOffset.DateTime);
            }
        }

        throw new ElementValidationException(EnumValidationReason.InvalidData);
    }

    private static KeyValuePair<string, string> ToPair(string fieldName, string content) =>
        KeyValuePair.Create(fieldName, content);
    
    private static KeyValuePair<string, int> ToPair(string fieldName, int content) =>
        KeyValuePair.Create(fieldName, content);
    
    private static KeyValuePair<string, DateTime> ToPair(string fieldName, DateTime content) =>
        KeyValuePair.Create(fieldName, content);
}