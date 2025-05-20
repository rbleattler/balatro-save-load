using BalatroSaveToolkit.Core.Services;
using Splat;

namespace BalatroSaveToolkit.Services.Notifications
{
    /// <summary>
    /// Extension methods for registering the notification service.
    /// </summary>
    public static class NotificationServiceExtensions
    {
        /// <summary>
        /// Registers the notification service with the dependency injection container.
        /// </summary>
        /// <param name="services">The service locator.</param>
        public static void RegisterNotificationService(this IMutableDependencyResolver services)
        {
            services.RegisterLazySingleton<INotificationService>(() => new NotificationService());
        }
    }
}
