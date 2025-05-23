using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using BalatroSaveToolkit.Core.Services;
using ReactiveUI;
using Avalonia.Styling;
using Splat;
using Avalonia.Threading;

namespace BalatroSaveToolkit.ViewModels
{    /// <summary>
    /// View model for theme selection and management.
    /// </summary>
    class ThemeSettingsViewModel : ViewModelBase
    {
        private readonly IThemeService _themeService;
        private readonly ISettingsService _settingsService;
        private bool _useDarkTheme;
        private bool _followSystemTheme;
        private readonly bool _isWindowsPlatform;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeSettingsViewModel"/> class.
        /// </summary>
        /// <param name="themeService">The theme service.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="hostScreen">The screen that will host this ViewModel.</param>
        public ThemeSettingsViewModel(IThemeService themeService, ISettingsService settingsService, IScreen hostScreen)
            : base(hostScreen)
        {
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _isWindowsPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);        // Initialize properties
        _useDarkTheme = _themeService.CurrentTheme == ThemeVariant.Dark;

        // Only use system theme on Windows
        _followSystemTheme = _isWindowsPlatform && _themeService.FollowSystemTheme;

            // Initialize command with RxApp.MainThreadScheduler to ensure UI thread execution
            ApplyThemeCommand = ReactiveCommand.Create(
                () =>
                {
                    ApplyTheme();
                    return Unit.Default;
                },
                outputScheduler: RxApp.MainThreadScheduler
            );        // Monitor property changes but don't auto-apply
        // Ensure UI property changes are on the main thread
        this.WhenAnyValue(x => x.UseDarkTheme)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(value =>
            {
                _settingsService.UseDarkTheme = value;
            });

        this.WhenAnyValue(x => x.FollowSystemTheme)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(value =>
            {
                // Only allow system theme following on Windows
                if (!_isWindowsPlatform && value)
                {
                    // Silently ignore attempts to follow system theme on non-Windows
                    this.RaiseAndSetIfChanged(ref _followSystemTheme, false);
                    return;
                }
                _settingsService.UseSystemTheme = value;
            });
        }

        /// <summary>
        /// Gets or sets whether to use the dark theme.
        /// </summary>
        public bool UseDarkTheme
        {
            get => _useDarkTheme;
            set => this.RaiseAndSetIfChanged(ref _useDarkTheme, value);
        }

        /// <summary>
        /// Gets or sets whether to follow the system theme.
        /// </summary>
        public bool FollowSystemTheme
        {
            get => _followSystemTheme;
            set => this.RaiseAndSetIfChanged(ref _followSystemTheme, value);
        }

        /// <summary>
        /// Gets whether the current platform is Windows.
        /// </summary>
        public bool IsWindowsPlatform => _isWindowsPlatform;

        /// <summary>
        /// Command to apply the theme settings.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ApplyThemeCommand { get; }

        private void ApplyTheme()
        {
            try
            {
                // Make sure we're on the UI thread - use the dispatcher if we're not
                if (!Dispatcher.UIThread.CheckAccess())
                {
                    // Instead of executing directly, dispatch to the UI thread
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        ApplyThemeInternal();
                    });
                }
                else
                {
                    // Already on UI thread, execute directly
                    ApplyThemeInternal();
                }
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme application thread error: {ex.Message}");
            }
            catch (System.Threading.ThreadStateException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Thread state error: {ex.Message}");
            }
            catch (System.Threading.SynchronizationLockException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Synchronization lock error: {ex.Message}");
            }
        }        private void ApplyThemeInternal()
        {
            try
            {
                if (_followSystemTheme)
                {
                    // Only set follow system theme if on Windows
                    // On non-Windows, the theme service will fall back to user preference
                    _themeService.SetFollowSystemTheme(true);
                }
                else
                {
                    _themeService.SetFollowSystemTheme(false);
                    _themeService.SetTheme(_useDarkTheme);
                }
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme operation error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme argument error: {ex.Message}");
            }
        }
    }
}
