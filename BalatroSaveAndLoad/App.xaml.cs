using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Win32;
using System;
using System.Windows.Threading;

namespace BalatroSaveAndLoad {
    public partial class App : Application {
        private DispatcherTimer? _themeCheckTimer;
        private bool? _lastIsDark = null;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            // Detect and apply system theme at startup
            ApplySystemTheme();

            // Start a timer to check for theme changes every 2 seconds
            _themeCheckTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _themeCheckTimer.Tick += ThemeCheckTimer_Tick;
            _themeCheckTimer.Start();
        }

        private void ThemeCheckTimer_Tick(object? sender, EventArgs e) { ApplySystemTheme(); }

        private void ApplySystemTheme() {
            var isDark = IsSystemInDarkMode();
            if (_lastIsDark == isDark)
                return;
            _lastIsDark = isDark;
            var dicts = Resources.MergedDictionaries;
            dicts.Clear();
            dicts.Add(
                      isDark
                          ? new ResourceDictionary { Source = new Uri("Themes/Dark.xaml", UriKind.Relative) }
                          : new ResourceDictionary { Source = new Uri("Themes/Light.xaml", UriKind.Relative) }
                     );
        }

        private static bool IsSystemInDarkMode() {
            try {
                using var key =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = key?.GetValue("AppsUseLightTheme");
                if (value is int i) return i == 0;
            }
            catch {
                // Handle exceptions if necessary
            }

            return false; // Default to light
        }
    }
}