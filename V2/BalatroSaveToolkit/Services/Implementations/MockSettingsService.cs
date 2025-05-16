using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations
{
    public class MockSettingsService : ISettingsService
    {
        private readonly IFileService _fileService;
        private AppSettings _settings;

        public MockSettingsService(IFileService fileService)
        {
            _fileService = fileService;
            _settings = new AppSettings
            {
                GamePath = "C:\\Program Files\\Steam\\steamapps\\common\\Balatro\\Balatro.exe",
                SavesPath = "C:\\Users\\User\\AppData\\Roaming\\Balatro\\SaveData",
                BackupPath = "C:\\Users\\User\\AppData\\Roaming\\BalatroSaveToolkit\\Backups",
                AutoDetectGame = true,
                AutoBackupOnStart = true,
                AutoBackupOnClose = true,
                MaxBackupsToKeep = 20,
                StartWithWindows = false,
                MinimizeToTray = true,
                LogLevel = "Info"
            };
        }

        public async Task<AppSettings> GetSettingsAsync()
        {
            return _settings;
        }

        public async Task<string> GetSettingAsync(string key, string defaultValue = "")
        {
            switch (key)
            {
                case "GamePath":
                    return _settings.GamePath;
                case "SavesPath":
                    return _settings.SavesPath;
                case "BackupPath":
                    return _settings.BackupPath;
                case "LogLevel":
                    return _settings.LogLevel;
                default:
                    return defaultValue;
            }
        }

        public async Task ResetToDefaultsAsync()
        {
            _settings = new AppSettings
            {
                GamePath = "C:\\Program Files\\Steam\\steamapps\\common\\Balatro\\Balatro.exe",
                SavesPath = "C:\\Users\\User\\AppData\\Roaming\\Balatro\\SaveData",
                BackupPath = "C:\\Users\\User\\AppData\\Roaming\\BalatroSaveToolkit\\Backups",
                AutoDetectGame = true,
                AutoBackupOnStart = true,
                AutoBackupOnClose = true,
                MaxBackupsToKeep = 20,
                StartWithWindows = false,
                MinimizeToTray = true,
                LogLevel = "Info"
            };
        }

        public async Task SaveSettingsAsync(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task SetSettingAsync(string key, string value)
        {
            switch (key)
            {
                case "GamePath":
                    _settings.GamePath = value;
                    break;
                case "SavesPath":
                    _settings.SavesPath = value;
                    break;
                case "BackupPath":
                    _settings.BackupPath = value;
                    break;
                case "LogLevel":
                    _settings.LogLevel = value;
                    break;
            }
        }
    }
}
