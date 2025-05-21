using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using BalatroSaveToolkit.Services;
using BalatroSaveToolkit.Services.FileSystem;
using BalatroSaveToolkit.Services.Game;
using BalatroSaveToolkit.Services.Logging;
using BalatroSaveToolkit.Services.Notifications;
using BalatroSaveToolkit.Services.Settings;
using BalatroSaveToolkit.Services.Theme;
using BalatroSaveToolkit.Theme;
using BalatroSaveToolkit.ViewModels;
using BalatroSaveToolkit.Views;
using ReactiveUI;

namespace BalatroSaveToolkit;

internal partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Configure ReactiveUI and service dependencies
        ConfigureServices();

        // Register ReactiveUI activation handlers
        RegisterActivationHandlers();        // Initialize theme resources
        ThemeManager.Initialize(this);        // Setup theme service and apply initial theme
        var themeService = Locator.Current.GetService<IThemeService>();
        if (themeService != null)
        {
            themeService.Initialize();
            themeService.ThemeChanged += (sender, theme) =>
            {
                ThemeManager.ApplyTheme(this, theme == Avalonia.Styling.ThemeVariant.Dark);
            };

            // Apply initial theme
            ThemeManager.ApplyTheme(this, themeService.CurrentTheme == Avalonia.Styling.ThemeVariant.Dark);
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit
            DisableAvaloniaDataAnnotationValidation();

            // Get the main window view model from the service locator
            var mainViewModel = Locator.Current.GetService<MainWindowViewModel>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }    private static void ConfigureServices()
    {
        // Create services
        var viewStackService = new ViewStackService();
        var navigationService = new NavigationService(viewStackService);
        var fileSystemService = FileSystemServiceFactory.Create();
        var settingsService = new SettingsService(fileSystemService);
        var themeService = new ThemeService(settingsService);
        var gameProcessService = new GameProcessService();
        var loggingService = new Services.Logging.LoggingService();
        var notificationService = new Services.Notifications.NotificationService();

        // Register services
        Locator.CurrentMutable.RegisterConstant<IFileSystemService>(fileSystemService);
        Locator.CurrentMutable.RegisterConstant<ISettingsService>(settingsService);
        Locator.CurrentMutable.RegisterConstant<IViewStackService>(viewStackService);
        Locator.CurrentMutable.RegisterConstant<INavigationService>(navigationService);
        Locator.CurrentMutable.RegisterConstant<IThemeService>(themeService);
        Locator.CurrentMutable.RegisterConstant<IGameProcessService>(gameProcessService);
        Locator.CurrentMutable.RegisterConstant<ILoggingService>(loggingService);
        Locator.CurrentMutable.RegisterConstant<INotificationService>(notificationService);

        // Create host screen for routing
        var hostScreen = new HostScreen();
        Locator.CurrentMutable.RegisterConstant<IScreen>(hostScreen);        // Register ViewModels        Locator.CurrentMutable.Register(() => new MainWindowViewModel());

        // Use null checks to avoid possible null reference exceptions
        Locator.CurrentMutable.Register(() => {
            var screen = Locator.Current.GetService<IScreen>();
            return screen != null
                ? new DashboardViewModel(screen)
                : new DashboardViewModel(new HostScreen());
        });

        // Register SaveContentViewModel
        Locator.CurrentMutable.Register(() => {
            var screen = Locator.Current.GetService<IScreen>();
            return new SaveContentViewModel {
                // Screen property would be set when navigating to this view
            };
        });

        Locator.CurrentMutable.Register(() => {
            var themeService = Locator.Current.GetService<IThemeService>();
            var settingsService = Locator.Current.GetService<ISettingsService>();

            if (themeService != null && settingsService != null)
                return new ThemeSettingsViewModel(themeService, settingsService);

            throw new InvalidOperationException("Required services not available for ThemeSettingsViewModel");
        });

        // Register view-viewmodel mappings
        RegisterViewMappings();
    }

    private static void RegisterActivationHandlers()
    {
        // This allows ReactiveUI to properly activate views
        Locator.CurrentMutable.RegisterConstant<IActivationForViewFetcher>(new AvaloniaActivationForViewFetcher());
    }    private static void RegisterViewMappings()
    {
        // Register views with their view models
        Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<MainWindowViewModel>));
        Locator.CurrentMutable.Register(() => new DashboardView(), typeof(IViewFor<DashboardViewModel>));
        Locator.CurrentMutable.Register(() => new ThemeSettingsView(), typeof(IViewFor<ThemeSettingsViewModel>));
        Locator.CurrentMutable.Register(() => new SaveContentView(), typeof(IViewFor<SaveContentViewModel>));
    }    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "We know this code uses reflection")]
    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // Remove each plugin
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}

// Simple host screen implementation for ReactiveUI routing
internal class HostScreen : IScreen
{
    public RoutingState Router { get; } = new RoutingState();
}
