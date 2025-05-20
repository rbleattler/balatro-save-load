using System;
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
using BalatroSaveToolkit.Services.Settings;
using BalatroSaveToolkit.ViewModels;
using BalatroSaveToolkit.Views;
using ReactiveUI;
using Splat;

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
        RegisterActivationHandlers();

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
    }

    private static void ConfigureServices()
    {
        // Create services
        var viewStackService = new ViewStackService();
        var navigationService = new NavigationService(viewStackService);
        var fileSystemService = FileSystemServiceFactory.Create();
        var settingsService = new SettingsService(fileSystemService);

        // Register services
        Locator.CurrentMutable.RegisterConstant(fileSystemService, typeof(IFileSystemService));
        Locator.CurrentMutable.RegisterConstant(settingsService, typeof(ISettingsService));
        Locator.CurrentMutable.RegisterConstant(viewStackService, typeof(IViewStackService));
        Locator.CurrentMutable.RegisterConstant(navigationService, typeof(INavigationService));

        // Create host screen for routing
        var hostScreen = new HostScreen();
        Locator.CurrentMutable.RegisterConstant(hostScreen, typeof(IScreen));

        // Register ViewModels
        Locator.CurrentMutable.Register(() => new MainWindowViewModel());
        Locator.CurrentMutable.Register(() => new DashboardViewModel(Locator.Current.GetService<IScreen>()));

        // Register view-viewmodel mappings
        RegisterViewMappings();
    }

    private static void RegisterActivationHandlers()
    {
        // This allows ReactiveUI to properly activate views
        Locator.CurrentMutable.RegisterConstant(
            new AvaloniaActivationForViewFetcher(),
            typeof(IActivationForViewFetcher));
    }

    private static void RegisterViewMappings()
    {
        // Register views with their view models
        Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<MainWindowViewModel>));
        Locator.CurrentMutable.Register(() => new DashboardView(), typeof(IViewFor<DashboardViewModel>));
    }

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
