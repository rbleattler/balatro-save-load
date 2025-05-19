using BalatroSaveToolkit.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BalatroSaveToolkit.Services.FileSystem
{
    /// <summary>
    /// Extension methods for registering file system services with the DI container.
    /// </summary>
    public static class FileSystemServiceExtensions
    {
        /// <summary>
        /// Adds the file system service to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFileSystemService(this IServiceCollection services)
        {
            // Register the platform-specific file system service
            services.AddSingleton<IFileSystemService>(sp => FileSystemServiceFactory.Create());

            return services;
        }
    }
}
