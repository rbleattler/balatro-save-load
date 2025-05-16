using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace BalatroSaveToolkit;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		// Register services
		builder.Services.AddSingleton<Services.Interfaces.IFileService, Services.Implementations.FileService>();

		// These will need implementations later
		builder.Services.AddSingleton<Services.Interfaces.ISettingsService>(
			serviceProvider => new Services.Implementations.MockSettingsService(
				serviceProvider.GetRequiredService<Services.Interfaces.IFileService>()));
		builder.Services.AddSingleton<Services.Interfaces.IGameProcessService>(
			serviceProvider => new Services.Implementations.MockGameProcessService());
		builder.Services.AddSingleton<Services.Interfaces.ISaveService>(
			serviceProvider => new Services.Implementations.MockSaveService(
				serviceProvider.GetRequiredService<Services.Interfaces.IFileService>()));
		builder.Services.AddSingleton<Services.Interfaces.ILogService>(
			serviceProvider => new Services.Implementations.MockLogService());

		// Register ViewModels
		builder.Services.AddSingleton<ViewModels.MainViewModel>();
		builder.Services.AddSingleton<ViewModels.DashboardViewModel>();
		builder.Services.AddSingleton<ViewModels.SettingsViewModel>();
		builder.Services.AddSingleton<ViewModels.SaveViewerViewModel>();

		// Register Views
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<Views.DashboardPage>();
		builder.Services.AddTransient<Views.SettingsPage>();
		builder.Services.AddTransient<Views.SaveViewerPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
