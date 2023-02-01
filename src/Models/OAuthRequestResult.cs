using System;

namespace i28511.Hattrick.OAuth.Models;

/// <summary>
/// OAuthRequestResult
/// </summary>
public class OAuthRequestResult
{
    /// <summary>
    /// Gets or sets the authorize URL.
    /// </summary>
    /// <value>
    /// The authorize URL.
    /// </value>
    public Uri AuthorizeUrl { get; set; }
    /// <summary>
    /// Gets or sets the token secret.
    /// </summary>
    /// <value>
    /// The token secret.
    /// </value>
    public string TokenSecret { get; set; }
}