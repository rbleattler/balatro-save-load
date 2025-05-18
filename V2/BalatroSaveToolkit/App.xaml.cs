using BalatroSaveToolkit.Services.Implementations;
using BalatroSaveToolkit.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BalatroSaveToolkit;

public partial class App : Application
{
	private AppExceptionHandler? _exceptionHandler;
	private readonly IErrorHandlingService? _errorHandlingService;

	public App()
	{
		InitializeComponent();

		try
		{
			// Get services from the container
			if (IPlatformApplication.Current?.Services != null)
			{
				_errorHandlingService = IPlatformApplication.Current.Services.GetService<IErrorHandlingService>();

				if (_errorHandlingService != null)
				{
					// Set up global exception handling
					_exceptionHandler = new AppExceptionHandler(_errorHandlingService);
					_exceptionHandler.Initialize();

					// Log application start
					_errorHandlingService.LogError("Application", "Application started successfully", ErrorSeverity.Information, false);
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error initializing app: {ex.Message}");
		}
	}
	protected override Window CreateWindow(IActivationState? activationState)
	{
		try
		{
			return new Window(new AppShell());
		}
		catch (Exception ex)
		{
			_errorHandlingService?.HandleException(ex, "App", "Failed to create main window", ErrorSeverity.Critical);
			throw; // Re-throw as this is a critical failure
		}
	}
}
