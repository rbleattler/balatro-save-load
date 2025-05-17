using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace BalatroSaveToolkit;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register services in the correct order to handle dependencies

		// Register storage services
		builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
		builder.Services.AddSingleton<IFolderPicker>(FolderPicker.Default);

		// First register services with no dependencies - Replace MockLogService with real LogService
		builder.Services.AddSingleton<Services.Interfaces.ILogService, Services.Implementations.LogService>();

		// Register services with dependencies on Log service
		builder.Services.AddSingleton<Services.Interfaces.IErrorHandlingService, Services.Implementations.ErrorHandlingService>();
		// Register services with dependencies on Error Handling - Use platform-specific file services
#if WINDOWS
        builder.Services.AddSingleton<Services.Interfaces.IFileService, Services.Implementations.Windows.WindowsFileService>();
        builder.Services.AddSingleton<Services.Interfaces.IPathProvider, Services.Implementations.Windows.WindowsPathProvider>();
#elif MACCATALYST
        builder.Services.AddSingleton<Services.Interfaces.IFileService, Services.Implementations.MacCatalyst.MacFileService>();
        // TODO: Add MacOS path provider when implemented
        // For now, use Windows implementation with a warning since BasePathProvider is abstract
        builder.Services.AddSingleton<Services.Interfaces.IPathProvider>(serviceProvider => {
            var logger = serviceProvider.GetRequiredService<Services.Interfaces.ILogService>();
            var errorHandler = serviceProvider.GetRequiredService<Services.Interfaces.IErrorHandlingService>();
            logger.LogWarning("MauiProgram", "Using WindowsPathProvider on MacCatalyst - this is temporary until MacPathProvider is implemented");
            return new Services.Implementations.Windows.WindowsPathProvider(logger, errorHandler);
        });
#elif LINUX
        builder.Services.AddSingleton<Services.Interfaces.IFileService, Services.Implementations.Linux.LinuxFileService>();
        // TODO: Add Linux path provider when implemented
        // For now, use Windows implementation with a warning since BasePathProvider is abstract
        builder.Services.AddSingleton<Services.Interfaces.IPathProvider>(serviceProvider => {
            var logger = serviceProvider.GetRequiredService<Services.Interfaces.ILogService>();
            var errorHandler = serviceProvider.GetRequiredService<Services.Interfaces.IErrorHandlingService>();
            logger.LogWarning("MauiProgram", "Using WindowsPathProvider on Linux - this is temporary until LinuxPathProvider is implemented");
            return new Services.Implementations.Windows.WindowsPathProvider(logger, errorHandler);
        });
#else
        builder.Services.AddSingleton<Services.Interfaces.IFileService, Services.Implementations.FileService>();
        // Default path provider for other platforms - use Windows implementation temporarily
        builder.Services.AddSingleton<Services.Interfaces.IPathProvider>(serviceProvider => {
            var logger = serviceProvider.GetRequiredService<Services.Interfaces.ILogService>();
            var errorHandler = serviceProvider.GetRequiredService<Services.Interfaces.IErrorHandlingService>();
            logger.LogWarning("MauiProgram", "Using WindowsPathProvider on unknown platform - this is temporary until platform-specific providers are implemented");
            return new Services.Implementations.Windows.WindowsPathProvider(logger, errorHandler);
        });
#endif

		// These will need proper implementations later
		builder.Services.AddSingleton<Services.Interfaces.ISettingsService>(
			serviceProvider => new Services.Implementations.MockSettingsService(
				serviceProvider.GetRequiredService<Services.Interfaces.IFileService>()));
		builder.Services.AddSingleton<Services.Interfaces.IGameProcessService>(
			serviceProvider => new Services.Implementations.MockGameProcessService());
		builder.Services.AddSingleton<Services.Interfaces.ISaveService>(
			serviceProvider => new Services.Implementations.MockSaveService(
				serviceProvider.GetRequiredService<Services.Interfaces.IFileService>()));
		// Register ViewModels
		builder.Services.AddSingleton<ViewModels.MainViewModel>();
		builder.Services.AddSingleton<ViewModels.DashboardViewModel>();
		builder.Services.AddSingleton<ViewModels.SettingsViewModel>();
		builder.Services.AddSingleton<ViewModels.SaveViewerViewModel>();
		builder.Services.AddSingleton<ViewModels.LogsViewModel>();

		// Register Views
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<Views.DashboardPage>();
		builder.Services.AddTransient<Views.SettingsPage>();
		builder.Services.AddTransient<Views.SaveViewerPage>();
		builder.Services.AddTransient<Views.LogsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
