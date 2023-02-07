using i28511.Hattrick.OAuth.Service;
using Microsoft.Extensions.DependencyInjection;

namespace i28511.Hattrick.OAuth
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to configure OAuth-related services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the OAuth-related services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configuration">The configuration for the OAuth service.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddHattrickOAuth(this IServiceCollection services, OAuthServiceConfiguration configuration)
        {
            // Adds the OAuth service to the service collection as a scoped service with the given configuration.
            services.AddScoped<IOAuthService, OAuthService>(_ => new OAuthService(configuration));

            return services;
        }
    }

}