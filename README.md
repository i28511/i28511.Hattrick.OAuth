# i28511.Hattrick.OAuth

This class library provides an OAuth implementation for the Hattrick CHPP XML API.

## Installation

You can install the library via NuGet by running the following command in the Package Manager Console:

```powershell
Install-Package i28511.Hattrick.OAuth
```

## Service Collection Extension Method

```csharp
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///  Add the Hattrick OAuth implementation to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        public static IServiceCollection AddHattrickOAuth(this IServiceCollection services)
        {
            services.AddScoped<IOAuthService, OAuthService>();
            return services;
        }
    }
}
```

This Service Collection Extension Method allows user to add the Hattrick OAuth implementation in a single line in their Startup.cs file making it more readable and easy to understand, while also providing the ability to pass in a OAuthServiceConfiguration object to configure the OAuth service.


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


## Compatibility

This library is compatible with the following frameworks:
- net7.0
- Microsoft.Extensions.DependencyInjection.Abstractions (>= 7.0.0)
- OAuth.DotNetCore (>= 3.0.1)

To use this library on a different framework, you may need to specify compatible versions of the NuGet packages used in the project.