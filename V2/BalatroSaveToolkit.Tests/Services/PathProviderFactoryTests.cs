using BalatroSaveToolkit.Services.Implementations;
using BalatroSaveToolkit.Services.Implementations.Linux;
using BalatroSaveToolkit.Services.Implementations.MacCatalyst;
using BalatroSaveToolkit.Services.Implementations.Windows;
using BalatroSaveToolkit.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Runtime.InteropServices;

namespace BalatroSaveToolkit.Tests.Services
{
    [TestClass]
    public class PathProviderFactoryTests
    {
        private Mock<ILogService> _mockLogService;
        private Mock<IErrorHandlingService> _mockErrorHandler;
        private IServiceCollection _services;

        [TestInitialize]
        public void Setup()
        {
            _mockLogService = new Mock<ILogService>();
            _mockErrorHandler = new Mock<IErrorHandlingService>();
            _services = new ServiceCollection();

            // Register mock services
            _services.AddSingleton(_mockLogService.Object);
            _services.AddSingleton(_mockErrorHandler.Object);
        }

        [TestMethod]
        public void RegisterPathProvider_ShouldRegisterAppropriateProviderForPlatform()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_mockLogService.Object);
            serviceCollection.AddSingleton(_mockErrorHandler.Object);

            // Act
            PathProviderFactory.RegisterPathProvider(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var pathProvider = serviceProvider.GetRequiredService<IPathProvider>();

            // Assert
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.IsInstanceOfType(pathProvider, typeof(WindowsPathProvider));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.IsInstanceOfType(pathProvider, typeof(MacPathProvider));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.IsInstanceOfType(pathProvider, typeof(LinuxPathProvider));
            }
            else
            {
                // Fallback case - should still get some implementation
                Assert.IsNotNull(pathProvider);
            }
        }

        [TestMethod]
        public void GetPathProvider_Windows_ShouldReturnWindowsPathProvider()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_mockLogService.Object);
            serviceCollection.AddSingleton(_mockErrorHandler.Object);
            serviceCollection.AddSingleton<WindowsPathProvider>();

            // Act - Use reflection to call GetPathProvider with OSPlatform.Windows
            var method = typeof(PathProviderFactory).GetMethod("GetPathProvider",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var pathProvider = method.Invoke(null, new object[] {
                serviceCollection.BuildServiceProvider(), OSPlatform.Windows }) as IPathProvider;

            // Assert
            Assert.IsInstanceOfType(pathProvider, typeof(WindowsPathProvider));
        }

        [TestMethod]
        public void GetPathProvider_OSX_ShouldReturnMacPathProvider()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_mockLogService.Object);
            serviceCollection.AddSingleton(_mockErrorHandler.Object);
            serviceCollection.AddSingleton<MacPathProvider>();

            // Act - Use reflection to call GetPathProvider with OSPlatform.OSX
            var method = typeof(PathProviderFactory).GetMethod("GetPathProvider",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var pathProvider = method.Invoke(null, new object[] {
                serviceCollection.BuildServiceProvider(), OSPlatform.OSX }) as IPathProvider;

            // Assert
            Assert.IsInstanceOfType(pathProvider, typeof(MacPathProvider));
        }

        [TestMethod]
        public void GetPathProvider_Linux_ShouldReturnLinuxPathProvider()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_mockLogService.Object);
            serviceCollection.AddSingleton(_mockErrorHandler.Object);
            serviceCollection.AddSingleton<LinuxPathProvider>();

            // Act - Use reflection to call GetPathProvider with OSPlatform.Linux
            var method = typeof(PathProviderFactory).GetMethod("GetPathProvider",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var pathProvider = method.Invoke(null, new object[] {
                serviceCollection.BuildServiceProvider(), OSPlatform.Linux }) as IPathProvider;

            // Assert
            Assert.IsInstanceOfType(pathProvider, typeof(LinuxPathProvider));
        }

        [TestMethod]
        public void GetPathProvider_UnknownPlatform_ShouldFallbackToBasePathProvider()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_mockLogService.Object);
            serviceCollection.AddSingleton(_mockErrorHandler.Object);

            // Register a fallback provider implementation
            serviceCollection.AddSingleton<IPathProvider>(sp =>
                new FallbackPathProvider(sp.GetService<ILogService>(), sp.GetService<IErrorHandlingService>()));

            // Act - Use reflection to call GetPathProvider with a custom OSPlatform
            var method = typeof(PathProviderFactory).GetMethod("GetPathProvider",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            // Create a new custom OSPlatform that doesn't match any existing ones
            var customOSPlatform = OSPlatform.Create("Custom");

            var pathProvider = method.Invoke(null, new object[] {
                serviceCollection.BuildServiceProvider(), customOSPlatform }) as IPathProvider;

            // Assert
            Assert.IsInstanceOfType(pathProvider, typeof(FallbackPathProvider));
        }

        /// <summary>
        /// A simple fallback path provider for testing purposes.
        /// </summary>
        private class FallbackPathProvider : BasePathProvider
        {
            public override bool IsCaseSensitive => true;

            public FallbackPathProvider(ILogService logService, IErrorHandlingService errorHandler)
                : base(logService, errorHandler)
            {
            }

            public override Task<string> GetBalatroSaveDirectoryAsync(bool steamInstallation = false)
            {
                return Task.FromResult("/fallback/balatro/save/path");
            }
        }
    }
}
