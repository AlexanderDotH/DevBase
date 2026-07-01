using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure
{
    public class MusixMatchApiResponse
    {
        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            Error = (_, args) => args.ErrorContext.Handled = true
        };

        public int StatusCode { get; init; }

        public double TimeElapsed { get; init; }

        public string Header { get; init; } = "{}";

        public string Body { get; init; } = "{}";

        public T? Cast<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.Body, SerializerSettings);
        }
    }

    public class MusixmatchApiResponse : MusixMatchApiResponse
    {
    }
}

namespace DevBase.Api.Apis.Musixmatch
{
    public class MusixMatchApiResponse : Structure.MusixMatchApiResponse
    {
    }

    public class MusixmatchApiResponse : Structure.MusixmatchApiResponse
    {
    }
}
