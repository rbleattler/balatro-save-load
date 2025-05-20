using System;
using Microsoft.Win32;
using System.Threading;
using System.Runtime.InteropServices;
using Avalonia.Styling;

namespace BalatroSaveToolkit.Services.Theme
{
    /// <summary>
    /// Windows-specific implementation of theme detection.
    /// </summary>
    public class WindowsThemeDetector
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";
        private readonly ThemeService _themeService;
        private Timer _themeCheckTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsThemeDetector"/> class.
        /// </summary>
        /// <param name="themeService">The theme service instance.</param>
        public WindowsThemeDetector(ThemeService themeService)
        {
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        }

        /// <summary>
        /// Detects if Windows is using dark mode.
        /// </summary>
        /// <returns>True if Windows is using dark mode, false otherwise.</returns>
        public bool IsDarkModeEnabled()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException("This method is only supported on Windows");
            }

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
                var value = key?.GetValue(RegistryValueName);
                return value is int i && i == 0; // 0 = Dark mode, 1 = Light mode
            }
            catch
            {
                // If registry access fails, default to light theme
                return false;
            }
        }

        /// <summary>
        /// Starts monitoring for Windows theme changes.
        /// </summary>
        public void StartMonitoring()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException("This method is only supported on Windows");
            }

            // Check for theme changes every 2 seconds
            _themeCheckTimer = new Timer(CheckForThemeChanges, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Stops monitoring for Windows theme changes.
        /// </summary>
        public void StopMonitoring()
        {
            _themeCheckTimer?.Dispose();
            _themeCheckTimer = null;
        }

        private void CheckForThemeChanges(object state)
        {
            try
            {
                var isDarkMode = IsDarkModeEnabled();
                if (_themeService.IsSystemInDarkMode != isDarkMode)
                {
                    // Use reflection to call the private UpdateSystemThemeState method
                    var methodInfo = typeof(ThemeService).GetMethod("UpdateSystemThemeState", 
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    
                    methodInfo?.Invoke(_themeService, null);
                }
            }
            catch
            {
                // Ignore errors during theme checking
            }
        }
    }
}
