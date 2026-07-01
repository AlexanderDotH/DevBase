namespace DevBase.Api.Apis.Musixmatch.Missions;

internal static class MissionQueries
{
    public const string AvailableMissionsList = """
        query AvailableMissionsList($appId: String, $userId: ID, $userToken: String) {
            getAvailableMissions(input: {appId: $appId, userId: $userId, userToken: $userToken}) {
                items {
                    id
                    description
                    title
                }
            }
        }
        """;

    public const string LyricsList = """
        query LyricsList(
            $appId: String
            $languages: String
            $destinationLanguage: String
            $limit: Int = 25
            $missionId: String
            $nextToken: String = null
            $sortAscending: Boolean = true
            $sortBy: String = "rank"
            $status: String = "AVAILABLE"
            $userToken: String
        ) {
            getSortedLyrics(
                appId: $appId
                languages: $languages
                destinationLanguage: $destinationLanguage
                limit: $limit
                missionId: $missionId
                nextToken: $nextToken
                sortForward: $sortAscending
                sortBy: $sortBy
                status: $status
                userToken: $userToken
            ) {
                items {
                    id
                    artistName
                    commonTrackId
                    hasLyrics
                    hasSync
                    language
                    title
                }
            }
        }
        """;

    public const string CreateTask = """
        mutation CreateTask(
            $appId: String
            $missionId: ID!
            $resourceId: ID!
            $type: TaskType!
            $userId: ID!
            $userToken: String = null
        ) {
            reserveResource(
                input: {
                    appId: $appId
                    missionId: $missionId
                    resourceId: $resourceId
                    type: $type
                    userId: $userId
                    userToken: $userToken
                }
            ) {
                id
                lastUpdated
                status
                type
                missionId
                resource {
                    id
                    actionURI
                    artistName
                    commonTrackId
                    hasLyrics
                    hasSync
                    language
                    missionId
                    publishedStatusMacro
                    rank
                    status
                    title
                    trackLength
                }
            }
        }
        """;

    public const string CancelTask = """
        mutation CancelTask(
            $appId: String
            $missionId: ID!
            $resourceId: ID!
            $taskId: ID!
            $type: TaskType!
            $userId: ID!
            $userToken: String = null
        ) {
            releaseResource(
                input: {
                    appId: $appId
                    missionId: $missionId
                    resourceId: $resourceId
                    taskId: $taskId
                    type: $type
                    userId: $userId
                    userToken: $userToken
                }
            ) {
                id
                lastUpdated
                status
                type
                resource {
                    id
                    actionURI
                    artistName
                    commonTrackId
                    hasLyrics
                    hasSync
                    language
                    missionId
                    publishedStatusMacro
                    rank
                    status
                    title
                    trackLength
                }
            }
        }
        """;
}
