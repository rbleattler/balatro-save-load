using System;
using System.Threading.Tasks;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Service interface for application settings management.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets or sets whether auto-save is enabled.
        /// </summary>
        bool AutoSaveEnabled { get; set; }

        /// <summary>
        /// Gets or sets the auto-save interval in minutes.
        /// </summary>
        double AutoSaveIntervalMinutes { get; set; }

        /// <summary>
        /// Gets or sets the selected profile number (1-based).
        /// </summary>
        int SelectedProfileNumber { get; set; }

        /// <summary>
        /// Gets or sets the cleanup time span for old backup files.
        /// </summary>
        TimeSpan CleanupTimeSpan { get; set; }

        /// <summary>
        /// Gets or sets whether the application should use system theme.
        /// </summary>
        bool UseSystemTheme { get; set; }

        /// <summary>
        /// Gets or sets whether the application should use dark theme.
        /// </summary>
        bool UseDarkTheme { get; set; }

        /// <summary>
        /// Loads settings from persistent storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous load operation.</returns>
        Task LoadAsync();

        /// <summary>
        /// Saves settings to persistent storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task SaveAsync();
    }
}
