# DevBase.Api Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Api project.

## Table of Contents

- [Apis](#apis)
  - [ApiClient](#apiclient)
  - [AppleMusic](#applemusic)
  - [BeautifulLyrics](#beautifullyrics)
  - [Deezer](#deezer)
- [Enums](#enums)
- [Exceptions](#exceptions)
- [Serializer](#serializer)
- [Structure](#structure)

## Apis

### ApiClient

```csharp
/// <summary>
/// Base class for API clients, providing common error handling and type conversion utilities.
/// </summary>
public class ApiClient
{
    /// <summary>
    /// Gets or sets a value indicating whether to throw exceptions on errors or return default values.
    /// </summary>
    public bool StrictErrorHandling { get; set; }
    
    /// <summary>
    /// Throws an exception if strict error handling is enabled, otherwise returns a default value for type T.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="exception">The exception to throw.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The calling file path.</param>
    /// <param name="callerLineNumber">The calling line number.</param>
    /// <returns>The default value of T if exception is not thrown.</returns>
    protected dynamic Throw<T>(
        System.Exception exception,
        [CallerMemberName] string callerMember = "", 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0)
    
    /// <summary>
    /// Throws an exception if strict error handling is enabled, otherwise returns a default tuple (empty string, false).
    /// </summary>
    /// <param name="exception">The exception to throw.</param>
    /// <param name="callerMember">The calling member name.</param>
    /// <param name="callerFilePath">The calling file path.</param>
    /// <param name="callerLineNumber">The calling line number.</param>
    /// <returns>A tuple (string.Empty, false) if exception is not thrown.</returns>
    protected (string, bool) ThrowTuple(
        System.Exception exception,
        [CallerMemberName] string callerMember = "", 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0)
}
```

### AppleMusic

```csharp
/// <summary>
/// Apple Music API client for searching tracks and retrieving lyrics.
/// </summary>
public class AppleMusic : ApiClient
{
    private readonly string _baseUrl;
    private readonly AuthenticationToken _apiToken;
    private GenericAuthenticationToken _userMediaToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleMusic"/> class.
    /// </summary>
    /// <param name="apiToken">The API token for authentication.</param>
    public AppleMusic(string apiToken)
    
    /// <summary>
    /// Sets the user media token for authenticated requests.
    /// </summary>
    /// <param name="userMediaToken">The user media token.</param>
    /// <returns>The current AppleMusic instance.</returns>
    public AppleMusic WithMediaUserToken(GenericAuthenticationToken userMediaToken)
    
    /// <summary>
    /// Sets the user media token from a cookie.
    /// </summary>
    /// <param name="myacinfoCookie">The myacinfo cookie value.</param>
    public async Task WithMediaUserTokenFromCookie(string myacinfoCookie)
    
    /// <summary>
    /// Creates an AppleMusic instance with an access token extracted from the website.
    /// </summary>
    /// <returns>A new AppleMusic instance or null if token extraction fails.</returns>
    public static async Task<AppleMusic> WithAccessToken()
    
    /// <summary>
    /// Searches for tracks on Apple Music.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="limit">The maximum number of results.</param>
    /// <returns>A list of AppleMusicTrack objects.</returns>
    public async Task<List<AppleMusicTrack>> Search(string searchTerm, int limit = 10)
    
    /// <summary>
    /// Performs a raw search and returns the JSON response.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="limit">The maximum number of results.</param>
    /// <returns>The raw JSON search response.</returns>
    public async Task<JsonAppleMusicSearchResult> RawSearch(string searchTerm, int limit = 10)
    
    /// <summary>
    /// Gets lyrics for a specific track.
    /// </summary>
    /// <param name="trackId">The track ID.</param>
    /// <returns>The lyrics response.</returns>
    public async Task<JsonAppleMusicLyricsResponse> GetLyrics(string trackId)
    
    /// <summary>
    /// Gets the API token.
    /// </summary>
    public AuthenticationToken ApiToken { get; }
}
```

### BeautifulLyrics

```csharp
/// <summary>
/// Beautiful Lyrics API client for retrieving song lyrics.
/// </summary>
public class BeautifulLyrics : ApiClient
{
    private readonly string _baseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="BeautifulLyrics"/> class.
    /// </summary>
    public BeautifulLyrics()
    
    /// <summary>
    /// Gets lyrics for a song by ISRC.
    /// </summary>
    /// <param name="isrc">The ISRC code.</param>
    /// <returns>Either TimeStampedLyric list or RichTimeStampedLyric list depending on lyrics type.</returns>
    public async Task<dynamic> GetLyrics(string isrc)
    
    /// <summary>
    /// Gets raw lyrics data for a song by ISRC.
    /// </summary>
    /// <param name="isrc">The ISRC code.</param>
    /// <returns>A tuple containing raw lyrics and a boolean indicating if lyrics are rich sync.</returns>
    public async Task<(string RawLyrics, bool IsRichSync)> GetRawLyrics(string isrc)
}
```

### Deezer

```csharp
/// <summary>
/// Deezer API client for searching tracks, retrieving lyrics, and downloading music.
/// </summary>
public class Deezer : ApiClient
{
    private readonly string _authEndpoint;
    private readonly string _apiEndpoint;
    private readonly string _pipeEndpoint;
    private readonly string _websiteEndpoint;
    private readonly string _mediaEndpoint;
    private readonly CookieContainer _cookieContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Deezer"/> class.
    /// </summary>
    /// <param name="arlToken">Optional ARL token for authentication.</param>
    public Deezer(string arlToken = "")
    
    /// <summary>
    /// Gets a JWT token for API authentication.
    /// </summary>
    /// <returns>The JWT token response.</returns>
    public async Task<JsonDeezerJwtToken> GetJwtToken()
    
    /// <summary>
    /// Gets an access token for unlogged requests.
    /// </summary>
    /// <param name="appID">The application ID.</param>
    /// <returns>The access token response.</returns>
    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string appID = "457142")
    
    /// <summary>
    /// Gets an access token for a session.
    /// </summary>
    /// <param name="sessionID">The session ID.</param>
    /// <param name="appID">The application ID.</param>
    /// <returns>The access token response.</returns>
    public async Task<JsonDeezerAuthTokenResponse> GetAccessToken(string sessionID, string appID = "457142")
    
    /// <summary>
    /// Gets an ARL token from a session.
    /// </summary>
    /// <param name="sessionID">The session ID.</param>
    /// <returns>The ARL token.</returns>
    public async Task<string> GetArlTokenFromSession(string sessionID)
    
    /// <summary>
    /// Gets lyrics for a track.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>A tuple containing raw lyrics and a list of timestamped lyrics.</returns>
    public async Task<(string RawLyrics, AList<TimeStampedLyric> TimeStampedLyrics)> GetLyrics(string trackID)
    
    /// <summary>
    /// Gets lyrics using the AJAX endpoint.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>The raw lyrics response.</returns>
    public async Task<JsonDeezerRawLyricsResponse> GetLyricsAjax(string trackID)
    
    /// <summary>
    /// Gets lyrics using the GraphQL endpoint.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>The lyrics response.</returns>
    public async Task<JsonDeezerLyricsResponse> GetLyricsGraph(string trackID)
    
    /// <summary>
    /// Gets the CSRF token.
    /// </summary>
    /// <returns>The CSRF token.</returns>
    public async Task<string> GetCsrfToken()
    
    /// <summary>
    /// Gets user data.
    /// </summary>
    /// <param name="retries">Number of retries.</param>
    /// <returns>The user data.</returns>
    public async Task<JsonDeezerUserData> GetUserData(int retries = 5)
    
    /// <summary>
    /// Gets raw user data.
    /// </summary>
    /// <param name="retries">Number of retries.</param>
    /// <returns>The raw user data.</returns>
    public async Task<string> GetUserDataRaw(int retries = 5)
    
    /// <summary>
    /// Gets song details.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>The DeezerTrack object.</returns>
    public async Task<DeezerTrack> GetSong(string trackID)
    
    /// <summary>
    /// Gets detailed song information.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <param name="csrfToken">The CSRF token.</param>
    /// <param name="retries">Number of retries.</param>
    /// <returns>The song details.</returns>
    public async Task<JsonDeezerSongDetails> GetSongDetails(string trackID, string csrfToken, int retries = 5)
    
    /// <summary>
    /// Gets song URLs for downloading.
    /// </summary>
    /// <param name="trackToken">The track token.</param>
    /// <param name="licenseToken">The license token.</param>
    /// <returns>The song source information.</returns>
    public async Task<JsonDeezerSongSource> GetSongUrls(string trackToken, string licenseToken)
    
    /// <summary>
    /// Downloads a song.
    /// </summary>
    /// <param name="trackID">The track ID.</param>
    /// <returns>The decrypted song data.</returns>
    public async Task<byte[]> DownloadSong(string trackID)
    
    /// <summary>
    /// Searches for content.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>The search response.</returns>
    public async Task<JsonDeezerSearchResponse> Search(string query)
    
    /// <summary>
    /// Searches for songs with specific parameters.
    /// </summary>
    /// <param name="track">Track name.</param>
    /// <param name="artist">Artist name.</param>
    /// <param name="album">Album name.</param>
    /// <param name="strict">Whether to use strict search.</param>
    /// <returns>The search response.</returns>
    public async Task<JsonDeezerSearchResponse> Search(string track = "", string artist = "", string album = "", bool strict = false)
    
    /// <summary>
    /// Searches for songs and returns track data.
    /// </summary>
    /// <param name="track">Track name.</param>
    /// <param name="artist">Artist name.</param>
    /// <param name="album">Album name.</param>
    /// <param name="strict">Whether to use strict search.</param>
    /// <param name="limit">Maximum number of results.</param>
    /// <returns>A list of DeezerTrack objects.</returns>
    public async Task<List<DeezerTrack>> SearchSongData(string track = "", string artist = "", string album = "", bool strict = false, int limit = 10)
}
```

## Enums

### EnumAppleMusicExceptionType
```csharp
/// <summary>
/// Specifies the type of Apple Music exception.
/// </summary>
public enum EnumAppleMusicExceptionType
{
    /// <summary>User media token is not provided.</summary>
    UnprovidedUserMediaToken,
    /// <summary>Access token is unavailable.</summary>
    AccessTokenUnavailable,
    /// <summary>Search results are empty.</summary>
    SearchResultsEmpty
}
```

### EnumBeautifulLyricsExceptionType
```csharp
/// <summary>
/// Specifies the type of Beautiful Lyrics exception.
/// </summary>
public enum EnumBeautifulLyricsExceptionType
{
    /// <summary>Lyrics not found.</summary>
    LyricsNotFound,
    /// <summary>Failed to parse lyrics.</summary>
    LyricsParsed
}
```

### EnumDeezerExceptionType
```csharp
/// <summary>
/// Specifies the type of Deezer exception.
/// </summary>
public enum EnumDeezerExceptionType
{
    /// <summary>ARL token is missing or invalid.</summary>
    ArlToken, 
    /// <summary>App ID is invalid.</summary>
    AppId, 
    /// <summary>App session ID is invalid.</summary>
    AppSessionId, 
    /// <summary>Session ID is invalid.</summary>
    SessionId, 
    /// <summary>No CSRF token available.</summary>
    NoCsrfToken, 
    /// <summary>CSRF token is invalid.</summary>
    InvalidCsrfToken, 
    /// <summary>JWT token has expired.</summary>
    JwtExpired, 
    /// <summary>Song details are missing.</summary>
    MissingSongDetails,
    /// <summary>Failed to receive song details.</summary>
    FailedToReceiveSongDetails,
    /// <summary>Wrong parameter provided.</summary>
    WrongParameter, 
    /// <summary>Lyrics not found.</summary>
    LyricsNotFound,
    /// <summary>Failed to parse CSRF token.</summary>
    CsrfParsing,
    /// <summary>User data error.</summary>
    UserData,
    /// <summary>URL data error.</summary>
    UrlData
}
```

## Exceptions

### AppleMusicException
```csharp
/// <summary>
/// Exception thrown for Apple Music API related errors.
/// </summary>
public class AppleMusicException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppleMusicException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public AppleMusicException(EnumAppleMusicExceptionType type)
}
```

### BeautifulLyricsException
```csharp
/// <summary>
/// Exception thrown for Beautiful Lyrics API related errors.
/// </summary>
public class BeautifulLyricsException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BeautifulLyricsException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public BeautifulLyricsException(EnumBeautifulLyricsExceptionType type)
}
```

### DeezerException
```csharp
/// <summary>
/// Exception thrown for Deezer API related errors.
/// </summary>
public class DeezerException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeezerException"/> class.
    /// </summary>
    /// <param name="type">The type of error.</param>
    public DeezerException(EnumDeezerExceptionType type)
}
```

## Serializer

### JsonDeserializer
```csharp
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
    
    /// <summary>
    /// Deserializes the JSON string into an object of type T.
    /// </summary>
    /// <param name="input">The JSON string.</param>
    /// <returns>The deserialized object.</returns>
    public T Deserialize(string input)
    
    /// <summary>
    /// Deserializes the JSON string into an object of type T asynchronously.
    /// </summary>
    /// <param name="input">The JSON string.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object.</returns>
    public Task<T> DeserializeAsync(string input)
    
    /// <summary>
    /// Gets or sets the list of errors encountered during deserialization.
    /// </summary>
    public AList<string> ErrorList { get; set; }
}
```

## Structure

### AppleMusicTrack
```csharp
/// <summary>
/// Represents a track from Apple Music.
/// </summary>
public class AppleMusicTrack
{
    /// <summary>Gets or sets the track title.</summary>
    public string Title { get; set; }
    /// <summary>Gets or sets the album name.</summary>
    public string Album { get; set; }
    /// <summary>Gets or sets the duration in milliseconds.</summary>
    public int Duration { get; set; }
    /// <summary>Gets or sets the array of artists.</summary>
    public string[] Artists { get; set; }
    /// <summary>Gets or sets the array of artwork URLs.</summary>
    public string[] ArtworkUrls { get; set; }
    /// <summary>Gets or sets the service internal ID.</summary>
    public string ServiceInternalId { get; set; }
    /// <summary>Gets or sets the ISRC code.</summary>
    public string Isrc { get; set; }

    /// <summary>
    /// Creates an AppleMusicTrack from a JSON response.
    /// </summary>
    /// <param name="response">The JSON response.</param>
    /// <returns>An AppleMusicTrack instance.</returns>
    public static AppleMusicTrack FromResponse(JsonAppleMusicSearchResultResultsSongData response)
}
```

### DeezerTrack
```csharp
/// <summary>
/// Represents a track from Deezer.
/// </summary>
public class DeezerTrack
{
    /// <summary>Gets or sets the track title.</summary>
    public string Title { get; set; }
    /// <summary>Gets or sets the album name.</summary>
    public string Album { get; set; }
    /// <summary>Gets or sets the duration in milliseconds.</summary>
    public int Duration { get; set; }
    /// <summary>Gets or sets the array of artists.</summary>
    public string[] Artists { get; set; }
    /// <summary>Gets or sets the array of artwork URLs.</summary>
    public string[] ArtworkUrls { get; set; }
    /// <summary>Gets or sets the service internal ID.</summary>
    public string ServiceInternalId { get; set; }
}
```

### JSON Structure Classes
The project contains numerous JSON structure classes for deserializing API responses:

#### Apple Music JSON Structures
- `JsonAppleMusicLyricsResponse`
- `JsonAppleMusicLyricsResponseData`
- `JsonAppleMusicLyricsResponseDataAttributes`
- `JsonAppleMusicSearchResult`
- `JsonAppleMusicSearchResultResultsSong`
- And many more...

#### Beautiful Lyrics JSON Structures
- `JsonBeautifulLyricsLineLyricsResponse`
- `JsonBeautifulLyricsRichLyricsResponse`
- And related vocal group classes...

#### Deezer JSON Structures
- `JsonDeezerArlTokenResponse`
- `JsonDeezerAuthTokenResponse`
- `JsonDeezerJwtToken`
- `JsonDeezerLyricsResponse`
- `JsonDeezerRawLyricsResponse`
- `JsonDeezerSearchResponse`
- `JsonDeezerSongDetails`
- `JsonDeezerSongSource`
- `JsonDeezerUserData`
- And many more...
