using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.ViewModels
{
    public partial class LogsViewModel : ObservableObject
    {
        private readonly ILogService _logService;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IFileSaver _fileSaver;

        [ObservableProperty]
        private ObservableCollection<ActivityLogItem> _allLogs = new();

        [ObservableProperty]
        private ObservableCollection<ActivityLogItem> _filteredLogs = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _selectedLogLevel = "All";

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now.AddDays(-7);

        [ObservableProperty]
        private ActivityLogItem? _selectedLogItem;

        public List<string> LogLevels { get; } = new() { "All", "DEBUG", "INFO", "WARN", "ERROR", "CRITICAL" };

        public LogsViewModel()
        {
            // Get services from DI
            _logService = IPlatformApplication.Current?.Services?.GetService<ILogService>()
                ?? throw new InvalidOperationException("ILogService not registered");

            _errorHandlingService = IPlatformApplication.Current?.Services?.GetService<IErrorHandlingService>()
                ?? throw new InvalidOperationException("IErrorHandlingService not registered");

            _fileSaver = IPlatformApplication.Current?.Services?.GetService<IFileSaver>()
                ?? throw new InvalidOperationException("IFileSaver not registered");
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilters();
        }

        partial void OnSelectedLogLevelChanged(string value)
        {
            ApplyFilters();
        }

        partial void OnStartDateChanged(DateTime value)
        {
            ApplyFilters();
        }

        [RelayCommand]
        public async Task LoadLogsAsync()
        {
            try
            {
                DateTime endDate = DateTime.Now;
                var logs = await _logService.GetLogEntriesAsync(StartDate, endDate);

                AllLogs = new ObservableCollection<ActivityLogItem>(logs);
                ApplyFilters();
            }
            catch (Exception ex)
            {
                _errorHandlingService.HandleException(ex, nameof(LogsViewModel),
                    "Failed to load logs", ErrorSeverity.Error);
            }
        }

        [RelayCommand]
        public void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedLogLevel = "All";
            StartDate = DateTime.Now.AddDays(-7);
        }

        [RelayCommand]
        public void Refresh()
        {
            LoadLogsAsync();
        }        // Note: We're not using this command anymore as we're directly showing the popup in LogsPage.xaml.cs
        [RelayCommand]
        public void ViewLogDetails(ActivityLogItem logItem)
        {
            // This method is kept for compatibility but isn't actively used anymore
        }

        [RelayCommand]
        public async Task ExportLogsAsync()
        {
            try
            {
                // Create a temp file to export logs to
                string tempFilePath = Path.Combine(
                    Path.GetTempPath(),
                    $"BalatroSaveToolkit_Logs_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                // Export logs to the temp file
                bool exported = await _logService.ExportLogsAsync(tempFilePath);

                if (exported && File.Exists(tempFilePath))
                {
                    // Use FileSaver to let user choose where to save the file
                    var fileSaverResult = await _fileSaver.SaveAsync(
                        "BalatroSaveToolkit_Logs.txt",
                        File.OpenRead(tempFilePath),
                        CancellationToken.None);

                    if (fileSaverResult.IsSuccessful)
                    {
                        _errorHandlingService.ShowNotificationAsync(
                            $"Logs exported successfully to {fileSaverResult.FilePath}",
                            ErrorSeverity.Information);
                    }

                    // Clean up temp file
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
                else
                {
                    _errorHandlingService.LogError(nameof(LogsViewModel),
                        "Failed to export logs. No logs were available to export.",
                        ErrorSeverity.Warning);
                }
            }
            catch (Exception ex)
            {
                _errorHandlingService.HandleException(ex, nameof(LogsViewModel),
                    "Failed to export logs", ErrorSeverity.Error);
            }
        }

        private void ApplyFilters()
        {
            IEnumerable<ActivityLogItem> filtered = AllLogs;

            // Apply level filter
            if (SelectedLogLevel != "All")
            {
                filtered = filtered.Where(log => log.Level == SelectedLogLevel);
            }

            // Apply search text filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.ToLower();
                filtered = filtered.Where(log =>
                    log.Message.ToLower().Contains(search) ||
                    log.Source.ToLower().Contains(search) ||
                    (log.Details?.ToLower().Contains(search) ?? false));
            }

            FilteredLogs = new ObservableCollection<ActivityLogItem>(filtered);
        }
    }
}
