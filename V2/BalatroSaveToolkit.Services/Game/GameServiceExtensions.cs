using BalatroSaveToolkit.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BalatroSaveToolkit.Services.Game
{
    /// <summary>
    /// Extension methods for registering game-related services.
    /// </summary>
    public static class GameServiceExtensions
    {
        /// <summary>
        /// Adds game-related services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            services.AddSingleton<IGameProcessService, GameProcessService>();
            return services;
        }
    }
}
