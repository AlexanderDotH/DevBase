using DevBase.Generics;
using Newtonsoft.Json;

namespace DevBase.Api.Serializer;

/// <summary>
/// A generic JSON deserializer helper that captures serialization errors.
/// </summary>
/// <typeparam name="T">The type to deserialize into.</typeparam>
public class JsonDeserializer<T>
{
    private JsonSerializerSettings _serializerSettings;
    private AList<string> _errorList;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDeserializer{T}"/> class.
    /// </summary>
    public JsonDeserializer()
    {
        this._errorList = new AList<string>();

        this._serializerSettings = new JsonSerializerSettings();

        this._serializerSettings.NullValueHandling = NullValueHandling.Ignore;

        this._serializerSettings.Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
            this._errorList.Add(args.ErrorContext.Error.Message);
            args.ErrorContext.Handled = true;
        };
    }

    /// <summary>
    /// Deserializes the JSON string into an object of type T.
    /// </summary>
    /// <param name="input">The JSON string.</param>
    /// <returns>The deserialized object.</returns>
    public T Deserialize(string input)
    {
        return JsonConvert.DeserializeObject<T>(input, this._serializerSettings);
    }

    /// <summary>
    /// Deserializes the JSON string into an object of type T asynchronously.
    /// </summary>
    /// <param name="input">The JSON string.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object.</returns>
    public Task<T> DeserializeAsync(string input)
    {
        return Task.FromResult(JsonConvert.DeserializeObject<T>(input, this._serializerSettings));
    }

    /// <summary>
    /// Gets or sets the list of errors encountered during deserialization.
    /// </summary>
    public AList<string> ErrorList
    {
        get => _errorList;
        set => _errorList = value;
    }
}