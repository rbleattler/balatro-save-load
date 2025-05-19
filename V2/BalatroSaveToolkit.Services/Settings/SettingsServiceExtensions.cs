using BalatroSaveToolkit.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BalatroSaveToolkit.Services.Settings
{
    /// <summary>
    /// Extension methods for registering settings services with the DI container.
    /// </summary>
    public static class SettingsServiceExtensions
    {
        /// <summary>
        /// Adds the settings service to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddSettingsService(this IServiceCollection services)
        {
            services.AddSingleton<ISettingsService, SettingsService>();
            return services;
        }
    }
}
