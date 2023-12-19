using Newtonsoft.Json.Linq;

namespace DevBase.Api.Utils;

public class JsonUtils
{
    public static void TryGetString(JObject jObject, string fieldName, out string convertedToken)
    {
        JToken? token = null;
        jObject.TryGetValue(fieldName, out token);

        switch (token.Type)
        {
            case JTokenType.String:
            {
                string rawToken = token.Value<string>();

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
                string rawToken = token.Value<string>();

                if (string.IsNullOrEmpty(rawToken))
                {
                    convertedDateTime = DateTime.MinValue;
                    return;
                }

                long convertedLong = long.MinValue;
                long.TryParse(rawToken, out convertedLong);

                if (convertedLong == long.MinValue)
                {
                    convertedDateTime = DateTime.MinValue;
                    return;
                }
                
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(convertedLong);
                convertedDateTime = dateTimeOffset.Date;
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