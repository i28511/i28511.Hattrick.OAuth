# i28511.Hattrick.OAuth

This class library provides an easy-to-use implementation of OAuth authentication and authorization for the Hattrick CHPP XML API. It leverages the power of Microsoft's Dependency Injection framework to allow developers to add OAuth functionality to their applications with minimal code and configuration. The library is compatible with .NET 7.0 and Microsoft.Extensions.DependencyInjection.Abstractions, making it a great choice for modern .NET applications. With this library, you can authenticate and authorize your requests to the Hattrick API with just a few lines of code, making it an indispensable tool for anyone working with this API.

## Installation

You can install the library via NuGet by running the following command in the Package Manager Console:

```powershell
Install-Package i28511.Hattrick.OAuth
```

## Usage Example

To use the library, you will need to add the following line to your `Startup.cs` file:

```csharp
OAuthServiceConfiguration oauthConfig = new OAuthServiceConfiguration
{
    ConsumerKey = Configuration.GetValue<string>("OAuth:ClientId"),
    ConsumerSecret = Configuration.GetValue<string>("OAuth:ClientSecret"),
    CallbackUrl = new Uri(Configuration.GetValue<string>("OAuth:CallbackUrl"))
};
services.AddHattrickOAuth(oauthConfig);
```

This will add the IOAuthService and OAuthService to your service collection with the given configuration.

You will also need to configure the OAuth settings in your appsettings.json file. An example configuration is provided below:


```json
{
	"OAuth": {
		"ClientId": "YOUR_CLIENT_ID",
		"ClientSecret": "YOUR_CLIENT_SECRET",
		"CallbackUrl": "YOUR_CALLBACK_URL"
	}
}

```

You can then inject the `IOAuthService` into your controllers or services and use it to authenticate and authorize requests to the Hattrick API.

## Methods and Authentication flow

Call the RequestTokenAsync method, passing in a CancellationToken object, to request an OAuth token. 
This method returns an OAuthRequestResult object, which contains the authorize URL and token secret.

Use the authorize URL to login into Hattrick. The callbackUrl defined will be called, and the token and verifier will be on the query string (you will need them in the next call).

Call the AuthorizeAsync method, passing in the token, verifier, secret (token secret from the RequestToken result), and a CancellationToken object, to authorize the OAuth request. 

This method returns an OAuthAuthorizeResult object, which contains the token and secret that can be used from now on.

In case of any OAuth errors, the methods may throw an OAuthException exception, with the error description provided in the exception message.


## Compatibility

This library is compatible with the following frameworks:
- net7.0
- Microsoft.Extensions.DependencyInjection.Abstractions (>= 7.0.0)
- OAuth.DotNetCore (>= 3.0.1)

To use this library on a different framework, you may need to specify compatible versions of the NuGet packages used in the project.