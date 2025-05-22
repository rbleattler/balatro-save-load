using System;
using Avalonia.Styling;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Service interface for theme management.
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Event fired when the theme changes.
        /// </summary>
        event EventHandler<ThemeChangedEventArgs> ThemeChanged;

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        ThemeVariant CurrentTheme { get; }

        /// <summary>
        /// Gets whether the system is currently in dark mode.
        /// </summary>
        bool IsSystemInDarkMode { get; }

        /// <summary>
        /// Gets or sets whether to follow the system theme.
        /// </summary>
        bool FollowSystemTheme { get; }

        /// <summary>
        /// Sets the theme to light or dark.
        /// </summary>
        /// <param name="isDark">True for dark theme, false for light theme.</param>
        void SetTheme(bool isDark);

        /// <summary>
        /// Sets the theme to follow the system theme.
        /// </summary>
        /// <param name="followSystem">True to follow system theme, false to use the application's setting.</param>
        void SetFollowSystemTheme(bool followSystem);

        /// <summary>
        /// Initializes the theme service.
        /// </summary>
        void Initialize();
    }
}
