using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Core.ViewModels;
using ReactiveUI;
using Splat;

namespace BalatroSaveToolkit.ViewModels {
    /// <summary>
    /// ViewModel for the main window.
    /// </summary>
    internal sealed class MainWindowViewModel : ReactiveObject, IScreen, IActivatableViewModel {
        private readonly IThemeService? _themeService;
        private readonly IGameProcessService? _gameProcessService;
        private readonly IFileSystemService? _fileSystemService;
        private readonly ISettingsService? _settingsService;

        private bool _isGameRunning;
        private RoutingState _router;
        private bool _isBusy;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel() {
            // Initialize services from the service locator
            _themeService = Locator.Current.GetService<IThemeService>();
            _gameProcessService = Locator.Current.GetService<IGameProcessService>();
            _fileSystemService = Locator.Current.GetService<IFileSystemService>();
            _settingsService = Locator.Current.GetService<ISettingsService>();

            // Initialize the router
            _router = new RoutingState();
            Activator = new ViewModelActivator();

            // Initialize commands - ensure all commands use the UI thread scheduler
            ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme, outputScheduler: RxApp.MainThreadScheduler);
            NavigateToSettingsCommand =
                ReactiveCommand.Create(NavigateToSettings, outputScheduler: RxApp.MainThreadScheduler);
            NavigateToHomeCommand = ReactiveCommand.Create(NavigateToHome, outputScheduler: RxApp.MainThreadScheduler);
            NavigateBackCommand = ReactiveCommand.CreateFromObservable(
                                                                       () => Router.NavigateBack,
                                                                       Router.NavigationStack.WhenAnyValue(x => x.Count)
                                                                             .Select(count => count > 0)
                                                                             .ObserveOn(RxApp.MainThreadScheduler),
                                                                       RxApp.MainThreadScheduler
                                                                      );
            DemoProgressCommand = ReactiveCommand.CreateFromTask(
                                                                 async () => {
                                                                     await Dispatcher.UIThread
                                                                         .InvokeAsync(() => AddNotification(
                                                                                      "Demo operation started..."
                                                                                     )
                                                                             );
                                                                     await Dispatcher.UIThread
                                                                         .InvokeAsync(ShowProgress);
                                                                     await Task.Delay(2500).ConfigureAwait(false);
                                                                     await Dispatcher.UIThread
                                                                         .InvokeAsync(HideProgress);
                                                                     await Dispatcher.UIThread
                                                                         .InvokeAsync(() => AddNotification(
                                                                                      "Demo operation complete!"
                                                                                     )
                                                                             );
                                                                 },
                                                                 // Explicitly set up the canExecute to run on the UI thread
                                                                 Observable.Return(true)
                                                                           .ObserveOn(RxApp.MainThreadScheduler),
                                                                 outputScheduler: RxApp.MainThreadScheduler
                                                                );

            // Show a demo notification and progress bar on startup for demonstration
            Task.Run(async () => {
                         await Task.Delay(500).ConfigureAwait(false);
                         await Dispatcher.UIThread.InvokeAsync(() => AddNotification(
                                                                    "Welcome! This is a demo notification."
                                                                   )
                                                              );
                         await Dispatcher.UIThread.InvokeAsync(ShowProgress);
                         await Task.Delay(2000).ConfigureAwait(false);
                         await Dispatcher.UIThread.InvokeAsync(HideProgress);
                     }
                    );

            // Configure activation
            this.WhenActivated(disposables => {
                                   // Register for cleanup
                                   Disposable.Create(() => {
                                                         /* Cleanup when deactivated */
                                                     }
                                                    )
                                             .DisposeWith(disposables); // Subscribe to Balatro process status changes
                                   if (_gameProcessService != null) {
                                       _gameProcessService.BalatroProcessStatusChanged += (sender, args) => {
                                           // Always update UI properties on the UI thread
                                           Dispatcher.UIThread.Post(() => IsGameRunning = args.IsRunning);
                                       };

                                       // Start the process check
                                       _gameProcessService.StartProcessCheck();
                                   }

                                   // Navigate to Dashboard as the initial view
                                   NavigateToHome();
                               }
                              );
        }

        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string WindowTitle { get; } = "Balatro Save Toolkit";

        /// <summary>
        /// Gets or sets a value indicating whether the game is running.
        /// </summary>
        public bool IsGameRunning {
            get => _isGameRunning;
            private set => this.RaiseAndSetIfChanged(ref _isGameRunning, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is busy.
        /// </summary>
        public bool IsBusy {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
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
        /// Gets the command for demo progress.
        /// </summary>
        public ReactiveCommand<Unit, Unit> DemoProgressCommand { get; }

        /// <summary>
        /// Gets the collection of notifications.
        /// </summary>
        public ObservableCollection<string> Notifications { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Toggles between dark and light themes.
        /// </summary>
        private void ToggleTheme() {
            if (_themeService != null) {
                bool isDark = _themeService.CurrentTheme.Key == ThemeVariant.Dark.Key;
                _themeService.SetTheme(!isDark);

                // Update settings if available
                if (_settingsService != null) {
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
        private void NavigateToSettings() {
            if (_themeService != null &&
                _settingsService != null) {
                var settingsPageViewModel = new ThemeSettingsPageViewModel(this, _themeService, _settingsService);
                Router.Navigate.Execute(settingsPageViewModel);
            }
        }

        /// <summary>
        /// Navigates to the dashboard/home view.
        /// </summary>
        private void NavigateToHome() {
            var dashboardViewModel = new DashboardViewModel(this);
            Router.Navigate.Execute(dashboardViewModel);
        }

        /// <summary>
        /// Shows progress by setting IsBusy to true.
        /// </summary>
        public void ShowProgress() {
            if (!Dispatcher.UIThread.CheckAccess())
                Dispatcher.UIThread.Post(() => IsBusy = true);
            else
                IsBusy = true;
        }

        /// <summary>
        /// Hides progress by setting IsBusy to false.
        /// </summary>
        public void HideProgress() {
            if (!Dispatcher.UIThread.CheckAccess())
                Dispatcher.UIThread.Post(() => IsBusy = false);
            else
                IsBusy = false;
        }

        /// <summary>
        /// Adds a notification message.
        /// </summary>
        /// <param name="message">The notification message.</param>
        public void AddNotification(string message) {
            void add() {
                Notifications.Add(message);
                // Optionally auto-remove after a delay
                Task.Run(async () => {
                             await Task.Delay(4000).ConfigureAwait(false);
                             RemoveNotification(message);
                         }
                        );
            }

            if (!Dispatcher.UIThread.CheckAccess())
                Dispatcher.UIThread.Post(add);
            else
                add();
        }

        /// <summary>
        /// Removes a notification message.
        /// </summary>
        /// <param name="message">The notification message.</param>
        public void RemoveNotification(string message) {
            void remove() { Notifications.Remove(message); }
            if (!Dispatcher.UIThread.CheckAccess())
                Dispatcher.UIThread.Post(remove);
            else
                remove();
        }
    }
}