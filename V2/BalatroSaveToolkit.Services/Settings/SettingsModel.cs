using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BalatroSaveToolkit.Services.Settings
{
    /// <summary>
    /// Model class for storing application settings.
    /// </summary>
    internal class SettingsModel
    {
        /// <summary>
        /// Gets or sets whether auto-save is enabled.
        /// </summary>
        [JsonPropertyName("autoSaveEnabled")]
        public bool AutoSaveEnabled { get; set; }

        /// <summary>
        /// Gets or sets the auto-save interval in minutes.
        /// </summary>
        [JsonPropertyName("autoSaveIntervalMinutes")]
        public double AutoSaveIntervalMinutes { get; set; }

        /// <summary>
        /// Gets or sets the selected profile number.
        /// </summary>
        [JsonPropertyName("selectedProfileNumber")]
        public int SelectedProfileNumber { get; set; }

        /// <summary>
        /// Gets or sets the cleanup time span for old backup files.
        /// </summary>
        [JsonPropertyName("cleanupTimeSpan")]
        public TimeSpan CleanupTimeSpan { get; set; }

        /// <summary>
        /// Gets or sets whether to use the system theme.
        /// </summary>
        [JsonPropertyName("useSystemTheme")]
        public bool UseSystemTheme { get; set; }

        /// <summary>
        /// Gets or sets whether to use the dark theme when system theme is not used.
        /// </summary>
        [JsonPropertyName("useDarkTheme")]
        public bool UseDarkTheme { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of custom settings.
        /// </summary>
        [JsonPropertyName("customSettings")]
        public Dictionary<string, object> CustomSettings { get; set; } = new Dictionary<string, object>();
    }
}
