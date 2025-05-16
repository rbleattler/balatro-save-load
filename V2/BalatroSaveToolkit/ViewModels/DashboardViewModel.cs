using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IFileService _fileService;
        private readonly IGameProcessService _gameProcessService;
        private readonly ISaveService _saveService;
        private readonly ILogService _logService;
        
        [ObservableProperty]
        private string _statusMessage = "Welcome to Balatro Save Toolkit";
        
        [ObservableProperty]
        private string _gameRunningStatus = "Game not detected";
        
        [ObservableProperty]
        private ObservableCollection<ActivityLogItem> _recentActivities = new();
        
        public DashboardViewModel(
            IFileService fileService,
            IGameProcessService gameProcessService,
            ISaveService saveService,
            ILogService logService)
        {
            _fileService = fileService;
            _gameProcessService = gameProcessService;
            _saveService = saveService;
            _logService = logService;
            
            Title = "Dashboard";
        }
        
        public async Task InitializeAsync()
        {
            // Check if game is running
            bool isGameRunning = await _gameProcessService.IsGameRunningAsync();
            GameRunningStatus = isGameRunning ? "Game is currently running" : "Game not detected";
            
            // Load recent activities
            await LoadRecentActivitiesAsync();
        }
        
        private async Task LoadRecentActivitiesAsync()
        {
            try
            {
                var activities = await _logService.GetRecentActivitiesAsync(10);
                RecentActivities.Clear();
                
                foreach (var activity in activities)
                {
                    RecentActivities.Add(activity);
                }
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Failed to load recent activities", ex);
            }
        }
        
        [RelayCommand]
        private async Task BackupSaveAsync()
        {
            try
            {
                StatusMessage = "Backing up current save...";
                await _saveService.BackupCurrentSaveAsync();
                StatusMessage = "Backup completed successfully";
                
                await _logService.LogInfoAsync("Manual backup created");
                await LoadRecentActivitiesAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = "Backup failed";
                await _logService.LogErrorAsync("Backup failed", ex);
            }
        }
        
        [RelayCommand]
        private async Task ViewSavesAsync()
        {
            // Navigate to the SaveViewerPage
            await Shell.Current.GoToAsync("//SaveViewer");
        }
        
        [RelayCommand]
        private async Task RestoreBackupAsync()
        {
            // Navigate to the SaveViewerPage with restore mode
            await Shell.Current.GoToAsync("//SaveViewer?mode=restore");
        }
        
        [RelayCommand]
        private async Task LoadCustomSaveAsync()
        {
            try
            {
                string filePath = await _fileService.PickFileAsync("Select Balatro save file", "*.txt");
                if (string.IsNullOrEmpty(filePath))
                    return;
                    
                StatusMessage = "Loading custom save...";
                await _saveService.LoadCustomSaveAsync(filePath);
                StatusMessage = "Custom save loaded successfully";
                
                await _logService.LogInfoAsync($"Loaded custom save from {filePath}");
                await LoadRecentActivitiesAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to load custom save";
                await _logService.LogErrorAsync("Loading custom save failed", ex);
            }
        }
    }
}
