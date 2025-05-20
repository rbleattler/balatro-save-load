using BalatroSaveToolkit.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BalatroSaveToolkit.Services
{
    /// <summary>
    /// Extension methods for registering navigation services with the DI container.
    /// </summary>
    public static class NavigationServiceExtensions
    {
        /// <summary>
        /// Adds the navigation service to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddNavigationServices(this IServiceCollection services)
        {
            services.AddSingleton<IViewStackService, ViewStackService>();
            services.AddSingleton<INavigationService, NavigationService>();

            return services;
        }
    }
}
