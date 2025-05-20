using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Styling;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace BalatroSaveToolkit.ViewModels
{
    /// <summary>
    /// ViewModel for the main window.
    /// </summary>
    internal class MainWindowViewModel : ReactiveObject, IScreen, IActivatableViewModel
    {
        private readonly IThemeService? _themeService;
        private readonly IGameProcessService? _gameProcessService;
        private readonly IFileSystemService? _fileSystemService;
        private readonly ISettingsService? _settingsService;

        private string _windowTitle = "Balatro Save Toolkit";
        private bool _isGameRunning;
        private RoutingState _router;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialize services from service locator
            _themeService = Locator.Current.GetService<IThemeService>();
            _gameProcessService = Locator.Current.GetService<IGameProcessService>();
            _fileSystemService = Locator.Current.GetService<IFileSystemService>();
            _settingsService = Locator.Current.GetService<ISettingsService>();

            // Initialize the router
            _router = new RoutingState();
            Activator = new ViewModelActivator();

            // Initialize commands
            ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme);
            NavigateToSettingsCommand = ReactiveCommand.Create(NavigateToSettings);
            NavigateToHomeCommand = ReactiveCommand.Create(NavigateToHome);
            NavigateBackCommand = ReactiveCommand.CreateFromObservable(
                () => Router.NavigateBack,
                Router.NavigationStack.WhenAnyValue(x => x.Count).Select(count => count > 0));

            // Configure activation
            this.WhenActivated(disposables =>
            {
                // Register for cleanup
                Disposable.Create(() => { /* Cleanup when deactivated */ })
                    .DisposeWith(disposables);

                // Subscribe to Balatro process status changes
                if (_gameProcessService != null)
                {
                    _gameProcessService.BalatroProcessStatusChanged += (sender, isRunning) =>
                    {
                        IsGameRunning = isRunning;
                    };

                    // Start the process check
                    _gameProcessService.StartProcessCheck();
                }

                // Navigate to Dashboard as the initial view
                NavigateToHome();
            });
        }

        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string WindowTitle
        {
            get => _windowTitle;
            private set => this.RaiseAndSetIfChanged(ref _windowTitle, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the game is running.
        /// </summary>
        public bool IsGameRunning
        {
            get => _isGameRunning;
            private set => this.RaiseAndSetIfChanged(ref _isGameRunning, value);
        }

        /// <summary>
        /// Gets the routing state for navigation between views.
        /// </summary>
        public RoutingState Router => _router;

        /// <summary>
        /// Gets the view model activator.
        /// </summary>
        public ViewModelActivator Activator { get; }

        /// <summary>
        /// Gets the command to toggle the theme.
        /// </summary>
        public ICommand ToggleThemeCommand { get; }

        /// <summary>
        /// Gets the command to navigate to settings.
        /// </summary>
        public ICommand NavigateToSettingsCommand { get; }

        /// <summary>
        /// Gets the command to navigate to the home/dashboard view.
        /// </summary>
        public ICommand NavigateToHomeCommand { get; }

        /// <summary>
        /// Gets the command to navigate back.
        /// </summary>
        public ICommand NavigateBackCommand { get; }

        /// <summary>
        /// Toggles between dark and light themes.
        /// </summary>
        private void ToggleTheme()
        {
            if (_themeService != null)
            {
                bool isDark = _themeService.CurrentTheme.Key == ThemeVariant.Dark.Key;
                _themeService.SetTheme(!isDark);

                // Update settings if available
                if (_settingsService != null)
                {
                    // Since we don't have direct access to SaveSettings, let's use a common approach
                    // of setting properties which should trigger save on property change
                    _settingsService.UseDarkTheme = !isDark;
                    _settingsService.UseSystemTheme = false;
                }
            }
        }

        /// <summary>
        /// Navigates to the settings view.
        /// </summary>
        private void NavigateToSettings()
        {
            if (_themeService != null && _settingsService != null)
            {
                var settingsViewModel = new ThemeSettingsViewModel(_themeService, _settingsService);
                Router.Navigate.Execute(settingsViewModel);
            }
        }

        /// <summary>
        /// Navigates to the dashboard/home view.
        /// </summary>
        private void NavigateToHome()
        {
            var dashboardViewModel = new DashboardViewModel(this);
            Router.Navigate.Execute(dashboardViewModel);
        }
    }
}
