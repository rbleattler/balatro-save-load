using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Theme
{
    /// <summary>
    /// Manager class for handling theme resources.
    /// </summary>
    public static class ThemeManager
    {
        private static ResourceDictionary? _lightTheme;
        private static ResourceDictionary? _darkTheme;

        /// <summary>
        /// Initializes theme resources.
        /// </summary>
        /// <param name="application">The application instance.</param>
        public static void Initialize(Application application)
        {
            _lightTheme = new ResourceDictionary
            {
                Source = new Uri("avares://BalatroSaveToolkit/Themes/LightTheme.axaml")
            };
            
            _darkTheme = new ResourceDictionary
            {
                Source = new Uri("avares://BalatroSaveToolkit/Themes/DarkTheme.axaml")
            };
        }

        /// <summary>
        /// Applies a theme to the application.
        /// </summary>
        /// <param name="application">The application instance.</param>
        /// <param name="isDarkTheme">Whether to apply the dark theme.</param>
        public static void ApplyTheme(Application application, bool isDarkTheme)
        {
            var resources = application.Resources;
            resources.MergedDictionaries.Clear();
            
            if (isDarkTheme)
            {
                resources.MergedDictionaries.Add(_darkTheme!);
                application.RequestedThemeVariant = ThemeVariant.Dark;
            }
            else
            {
                resources.MergedDictionaries.Add(_lightTheme!);
                application.RequestedThemeVariant = ThemeVariant.Light;
            }
        }
    }
}
