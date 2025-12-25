namespace DevBase.Enums;

/// <summary>
/// Specifies the content type of a request or response.
/// </summary>
public enum EnumContentType
{
    /// <summary>
    /// application/json
    /// </summary>
    APPLICATION_JSON,
    
    /// <summary>
    /// application/x-www-form-urlencoded
    /// </summary>
    APPLICATION_FORM_URLENCODED,
    
    /// <summary>
    /// multipart/form-data
    /// </summary>
    MULTIPART_FORMDATA,
    
    /// <summary>
    /// text/plain
    /// </summary>
    TEXT_PLAIN,
    
    /// <summary>
    /// text/html
    /// </summary>
    TEXT_HTML
}