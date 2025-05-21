using System;
using Avalonia.Styling;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Event arguments for theme changes.
    /// </summary>
    public class ThemeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the new theme variant.
        /// </summary>
        public ThemeVariant Theme { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="theme">The new theme.</param>
        public ThemeChangedEventArgs(ThemeVariant theme)
        {
            Theme = theme;
        }
    }
}
