namespace i28511.Hattrick.OAuth.Service;

/// <summary>
/// OAuthPath
/// </summary>
public static class OAuthPath
{
    /// <summary>
    /// Gets the request token path.
    /// </summary>
    /// <value>
    /// The request token path.
    /// </value>
    public static string RequestTokenPath => "https://chpp.hattrick.org/oauth/request_token.ashx";
    /// <summary>
    /// Gets the authorize path.
    /// </summary>
    /// <value>
    /// The authorize path.
    /// </value>
    public static string AuthorizePath => "https://chpp.hattrick.org/oauth/authorize.aspx";
    /// <summary>
    /// Gets the access token path.
    /// </summary>
    /// <value>
    /// The access token path.
    /// </value>
    public static string AccessTokenPath => "https://chpp.hattrick.org/oauth/access_token.ashx";
}