using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

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
            services.AddSingleton<INavigationService>(provider =>
            {
                var hostScreen = provider.GetService<IScreen>();
                if (hostScreen == null)
                {
                    throw new System.InvalidOperationException("IScreen must be registered before adding navigation services");
                }
                return new Navigation.NavigationService(hostScreen);
            });

            return services;
        }
    }
}
