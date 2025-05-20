using System;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Styling;

namespace BalatroSaveToolkit.Services.Theme
{
    /// <summary>
    /// macOS-specific implementation of theme detection.
    /// </summary>
    public class MacOsThemeDetector
    {
        private readonly ThemeService _themeService;
        private Timer _themeCheckTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacOsThemeDetector"/> class.
        /// </summary>
        /// <param name="themeService">The theme service instance.</param>
        public MacOsThemeDetector(ThemeService themeService)
        {
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        }

        /// <summary>
        /// Detects if macOS is using dark mode.
        /// </summary>
        /// <returns>True if macOS is using dark mode, false otherwise.</returns>
        public bool IsDarkModeEnabled()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new PlatformNotSupportedException("This method is only supported on macOS");
            }

            try
            {
                // macOS Appearance API is accessed via ObjC runtime
                // For Avalonia, we use a simplified approach with process execution
                
                // Use the 'defaults' command to read the system appearance setting
                // defaults read -g AppleInterfaceStyle
                // If the output is "Dark", then dark mode is enabled
                // If the command returns an error or empty string, then light mode is enabled
                
                using var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "defaults",
                    Arguments = "read -g AppleInterfaceStyle",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                
                process.Start();
                string output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();
                
                return output.Equals("Dark", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // If the command fails, default to light theme
                return false;
            }
        }

        /// <summary>
        /// Starts monitoring for macOS theme changes.
        /// </summary>
        public void StartMonitoring()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new PlatformNotSupportedException("This method is only supported on macOS");
            }

            // Check for theme changes every 5 seconds
            _themeCheckTimer = new Timer(CheckForThemeChanges, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Stops monitoring for macOS theme changes.
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
