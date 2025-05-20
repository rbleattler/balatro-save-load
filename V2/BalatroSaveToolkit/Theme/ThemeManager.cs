using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System;
using System.Diagnostics;

namespace BalatroSaveToolkit.Theme
{
    /// <summary>
    /// Manager class for handling theme resources.
    /// </summary>
    public static class ThemeManager
    {
        private static ResourceDictionary? _lightTheme;
        private static ResourceDictionary? _darkTheme;        /// <summary>
        /// Initializes theme resources.
        /// </summary>
        /// <param name="application">The application instance.</param>
        public static void Initialize(Application application)
        {
            ArgumentNullException.ThrowIfNull(application, nameof(application));

            try
            {
                Debug.WriteLine("Initializing theme resources");
                  // Load the theme resources directly using AvaloniaXamlLoader
                _lightTheme = AvaloniaXamlLoader.Load(new Uri("avares://BalatroSaveToolkit/Theme/LightTheme.axaml")) as ResourceDictionary;
                Debug.WriteLine($"Light theme loaded: {_lightTheme != null}");

                _darkTheme = AvaloniaXamlLoader.Load(new Uri("avares://BalatroSaveToolkit/Theme/DarkTheme.axaml")) as ResourceDictionary;
                Debug.WriteLine($"Dark theme loaded: {_darkTheme != null}");

                if (_lightTheme == null || _darkTheme == null)
                {
                    throw new InvalidOperationException("Failed to load theme resources");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing themes: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                throw; // Rethrow to make initialization issues visible
            }
        }

        /// <summary>
        /// Applies a theme to the application.
        /// </summary>
        /// <param name="application">The application instance.</param>
        /// <param name="isDarkTheme">Whether to apply the dark theme.</param>
        public static void ApplyTheme(Application application, bool isDarkTheme)
        {
            if (application == null)
                return;            try
            {
                Debug.WriteLine($"Applying theme: {(isDarkTheme ? "Dark" : "Light")}");

                // Check if themes are initialized
                if (_lightTheme == null || _darkTheme == null)
                {
                    Debug.WriteLine("Themes are not initialized properly!");
                    return;
                }

                // Get the application's merged dictionaries
                var resources = application.Resources;
                resources.MergedDictionaries.Clear();

                // Apply the selected theme
                if (isDarkTheme)
                {
                    Debug.WriteLine("Adding dark theme to resources");
                    resources.MergedDictionaries.Add(_darkTheme);
                    application.RequestedThemeVariant = ThemeVariant.Dark;
                }
                else
                {
                    Debug.WriteLine("Adding light theme to resources");
                    resources.MergedDictionaries.Add(_lightTheme);
                    application.RequestedThemeVariant = ThemeVariant.Light;
                }
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Invalid operation in theme application: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                // Don't rethrow - we want the app to continue even if theme fails
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"Invalid argument in theme application: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                // Don't rethrow - we want the app to continue even if theme fails
            }
        }
    }
}
