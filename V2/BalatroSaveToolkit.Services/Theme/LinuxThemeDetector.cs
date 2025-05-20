using System;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Styling;

namespace BalatroSaveToolkit.Services.Theme
{
    /// <summary>
    /// Linux-specific implementation of theme detection.
    /// </summary>
    public class LinuxThemeDetector
    {
        private readonly ThemeService _themeService;
        private Timer _themeCheckTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxThemeDetector"/> class.
        /// </summary>
        /// <param name="themeService">The theme service instance.</param>
        public LinuxThemeDetector(ThemeService themeService)
        {
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        }

        /// <summary>
        /// Detects if Linux is using dark mode.
        /// </summary>
        /// <returns>True if Linux is using dark mode, false otherwise.</returns>
        public bool IsDarkModeEnabled()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new PlatformNotSupportedException("This method is only supported on Linux");
            }

            try
            {
                // On Linux, we need to check several desktop environments
                // This covers the most common desktop environments: GNOME, KDE, XFCE

                // 1. GNOME/GTK
                var result = CheckGnomeTheme();
                if (result.HasValue)
                {
                    return result.Value;
                }

                // 2. KDE Plasma
                result = CheckKdePlasmaTheme();
                if (result.HasValue)
                {
                    return result.Value;
                }

                // 3. XFCE
                result = CheckXfceTheme();
                if (result.HasValue)
                {
                    return result.Value;
                }

                // No theme detection method succeeded
                return false;
            }
            catch
            {
                // If detection fails, default to light theme
                return false;
            }
        }

        private bool? CheckGnomeTheme()
        {
            try
            {
                using var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "gsettings",
                    Arguments = "get org.gnome.desktop.interface color-scheme",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd().Trim().ToLower();
                process.WaitForExit();

                if (output.Contains("dark") || output.Contains("prefer-dark"))
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(output))
                {
                    return false;
                }
                
                // Alternative check for older GNOME versions
                process.StartInfo.Arguments = "get org.gnome.desktop.interface gtk-theme";
                process.Start();
                output = process.StandardOutput.ReadToEnd().Trim().ToLower();
                process.WaitForExit();

                if (output.Contains("dark") || output.Contains("adwaita-dark"))
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(output))
                {
                    return false;
                }
            }
            catch
            {
                // Ignore error and continue to next check method
            }

            return null;
        }

        private bool? CheckKdePlasmaTheme()
        {
            try
            {
                using var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "kreadconfig5",
                    Arguments = "--group General --key ColorScheme",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd().Trim().ToLower();
                process.WaitForExit();

                if (output.Contains("breezedark") || output.Contains("dark") || output.Contains("black"))
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(output))
                {
                    return false;
                }
            }
            catch
            {
                // Ignore error and continue to next check method
            }

            return null;
        }

        private bool? CheckXfceTheme()
        {
            try
            {
                using var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "xfconf-query",
                    Arguments = "-c xsettings -p /Net/ThemeName",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd().Trim().ToLower();
                process.WaitForExit();

                if (output.Contains("dark") || output.Contains("-dark"))
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(output))
                {
                    return false;
                }
            }
            catch
            {
                // Ignore error
            }

            return null;
        }

        /// <summary>
        /// Starts monitoring for Linux theme changes.
        /// </summary>
        public void StartMonitoring()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new PlatformNotSupportedException("This method is only supported on Linux");
            }

            // Check for theme changes every 5 seconds
            _themeCheckTimer = new Timer(CheckForThemeChanges, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Stops monitoring for Linux theme changes.
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
