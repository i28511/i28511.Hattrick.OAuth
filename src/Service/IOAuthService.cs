using System;
using System.Threading;
using System.Threading.Tasks;
using i28511.Hattrick.OAuth.Models;

namespace i28511.Hattrick.OAuth.Service;

/// <summary>
/// This class is used to handle OAuth authentication and authorization.
/// </summary>
/// <seealso cref="IOAuthService" />
public interface IOAuthService
{
    /// <summary>
    /// Asynchronously requests an OAuth token from the server.
    /// </summary>
    /// <param name="ct">The cancellation token used to cancel the operation.</param>
    /// <returns>An <see cref="OAuthRequestResult"/> containing the authorize URL and token secret.</returns>
    /// <exception cref="OAuthException">Thrown when there is an error with the OAuth request.</exception>
    /// <exception cref="Exception">Thrown when there is an unexpected error while requesting the OAuth token.</exception>
    public Task<OAuthRequestResult> RequestTokenAsync(CancellationToken ct);
    /// <summary>
    /// Authorize the OAuth request using the provided token, verifier, and secret.
    /// </summary>
    /// <param name="token">The OAuth token.</param>
    /// <param name="verifier">The OAuth verifier.</param>
    /// <param name="secret">The OAuth secret.</param>
    /// <param name="ct">The cancellation token used to cancel the operation.</param>
    /// <returns>The OAuth authorize result.</returns>
    /// <exception cref="OAuthException">Thrown when an error occurs while authorizing the OAuth request.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while authorizing the OAuth request.</exception>
    public Task<OAuthAuthorizeResult> AuthorizeAsync(string token, string verifier, string secret, CancellationToken ct);
}