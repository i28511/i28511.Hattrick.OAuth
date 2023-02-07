using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using i28511.Hattrick.OAuth.Models;
using OAuth;

namespace i28511.Hattrick.OAuth.Service;

/// <summary>
/// OAuthService is a class that implements the IOAuthService interface.
/// This class is used to handle OAuth authentication and authorization.
/// </summary>
/// <seealso cref="IOAuthService" />
internal class OAuthService : IOAuthService
{
    /// <summary>
    /// The consumer key is a unique identifier for the application that is using the OAuth service.
    /// It is provided by the OAuth provider.
    /// </summary>
    private readonly string _consumerKey;
    /// <summary>
    /// The consumer secret is a secret key that is used to authenticate the application that is using the OAuth service.
    /// It is provided by the OAuth provider.
    /// </summary>
    private readonly string _consumerSecret;
    /// <summary>
    /// The callback URL is the URL that the OAuth provider will redirect to after the user has authorized the application.
    /// </summary>
    private readonly Uri _callbackUrl;
    /// <summary>
    /// The HTTP client is used to make HTTP requests to the OAuth provider.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthService"/> class.
    /// </summary>
    /// <param name="config">The configuration object that contains the required parameters for the OAuth service</param>
    public OAuthService(OAuthServiceConfiguration config)
    {
        _consumerKey = config.ConsumerKey;
        _consumerSecret = config.ConsumerSecret;
        _callbackUrl = config.CallbackUrl;
        _httpClient = new HttpClient();
    }


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
    public async Task<OAuthAuthorizeResult> AuthorizeAsync(string token, string verifier, string secret, CancellationToken ct)
    {
        try
        {
            var client = new OAuthRequest
            {
                Method = "GET",
                Type = OAuthRequestType.AccessToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = _consumerKey,
                ConsumerSecret = _consumerSecret,
                RequestUrl = OAuthPath.AccessTokenPath,
                Token = token,
                Verifier = verifier,
                TokenSecret = secret
            };
            
            var parameters = System.Web.HttpUtility.ParseQueryString(client.GetAuthorizationQuery());
            
            var uriBuilder = new UriBuilder(client.RequestUrl) { Query = parameters.ToString() ?? throw new InvalidOperationException() };

            var response = await _httpClient.GetAsync(uriBuilder.Uri, ct);

            if (!response.IsSuccessStatusCode || response.RequestMessage?.RequestUri is null)
            {

                var error = new OAuthError
                {
                    Error = "invalid_grant",
                    ErrorDescription = "The provided credentials are invalid."
                };

                throw new OAuthException(error);
            }

            var responseAsString = await response.Content.ReadAsStringAsync(ct);
            var queryParams = System.Web.HttpUtility.ParseQueryString(responseAsString);

            var accessToken = queryParams["oauth_token"];
            var accessTokenSecret = queryParams["oauth_token_secret"];


            if (accessToken == null || accessTokenSecret == null)
            {
                var error = new OAuthError
                {
                    Error = "invalid_grant",
                    ErrorDescription = "The provided credentials are invalid."
                };

                throw new OAuthException(error);
            }

            return new OAuthAuthorizeResult
            {
                Secret = accessTokenSecret,
                Token = accessToken
            };
        }
        catch (HttpRequestException ex)
        {
            const string message = "An error occurred while authorizing the OAuth request.";
            // Handle network-related errors
            throw new Exception(message, ex);
        }
        catch (OAuthException ex)
        {
            const string message = "An error occurred while authorizing the OAuth request:";
            // Handle OAuth-specific errors
            throw new Exception(message + ex.Message, ex);
        }
        catch (Exception ex)
        {
            const string message = "An unexpected error occurred while authorizing the OAuth request.";
            // Handle unexpected errors
            throw new Exception(message, ex);
        }

    }

    /// <summary>
    /// Asynchronously requests an OAuth token from the server.
    /// </summary>
    /// <param name="scopes">scopes to authorize</param>
    /// <param name="ct">The cancellation token used to cancel the operation.</param>
    /// <returns>An <see cref="OAuthRequestResult"/> containing the authorize URL and token secret.</returns>
    /// <exception cref="OAuthException">Thrown when there is an error with the OAuth request.</exception>
    /// <exception cref="Exception">Thrown when there is an unexpected error while requesting the OAuth token.</exception>
    public async Task<OAuthRequestResult> RequestTokenAsync(IReadOnlyCollection<HattrickScopes> scopes, CancellationToken ct)
    {
        const string invalidTokenMessage = "Failed to retreive the authentication token.";
        const string requestErrorMessage = "An error occurred while requesting the OAuth token.";
        const string unexpectedErrorMessage = "An unexpected error occurred while requesting the OAuth token.";

        try
        {
            var client = new OAuthRequest
            {
                Method = "GET",
                Type = OAuthRequestType.RequestToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = _consumerKey,
                ConsumerSecret = _consumerSecret,
                RequestUrl = OAuthPath.RequestTokenPath,
                Version = "1.0",
                CallbackUrl = _callbackUrl.AbsoluteUri
            };

            var auth = client.GetAuthorizationQuery();
            var uriBuilder = new UriBuilder(client.RequestUrl) { Query = auth };

            var response = await _httpClient.GetAsync(uriBuilder.Uri, ct);

            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                throw new OAuthException(new OAuthError
                {
                    Error = "invalid_token",
                    ErrorDescription = invalidTokenMessage
                });
            }

            var responseAsString = await response.Content.ReadAsStringAsync(ct);
            var queryParams = System.Web.HttpUtility.ParseQueryString(responseAsString);

            var oAuthToken = queryParams["oauth_token"];
            var oauthTokenSecret = queryParams["oauth_token_secret"];

            if (oauthTokenSecret is null || oAuthToken is null)
            {
                throw new OAuthException(new OAuthError
                {
                    Error = "invalid_token",
                    ErrorDescription = invalidTokenMessage
                });
            }

            var scopeValues = scopes is null ? new List<string>() : scopes.Select(s => s.ToString().ToLowerInvariant()).ToList();

            var parameters = System.Web.HttpUtility.ParseQueryString(new UriBuilder(OAuthPath.AuthorizePath)
            {
                Query = $"oauth_token={oAuthToken}"
            }.Query);

            if (scopeValues.Count > 0)
            {
                parameters.Add("scope", string.Join(",", scopeValues));
            }

            return new OAuthRequestResult
            {
                AuthorizeUrl = new UriBuilder(OAuthPath.AuthorizePath)
                {
                    Query = parameters.ToString() ?? throw new InvalidOperationException()
                }.Uri,
                TokenSecret = oauthTokenSecret
            };
        }
        catch (HttpRequestException ex)
        {
            throw new Exception(requestErrorMessage, ex);
        }
        catch (OAuthException ex)
        {
            throw new Exception(requestErrorMessage + ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new Exception(unexpectedErrorMessage, ex);
        }
    }
}