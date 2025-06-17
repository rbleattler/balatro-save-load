using System.Configuration;
using System.Data;
using System.Windows;

namespace BalatroSaveAndLoad
{
    public partial class App : Application
    {
        public static event EventHandler? ThemeChanged;

#pragma warning disable WPF0001 // Type is for evaluation purposes only and is subject to change or removal in future updates
        public static void ToggleTheme()
        {
            var app = Current as App;
            if (app != null)
            {
                // Toggle between System and None
                var currentTheme = app.ThemeMode;
                var newTheme = currentTheme == ThemeMode.System ? ThemeMode.None : ThemeMode.System;

                app.ThemeMode = newTheme;

                // Notify all windows about the theme change
                ThemeChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static ThemeMode GetCurrentTheme()
        {
            var app = Current as App;
            return app?.ThemeMode ?? ThemeMode.System;
        }
#pragma warning restore WPF0001
    }
}
