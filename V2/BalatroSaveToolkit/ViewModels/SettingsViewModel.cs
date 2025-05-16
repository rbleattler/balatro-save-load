using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IFileService _fileService;
        private readonly ILogService _logService;
        
        [ObservableProperty]
        private string _gamePath;
        
        [ObservableProperty]
        private string _savesPath;
        
        [ObservableProperty]
        private string _backupPath;
        
        [ObservableProperty]
        private bool _autoDetectGame;
        
        [ObservableProperty]
        private bool _autoBackupOnStart;
        
        [ObservableProperty]
        private bool _autoBackupOnClose;
        
        [ObservableProperty]
        private int _maxBackupsToKeep;
        
        [ObservableProperty]
        private bool _startWithWindows;
        
        [ObservableProperty]
        private bool _minimizeToTray;
        
        [ObservableProperty]
        private ObservableCollection<string> _logLevels = new ObservableCollection<string>
        {
            "Debug",
            "Info",
            "Warning",
            "Error"
        };
        
        [ObservableProperty]
        private string _selectedLogLevel;
        
        public SettingsViewModel(
            ISettingsService settingsService,
            IFileService fileService,
            ILogService logService)
        {
            _settingsService = settingsService;
            _fileService = fileService;
            _logService = logService;
            
            Title = "Settings";
        }
        
        public async Task LoadSettingsAsync()
        {
            var settings = await _settingsService.GetSettingsAsync();
            
            GamePath = settings.GamePath;
            SavesPath = settings.SavesPath;
            BackupPath = settings.BackupPath;
            AutoDetectGame = settings.AutoDetectGame;
            AutoBackupOnStart = settings.AutoBackupOnStart;
            AutoBackupOnClose = settings.AutoBackupOnClose;
            MaxBackupsToKeep = settings.MaxBackupsToKeep;
            StartWithWindows = settings.StartWithWindows;
            MinimizeToTray = settings.MinimizeToTray;
            SelectedLogLevel = settings.LogLevel;
        }
        
        [RelayCommand]
        private async Task BrowseGamePathAsync()
        {
            string path = await _fileService.PickFileAsync("Select Balatro executable", "*.exe");
            if (!string.IsNullOrEmpty(path))
            {
                GamePath = path;
            }
        }
        
        [RelayCommand]
        private async Task BrowseSavesPathAsync()
        {
            string path = await _fileService.PickFolderAsync("Select Balatro saves directory");
            if (!string.IsNullOrEmpty(path))
            {
                SavesPath = path;
            }
        }
        
        [RelayCommand]
        private async Task BrowseBackupPathAsync()
        {
            string path = await _fileService.PickFolderAsync("Select backup directory");
            if (!string.IsNullOrEmpty(path))
            {
                BackupPath = path;
            }
        }
        
        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            try
            {
                var settings = new AppSettings
                {
                    GamePath = GamePath,
                    SavesPath = SavesPath,
                    BackupPath = BackupPath,
                    AutoDetectGame = AutoDetectGame,
                    AutoBackupOnStart = AutoBackupOnStart,
                    AutoBackupOnClose = AutoBackupOnClose,
                    MaxBackupsToKeep = MaxBackupsToKeep,
                    StartWithWindows = StartWithWindows,
                    MinimizeToTray = MinimizeToTray,
                    LogLevel = SelectedLogLevel
                };
                
                await _settingsService.SaveSettingsAsync(settings);
                await _logService.LogInfoAsync("Settings saved successfully");
                
                // Show confirmation
                await Shell.Current.DisplayAlert("Settings", "Settings saved successfully", "OK");
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Failed to save settings", ex);
                await Shell.Current.DisplayAlert("Error", "Failed to save settings: " + ex.Message, "OK");
            }
        }
        
        [RelayCommand]
        private async Task ResetDefaultsAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Reset Settings", 
                "Are you sure you want to reset all settings to default values?", 
                "Yes", "No");
                
            if (confirm)
            {
                await _settingsService.ResetToDefaultsAsync();
                await LoadSettingsAsync();
                await _logService.LogInfoAsync("Settings reset to defaults");
            }
        }
    }
}
