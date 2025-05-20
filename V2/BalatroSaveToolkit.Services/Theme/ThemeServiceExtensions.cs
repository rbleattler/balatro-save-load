using BalatroSaveToolkit.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BalatroSaveToolkit.Services.Theme
{
    /// <summary>
    /// Extension methods for registering theme services.
    /// </summary>
    public static class ThemeServiceExtensions
    {
        /// <summary>
        /// Adds theme services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddThemeServices(this IServiceCollection services)
        {
            services.AddSingleton<IThemeService, ThemeService>();
            return services;
        }
    }
}
