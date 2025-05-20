using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;

namespace BalatroSaveToolkit.ViewModels
{    /// <summary>
     /// ViewModel for the main window.
     /// </summary>
  internal class MainWindowViewModel : ViewModelBase, IScreen
    {
        private string _windowTitle = "Balatro Save Toolkit";
        private bool _isGameRunning;
        private readonly RoutingState _router;

        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string WindowTitle
        {
            get => _windowTitle;
            private set => SetProperty(ref _windowTitle, value);
        }

        /// <summary>
        /// Gets a value indicating whether the game is running.
        /// </summary>
        public bool IsGameRunning
        {
            get => _isGameRunning;
            private set => SetProperty(ref _isGameRunning, value);
        }

        /// <summary>
        /// Gets the routing state for navigation between views.
        /// </summary>
        public RoutingState Router => _router;

        /// <summary>
        /// Gets the command to toggle the theme.
        /// </summary>
        public ICommand ToggleThemeCommand { get; }

        /// <summary>
        /// Gets the command to open the settings dialog.
        /// </summary>
        public ICommand OpenSettingsCommand { get; }

        /// <summary>
        /// Gets the command to open the about dialog.
        /// </summary>
        public ICommand OpenAboutCommand { get; }

        /// <summary>
        /// Gets the command to navigate back.
        /// </summary>
        public ICommand NavigateBackCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialize the router
            _router = new RoutingState();

            // Initialize commands
            ToggleThemeCommand = CreateCommand(() => ToggleTheme());            OpenSettingsCommand = CreateCommand(() => OpenSettings());
            OpenAboutCommand = CreateCommand(() => OpenAbout());
            NavigateBackCommand = ReactiveCommand.CreateFromObservable(
                () => Router.NavigateBack,
                Router.NavigationStack.WhenAnyValue(x => x.Count).Select(count => count > 0));            // Configure the activator for ReactiveUI
            this.WhenActivated(disposables =>
            {
                // Register the activation items that need to be cleaned up
                Disposable.Create(() => { /* Cleanup when deactivated */ })
                    .DisposeWith(disposables);

                // Navigate to Dashboard as the initial view
                Router.Navigate.Execute(new DashboardViewModel(this))
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }

        /// <summary>
        /// Initializes the ViewModel.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task InitializeAsync()
        {
            // Initialize resources, check game status, etc.
            await Task.CompletedTask;
        }

        private static void ToggleTheme()
        {
            // Toggle theme logic will be implemented later
        }

        private static void OpenSettings()
        {
            // Open settings dialog logic will be implemented later
        }

        private static void OpenAbout()
        {
            // Open about dialog logic will be implemented later
        }
    }
}
