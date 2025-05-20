using System;
using System.Runtime.InteropServices;
using Avalonia.Styling;
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

        private WindowsThemeDetector? _windowsThemeDetector;
        private MacOsThemeDetector? _macOsThemeDetector;
        private LinuxThemeDetector? _linuxThemeDetector;

        /// <summary>
        /// Event fired when the theme changes.
        /// </summary>
        public event EventHandler<ThemeVariant>? ThemeChanged;

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        public ThemeVariant CurrentTheme => _currentTheme;

        /// <summary>
        /// Gets whether the system is currently in dark mode.
        /// </summary>
        public bool IsSystemInDarkMode { get; private set; }

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

            // Detect initial system theme
            UpdateSystemThemeState();

            // Set initial theme
            if (_followSystemTheme)
            {
                _currentTheme = IsSystemInDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
            }
            else
            {
                _currentTheme = _settingsService.UseDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
            }

            // Start monitoring for system theme changes based on the platform
            StartSystemThemeMonitoring();

            _isInitialized = true;
        }

        /// <summary>
        /// Sets the theme to light or dark.
        /// </summary>
        /// <param name="isDark">True for dark theme, false for light theme.</param>
        public void SetTheme(bool isDark)
        {
            _settingsService.UseDarkTheme = isDark;

            if (!_followSystemTheme)
            {
                var newTheme = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
                if (_currentTheme != newTheme)
                {
                    _currentTheme = newTheme;
                    ThemeChanged?.Invoke(this, _currentTheme);
                }
            }
        }

        /// <summary>
        /// Sets the theme to follow the system theme.
        /// </summary>
        /// <param name="followSystem">True to follow system theme, false to use the application's setting.</param>
        public void SetFollowSystemTheme(bool followSystem)
        {
            _followSystemTheme = followSystem;
            _settingsService.UseSystemTheme = followSystem;

            if (followSystem)
            {
                var systemTheme = IsSystemInDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
                if (_currentTheme != systemTheme)
                {
                    _currentTheme = systemTheme;
                    ThemeChanged?.Invoke(this, _currentTheme);
                }
            }
            else
            {
                var userTheme = _settingsService.UseDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
                if (_currentTheme != userTheme)
                {
                    _currentTheme = userTheme;
                    ThemeChanged?.Invoke(this, _currentTheme);
                }
            }
        }

        /// <summary>
        /// Updates the system theme state by detecting the current system theme.
        /// </summary>
        internal void UpdateSystemThemeState()
        {
            IsSystemInDarkMode = DetectSystemTheme();

            if (_followSystemTheme)
            {
                var newTheme = IsSystemInDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
                if (_currentTheme != newTheme)
                {
                    _currentTheme = newTheme;
                    ThemeChanged?.Invoke(this, _currentTheme);
                }
            }
        }

        /// <summary>
        /// Detects the system theme based on the current platform.
        /// </summary>
        /// <returns>True if the system is using a dark theme, false otherwise.</returns>
        private bool DetectSystemTheme()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return DetectWindowsTheme();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return DetectMacOsTheme();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return DetectLinuxTheme();
            }

            // Default to light theme if platform is not supported
            return false;
        }

        /// <summary>
        /// Starts monitoring system theme changes based on the current platform.
        /// </summary>
        private void StartSystemThemeMonitoring()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                StartWindowsThemeMonitoring();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                StartMacOsThemeMonitoring();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                StartLinuxThemeMonitoring();
            }
        }

        private bool DetectWindowsTheme()
        {
            try
            {
                _windowsThemeDetector ??= new WindowsThemeDetector(this);
                return _windowsThemeDetector.IsDarkModeEnabled();
            }
            catch
            {
                // Fallback to settings if detection fails
                return _settingsService.UseDarkTheme;
            }
        }

        private void StartWindowsThemeMonitoring()
        {
            _windowsThemeDetector ??= new WindowsThemeDetector(this);
            _windowsThemeDetector.StartMonitoring();
        }

        private bool DetectMacOsTheme()
        {
            try
            {
                _macOsThemeDetector ??= new MacOsThemeDetector(this);
                return _macOsThemeDetector.IsDarkModeEnabled();
            }
            catch
            {
                // Fallback to settings if detection fails
                return _settingsService.UseDarkTheme;
            }
        }

        private void StartMacOsThemeMonitoring()
        {
            _macOsThemeDetector ??= new MacOsThemeDetector(this);
            _macOsThemeDetector.StartMonitoring();
        }

        private bool DetectLinuxTheme()
        {
            try
            {
                _linuxThemeDetector ??= new LinuxThemeDetector(this);
                return _linuxThemeDetector.IsDarkModeEnabled();
            }
            catch
            {
                // Fallback to settings if detection fails
                return _settingsService.UseDarkTheme;
            }
        }

        private void StartLinuxThemeMonitoring()
        {
            _linuxThemeDetector ??= new LinuxThemeDetector(this);
            _linuxThemeDetector.StartMonitoring();
        }
    }
}
