using DevBase.Api.Apis.Musixmatch.Missions;

namespace DevBase.Api.Apis.Musixmatch;

public partial class MusixMatch
{
    public MusixMatchMissionClient CreateMissionClient()
    {
        return new MusixMatchMissionClient(this);
    }

    [Obsolete("Musixmatch mission reservation is not well tested and may change account state.")]
    public bool ReserveTask(string missionId, string resourceId, string type = "VERIFY")
    {
        return this.CreateMissionClient().ReserveTask(missionId, resourceId, type);
    }

    [Obsolete("Musixmatch mission reservation is not well tested and may change account state.")]
    public Task<bool> ReserveTaskAsync(
        string missionId,
        string resourceId,
        string type = "VERIFY",
        CancellationToken cancellationToken = default)
    {
        return this.CreateMissionClient().ReserveTaskAsync(missionId, resourceId, type, cancellationToken);
    }

    [Obsolete("Musixmatch mission release is not well tested and may change account state.")]
    public bool CancelTask(string missionId, string resourceId, string taskId, string type)
    {
        return this.CreateMissionClient().CancelTask(missionId, resourceId, taskId, type);
    }

    [Obsolete("Musixmatch mission release is not well tested and may change account state.")]
    public Task<bool> CancelTaskAsync(
        string missionId,
        string resourceId,
        string taskId,
        string type,
        CancellationToken cancellationToken = default)
    {
        return this.CreateMissionClient().CancelTaskAsync(missionId, resourceId, taskId, type, cancellationToken);
    }
}
