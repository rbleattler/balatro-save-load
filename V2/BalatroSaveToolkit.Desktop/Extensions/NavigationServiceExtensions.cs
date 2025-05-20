using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using BalatroSaveToolkit.Services.Navigation;
using BalatroSaveToolkit.UI.Infrastructure;
using BalatroSaveToolkit.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace BalatroSaveToolkit.Desktop.Extensions
{
    /// <summary>
    /// Extension methods for registering navigation services
    /// </summary>
    public static class NavigationServiceExtensions
    {
        /// <summary>
        /// Adds navigation services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddNavigationServices(this IServiceCollection services)
        {
            // Register the host screen
            services.AddSingleton<IScreen, MainViewModel>();

            // Register the navigation service
            services.AddSingleton<INavigationService, NavigationService>();

            // Register the view locator
            services.AddSingleton<IViewLocator, ViewLocator>();

            // Register view models
            services.AddTransient<HomeViewModel>();
            services.AddTransient<SettingsViewModel>();

            return services;
        }
    }
}