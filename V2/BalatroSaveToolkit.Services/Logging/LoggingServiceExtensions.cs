using BalatroSaveToolkit.Core.Services;
using Splat;

namespace BalatroSaveToolkit.Services.Logging
{
    /// <summary>
    /// Extension methods for registering the logging service.
    /// </summary>
    public static class LoggingServiceExtensions
    {
        /// <summary>
        /// Registers the logging service with the dependency injection container.
        /// </summary>
        /// <param name="services">The service locator.</param>
        public static void RegisterLoggingService(this IMutableDependencyResolver services)
        {
            services.RegisterConstant<ILoggingService>(new LoggingService());
        }
    }
}
