using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Missions;

internal sealed class MusixMatchGraphQlRequest
{
    public MusixMatchGraphQlRequest(string query, string operationName, object variables)
    {
        this.Query = query;
        this.OperationName = operationName;
        this.Variables = variables;
    }

    [JsonProperty("query")]
    public string Query { get; }

    [JsonProperty("operationName")]
    public string OperationName { get; }

    [JsonProperty("variables")]
    public object Variables { get; }
}
