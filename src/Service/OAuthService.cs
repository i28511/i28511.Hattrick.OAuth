using System;
using System.Net.Http;
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
    private readonly string _callbackUrl;
    /// <summary>
    /// The HTTP client is used to make HTTP requests to the OAuth provider.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthService"/> class.
    /// </summary>
    /// <param name="consumerKey">consumerKey is the unique identifier for the application that is using the OAuth service.
    /// It is provided by the OAuth provider.
    /// </param>
    /// <param name="consumerSecret">consumerSecret is the secret key that is used to authenticate the application that is using the OAuth service.
    /// It is provided by the OAuth provider.
    /// </param>
    /// <param name="callbackUrl">callbackUrl is the URL that the OAuth provider will redirect to after the user has authorized the application.
    /// </param>
    public OAuthService(string consumerKey, string consumerSecret, string callbackUrl)
    {
        _consumerKey = consumerKey;
        _consumerSecret = consumerSecret;
        _callbackUrl = callbackUrl;
        _httpClient = new HttpClient();
    }


    /// <summary>
    /// Authorize the OAuth request using the provided token, verifier, and secret.
    /// </summary>
    /// <param name="token">The OAuth token.</param>
    /// <param name="verifier">The OAuth verifier.</param>
    /// <param name="secret">The OAuth secret.</param>
    /// <returns>The OAuth authorize result.</returns>
    /// <exception cref="OAuthException">Thrown when an error occurs while authorizing the OAuth request.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while authorizing the OAuth request.</exception>
    public async Task<OAuthAuthorizeResult> AuthorizeAsync(string token, string verifier, string secret)
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

            var auth = client.GetAuthorizationQuery();
            var uriBuilder = new UriBuilder(client.RequestUrl) { Query = auth };
            var response = await _httpClient.GetAsync(uriBuilder.Uri);

            if (!response.IsSuccessStatusCode || response.RequestMessage?.RequestUri is null)
            {

                var error = new OAuthError
                {
                    Error = "invalid_grant",
                    ErrorDescription = "The provided credentials are invalid."
                };

                throw new OAuthException(error);
            }

            var queryParams = System.Web.HttpUtility.ParseQueryString(response.RequestMessage.RequestUri.Query);

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
    /// <returns>An <see cref="OAuthRequestResult"/> containing the authorize URL and token secret.</returns>
    /// <exception cref="OAuthException">Thrown when there is an error with the OAuth request.</exception>
    /// <exception cref="Exception">Thrown when there is an unexpected error while requesting the OAuth token.</exception>
    public async Task<OAuthRequestResult> RequestTokenAsync()
    {
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
                CallbackUrl = _callbackUrl
            };

            // Using URL query authorization

            var auth = client.GetAuthorizationQuery();

            var uriBuilder = new UriBuilder(client.RequestUrl) { Query = auth };
            var response = await _httpClient.GetAsync(uriBuilder.Uri);


            if (!response.IsSuccessStatusCode || response.RequestMessage?.RequestUri is null)
            {

                var error = new OAuthError
                {
                    Error = "invalid_token",
                    ErrorDescription = "Failed to retreive the authentication token."
                };

                throw new OAuthException(error);
            }

            var queryParams = System.Web.HttpUtility.ParseQueryString(response.RequestMessage.RequestUri.Query);
            
            var oAuthToken = queryParams["oauth_token"];
            var oauthTokenSecret = queryParams["oauth_token_secret"];

            if (oauthTokenSecret is null || oAuthToken is null)
            {
                var error = new OAuthError
                {
                    Error = "invalid_token",
                    ErrorDescription = "Failed to retreive the authentication token."
                };

                throw new OAuthException(error);
            }

            var authorizeUrl = new UriBuilder(OAuthPath.AuthorizePath)
            {
                Query = $"oauth_token={oAuthToken}"
            };

            return new OAuthRequestResult
            {
                AuthorizeUrl = authorizeUrl.Uri,
                TokenSecret = oauthTokenSecret
            };
        }
        catch (HttpRequestException ex)
        {
            const string message = "An error occurred while requesting the OAuth token.";
            // Handle network-related errors
            throw new Exception(message, ex);
        }
        catch (OAuthException ex)
        {
            const string message = "An error occurred while requesting the OAuth token:";
            // Handle OAuth-specific errors
            throw new Exception(message + ex.Message, ex);
        }
        catch (Exception ex)
        {
            const string message = "An unexpected error occurred while requesting the OAuth token.";
            // Handle unexpected errors
            throw new Exception(message, ex);
        }
    }
}