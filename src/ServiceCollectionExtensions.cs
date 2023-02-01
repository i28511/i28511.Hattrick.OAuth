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
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddHattrickOAuth(this IServiceCollection services)
        {
            // Adds the OAuth service to the service collection as a scoped service.
            services.AddScoped<IOAuthService, OAuthService>();
            
            return services;
        }
    }

}