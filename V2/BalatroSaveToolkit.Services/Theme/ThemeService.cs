using System;
using System.Runtime.InteropServices;
using Avalonia.Styling;
using Avalonia.Threading;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Services.Theme
{
    /// <summary>
    /// Implementation of the theme service with platform-specific theme detection.
    /// </summary>
    public class ThemeService : IThemeService
    {
        private readonly ISettingsService _settingsService;
        private bool _followSystemTheme;
        private ThemeVariant _currentTheme;
        private bool _isInitialized;

        // Only keep Windows detector since we're only supporting system theme on Windows
        private WindowsThemeDetector? _windowsThemeDetector;

        /// <summary>
        /// Event fired when the theme changes.
        /// </summary>
        public event EventHandler<ThemeChangedEventArgs> ThemeChanged = delegate { };

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        public ThemeVariant CurrentTheme => _currentTheme;

        /// <summary>
        /// Gets whether the system is currently in dark mode.
        /// Only accurate on Windows - defaults to user preference on other platforms.
        /// </summary>
        public bool IsSystemInDarkMode { get; private set; }

        /// <summary>
        /// Gets or sets whether to follow the system theme.
        /// Only supported on Windows - other platforms will use user preference.
        /// </summary>
        public bool FollowSystemTheme
        {
            get => _followSystemTheme;
            set => SetFollowSystemTheme(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeService"/> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        public ThemeService(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _currentTheme = ThemeVariant.Default;
            _followSystemTheme = _settingsService.UseSystemTheme;
        }

        /// <summary>
        /// Initializes the theme service.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            // Detect initial system theme (only works on Windows)
            UpdateSystemThemeState();

            // Set initial theme
            if (_followSystemTheme && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _currentTheme = IsSystemInDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
            }
            else
            {
                _currentTheme = _settingsService.UseDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
            }

            // Start monitoring for system theme changes only on Windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                StartSystemThemeMonitoring();
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Sets the theme to light or dark.
        /// </summary>
        /// <param name="isDark">True for dark theme, false for light theme.</param>
        public void SetTheme(bool isDark)
        {
            _settingsService.UseDarkTheme = isDark;

            if (!_followSystemTheme || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var newTheme = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
                if (_currentTheme != newTheme)
                {
                    _currentTheme = newTheme;
                    RaiseThemeChangedEvent();
                }
            }
        }

        /// <summary>
        /// Sets the theme to follow the system theme.
        /// Only effective on Windows.
        /// </summary>
        /// <param name="followSystem">True to follow system theme, false to use the application's setting.</param>
        public void SetFollowSystemTheme(bool followSystem)
        {
            _followSystemTheme = followSystem;
            _settingsService.UseSystemTheme = followSystem;

            if (followSystem && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var systemTheme = IsSystemInDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
                if (_currentTheme != systemTheme)
                {
                    _currentTheme = systemTheme;
                    RaiseThemeChangedEvent();
                }
            }
            else
            {
                var userTheme = _settingsService.UseDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
                if (_currentTheme != userTheme)
                {
                    _currentTheme = userTheme;
                    RaiseThemeChangedEvent();
                }
            }
        }

        /// <summary>
        /// Updates the system theme state by detecting the current system theme.
        /// Only effective on Windows.
        /// </summary>
        internal void UpdateSystemThemeState()
        {
            IsSystemInDarkMode = DetectSystemTheme();

            if (_followSystemTheme && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var newTheme = IsSystemInDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
                if (_currentTheme != newTheme)
                {
                    _currentTheme = newTheme;
                    RaiseThemeChangedEvent();
                }
            }
        }

        /// <summary>
        /// Detects the system theme based on the current platform.
        /// Only actually detects on Windows; other platforms just use user preference.
        /// </summary>
        /// <returns>True if the system is using a dark theme, false otherwise.</returns>
        private bool DetectSystemTheme()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return DetectWindowsTheme();
            }

            // For macOS and Linux, just use the user preference instead of system detection
            return _settingsService.UseDarkTheme;
        }

        /// <summary>
        /// Starts monitoring system theme changes based on the current platform.
        /// Only Windows will actually monitor system theme.
        /// </summary>
        private void StartSystemThemeMonitoring()
        {
            // Only monitor Windows theme changes
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                StartWindowsThemeMonitoring();
            }
        }

        private bool DetectWindowsTheme()
        {
            try
            {
                _windowsThemeDetector ??= new WindowsThemeDetector(this);
                return _windowsThemeDetector.IsDarkModeEnabled();
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                // Fallback to settings if detection fails
                return _settingsService.UseDarkTheme;
            }
        }

        private void StartWindowsThemeMonitoring()
        {
            try
            {
                _windowsThemeDetector ??= new WindowsThemeDetector(this);
                _windowsThemeDetector.StartMonitoring();
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                // Just log the error and continue without monitoring
                System.Diagnostics.Debug.WriteLine($"Failed to start Windows theme monitoring: {ex.Message}");
            }
        }

        /// <summary>
        /// Raises the ThemeChanged event ensuring it runs on the UI thread.
        /// </summary>
        private void RaiseThemeChangedEvent()
        {
            if (ThemeChanged == null)
                return;

            // If we're on the UI thread, invoke directly
            if (Dispatcher.UIThread.CheckAccess())
            {
                ThemeChanged.Invoke(this, new ThemeChangedEventArgs(_currentTheme));
            }
            else
            {
                // Otherwise, dispatch to the UI thread
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ThemeChanged.Invoke(this, new ThemeChangedEventArgs(_currentTheme));
                });
            }
        }
    }
}
