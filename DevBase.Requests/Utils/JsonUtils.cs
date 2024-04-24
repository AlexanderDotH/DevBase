using Newtonsoft.Json.Linq;

namespace DevBase.Requests.Utils;

// TODO: Implement this class
// TODO: Authentication token should have all values of the jwt(inside each class header and payload)
// TODO: Default fields like iss or iad should be properties
// TODO: Unit tests for the enw RequestHeaderBuilder @DevBase.Requests.Data.Header.RequestHeaderBuilder
// TODO: Implement unit tests for this class and the AuthenticationToken class
public class JsonUtils
{
    protected JsonUtils() { }


    public static bool TryGetEntries(JObject parsedDocument, out List<KeyValuePair<string, object>> jsonEntries)
    {
        IEnumerator<KeyValuePair<string, JToken?>> enumerable = parsedDocument.GetEnumerator();
        
        while (enumerable.MoveNext())
        {
            KeyValuePair<string, JToken> current = enumerable.Current!;
            
            
        }
    }

    private static bool TryGet(JToken token, JTokenType type, out object value)
    {
        object? carvedValue = null;
        

        if (carvedValue == null)
        {
            value = null;
            return false;
        }
        
        value = carvedValue;
        return true;
    }

    private static object ToValue(JToken token, JTokenType tokenType) => token.Type switch
    {
        JTokenType.String => Get<string>(token),
        _ => null
    };

    private static T Get<T>(JToken token, Type type)
    {
        T? content = token.Value<T>();

        typeof(T)
        
        if (content == null)
            return default;

        return content;
    } 
    
    public static void TryGetString(JObject jObject, string fieldName, out string convertedToken)
    {
        JToken? token;

        if (!jObject.TryGetValue(fieldName, out token))
        {
            convertedToken = string.Empty;
            return;
        }

        switch (token.Type)
        {
            case JTokenType.String:
            {
                string rawToken = token.Value<string>()!;

                if (string.IsNullOrEmpty(rawToken))
                {
                    convertedToken = string.Empty;
                    return;
                }

                convertedToken = rawToken;
                return;
            }
        }

        convertedToken = string.Empty;
    }
    
    public static void TryGetDateTime(JObject jObject, string fieldName, out DateTime convertedDateTime)
    {
        JToken? token = null;
        jObject.TryGetValue(fieldName, out token);

        if (token == null)
        {
            convertedDateTime = DateTime.MinValue;
            return;
        }

        switch (token.Type)
        {
            case JTokenType.String:
            {
                string rawToken = token.Value<string>()!;

                if (string.IsNullOrEmpty(rawToken))
                {
                    convertedDateTime = DateTime.MinValue;
                    return;
                }

                if (long.TryParse(rawToken, out long convertedLong))
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(convertedLong);
                    convertedDateTime = dateTimeOffset.Date;
                    return;
                }
                
                convertedDateTime = DateTime.MinValue;
                return;
            }

            case JTokenType.Date:
            {
                DateTime rawToken = token.Value<DateTime>();
                convertedDateTime = rawToken;
                return;
            }

            case JTokenType.Integer:
            {
                int rawToken = token.Value<int>();
                
                long convertedLong = (long)rawToken;
                
                DateTimeOffset convertedDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(convertedLong);
                convertedDateTime = convertedDateTimeOffset.DateTime;
                return;
            }
        }
        
        convertedDateTime = DateTime.MinValue;
    }
}