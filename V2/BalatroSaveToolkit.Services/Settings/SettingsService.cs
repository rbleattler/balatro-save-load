using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Services;
using BalatroSaveToolkit.Services.FileSystem;

namespace BalatroSaveToolkit.Services.Settings
{
    /// <summary>
    /// Implementation of the settings service that stores settings in JSON format.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly string _settingsFilePath;
        private SettingsModel _currentSettings;

        /// <inheritdoc/>
        public bool AutoSaveEnabled
        {
            get => _currentSettings.AutoSaveEnabled;
            set
            {
                _currentSettings.AutoSaveEnabled = value;
                SaveSettingsAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public double AutoSaveIntervalMinutes
        {
            get => _currentSettings.AutoSaveIntervalMinutes;
            set
            {
                _currentSettings.AutoSaveIntervalMinutes = value;
                SaveSettingsAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public int SelectedProfileNumber
        {
            get => _currentSettings.SelectedProfileNumber;
            set
            {
                if (value < 1 || value > 4)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Profile number must be between 1 and 4.");
                }
                _currentSettings.SelectedProfileNumber = value;
                SaveSettingsAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public TimeSpan CleanupTimeSpan
        {
            get => _currentSettings.CleanupTimeSpan;
            set
            {
                _currentSettings.CleanupTimeSpan = value;
                SaveSettingsAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public bool UseSystemTheme
        {
            get => _currentSettings.UseSystemTheme;
            set
            {
                _currentSettings.UseSystemTheme = value;
                SaveSettingsAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public bool UseDarkTheme
        {
            get => _currentSettings.UseDarkTheme;
            set
            {
                _currentSettings.UseDarkTheme = value;
                SaveSettingsAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        /// <param name="fileSystemService">The file system service.</param>
        public SettingsService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _settingsFilePath = Path.Combine(fileSystemService.ApplicationDataDirectory, "settings.json");
            _currentSettings = LoadSettingsSync();
        }

        /// <inheritdoc/>
        public async Task<T> GetSettingAsync<T>(string key, T defaultValue = default)
        {
            if (_currentSettings.CustomSettings.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }

            return defaultValue;
        }

        /// <inheritdoc/>
        public async Task SetSettingAsync<T>(string key, T value)
        {
            _currentSettings.CustomSettings[key] = value;
            await SaveSettingsAsync();
        }

        /// <inheritdoc/>
        public async Task ResetToDefaultsAsync()
        {
            _currentSettings = CreateDefaultSettings();
            await SaveSettingsAsync();
        }

        /// <inheritdoc/>
        public Task LoadAsync()
        {
            _currentSettings = LoadSettingsSync();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SaveAsync()
        {
            return SaveSettingsAsync();
        }

        private SettingsModel LoadSettingsSync()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<SettingsModel>(json);
                    if (settings != null)
                    {
                        return settings;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }

            return CreateDefaultSettings();
        }

        private async Task SaveSettingsAsync()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(_currentSettings, options);
                await File.WriteAllTextAsync(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private SettingsModel CreateDefaultSettings()
        {
            return new SettingsModel
            {
                AutoSaveEnabled = false,
                AutoSaveIntervalMinutes = 5,
                SelectedProfileNumber = 1,
                CleanupTimeSpan = TimeSpan.FromDays(7),
                UseSystemTheme = true,
                UseDarkTheme = false
            };
        }
    }
}
