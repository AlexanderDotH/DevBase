using DevBase.Generics;
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

    public static bool TryGetEntries(JObject parsedDocument, out AList<KeyValuePair<string, object>> jsonEntries, params string[] except)
    {
        IEnumerator<KeyValuePair<string, JToken?>> enumerable = parsedDocument.GetEnumerator();

        AList<KeyValuePair<string, object>> jsonEntriesList = new AList<KeyValuePair<string, object>>();
        
        while (enumerable.MoveNext())
        {
            KeyValuePair<string, JToken> current = enumerable.Current!;
            
            if (except.Contains(current.Key))
                continue;
            
            jsonEntriesList.Add(KeyValuePair.Create<string, object>(current.Key, current.Value!));
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

    public static void TryGetDateTime(JObject jObject, string fieldName, out KeyValuePair<string, DateTime> convertedDateTime)
    {
        JToken? token = null;
        jObject.TryGetValue(fieldName, out token);

        if (token == null)
        {
            convertedDateTime = ToPair(fieldName, DateTime.MinValue);
            return;
        }

        switch (token.Type)
        {
            case JTokenType.String:
            {
                string rawToken = token.Value<string>()!;

                if (string.IsNullOrEmpty(rawToken))
                {
                    convertedDateTime = ToPair(fieldName, DateTime.MinValue);
                    return;
                }

                if (long.TryParse(rawToken, out long convertedLong))
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(convertedLong);
                    convertedDateTime = ToPair(fieldName, dateTimeOffset.Date);
                    return;
                }
                
                convertedDateTime = ToPair(fieldName, DateTime.MinValue);
                return;
            }

            case JTokenType.Date:
            {
                DateTime rawToken = token.Value<DateTime>();
                convertedDateTime = ToPair(fieldName, rawToken);
                return;
            }

            case JTokenType.Integer:
            {
                int rawToken = token.Value<int>();
                
                long convertedLong = (long)rawToken;
                
                DateTimeOffset convertedDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(convertedLong);
                convertedDateTime = ToPair(fieldName, convertedDateTimeOffset.DateTime);
                return;
            }
        }
        
        convertedDateTime = ToPair(fieldName, DateTime.MinValue);
    }
    
    
    private static KeyValuePair<string, string> ToPair(string fieldName, string content) =>
        KeyValuePair.Create(fieldName, content);
    
    private static KeyValuePair<string, DateTime> ToPair(string fieldName, DateTime content) =>
        KeyValuePair.Create(fieldName, content);
}