using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.Extensions;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.ViewModels
{
    public partial class SaveViewerViewModel : BaseViewModel
    {
        private readonly ISaveService _saveService;
        private readonly ILogService _logService;
        private readonly IFileService _fileService;

        [ObservableProperty]
        private ObservableCollection<SaveFileInfo> _saveFiles = new();
          [ObservableProperty]
        private SaveFileInfo? _selectedSaveFile;
          [ObservableProperty]
        private SaveData? _saveData;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private string _rawSaveData = string.Empty;

        [ObservableProperty]
        private string _noSaveSelectedMessage = "Please select a save file to view its data";

        [ObservableProperty]
        private bool _isSaveSelected;

        [ObservableProperty]
        private bool _isNoSaveSelected = true;

        public SaveViewerViewModel(
            ISaveService saveService,
            ILogService logService,
            IFileService fileService)
        {
            _saveService = saveService;
            _logService = logService;
            _fileService = fileService;

            Title = "Save Viewer";
        }

        public async Task LoadSavesAsync()
        {
            try
            {
                IsBusy = true;

                var saves = await _saveService.GetSaveFilesAsync();
                SaveFiles.Clear();

                foreach (var save in saves)
                {
                    SaveFiles.Add(save);
                }

                await _logService.LogInfoAsync($"Loaded {saves.Count} save files");
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Failed to load save files", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSelectedSaveFileChanged(SaveFileInfo value)
        {
            if (value != null)
            {
                LoadSaveDataAsync(value).ConfigureAwait(false);
                IsSaveSelected = true;
                IsNoSaveSelected = false;
            }
            else
            {
                IsSaveSelected = false;
                IsNoSaveSelected = true;
                SaveData = null;
                RawSaveData = string.Empty;
            }
        }

        private async Task LoadSaveDataAsync(SaveFileInfo saveFile)
        {
            try
            {
                IsBusy = true;

                SaveData = await _saveService.LoadSaveDataAsync(saveFile.FilePath);
                RawSaveData = await _fileService.ReadTextAsync(saveFile.FilePath);

                await _logService.LogInfoAsync($"Loaded save data from {saveFile.Name}");
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Failed to load save data from {saveFile.FilePath}", ex);
                await Shell.Current.DisplayAlert("Error", $"Failed to load save data: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SelectSaveAsync(SaveFileInfo saveFile)
        {
            SelectedSaveFile = saveFile;
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            try
            {
                IsBusy = true;

                var saves = await _saveService.SearchSavesAsync(SearchQuery);
                SaveFiles.Clear();

                foreach (var save in saves)
                {
                    SaveFiles.Add(save);
                }
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Search failed", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadSaveAsync()
        {
            if (SelectedSaveFile == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Load Save",
                $"Are you sure you want to load this save: {SelectedSaveFile.Name}?\n\nThis will replace your current game save.",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    await _saveService.LoadSaveAsync(SelectedSaveFile.FilePath);
                    await _logService.LogInfoAsync($"Loaded save {SelectedSaveFile.Name}");
                    await Shell.Current.DisplayAlert("Success", "Save loaded successfully", "OK");
                }
                catch (Exception ex)
                {
                    await _logService.LogErrorAsync($"Failed to load save {SelectedSaveFile.Name}", ex);
                    await Shell.Current.DisplayAlert("Error", $"Failed to load save: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task DeleteSaveAsync()
        {
            if (SelectedSaveFile == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Save",
                $"Are you sure you want to delete this save: {SelectedSaveFile.Name}?",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    await _saveService.DeleteSaveAsync(SelectedSaveFile.FilePath);
                    SaveFiles.Remove(SelectedSaveFile);
                    SelectedSaveFile = null;

                    await _logService.LogInfoAsync($"Deleted save {SelectedSaveFile.Name}");
                }
                catch (Exception ex)
                {
                    await _logService.LogErrorAsync($"Failed to delete save {SelectedSaveFile.Name}", ex);
                    await Shell.Current.DisplayAlert("Error", $"Failed to delete save: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task ExportSaveAsync()
        {
            if (SelectedSaveFile == null)
                return;

            try
            {
                string exportPath = await _fileService.PickSaveFileAsync(
                    "Export save file",
                    SelectedSaveFile.Name,
                    "Text files (*.txt)|*.txt");

                if (string.IsNullOrEmpty(exportPath))
                    return;

                await _fileService.CopyFileAsync(SelectedSaveFile.FilePath, exportPath);
                await _logService.LogInfoAsync($"Exported save {SelectedSaveFile.Name} to {exportPath}");
                await Shell.Current.DisplayAlert("Success", "Save exported successfully", "OK");
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync($"Failed to export save {SelectedSaveFile.Name}", ex);
                await Shell.Current.DisplayAlert("Error", $"Failed to export save: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task EditDataAsync()
        {
            // Navigate to save editor
            if (SelectedSaveFile != null)
            {
                // In a real implementation, we'd pass the save path
                await Shell.Current.DisplayAlert("Coming Soon", "Save editing functionality will be available in a future update", "OK");
            }
        }

        [RelayCommand]
        private async Task CompareSavesAsync()
        {
            // Navigate to save comparison
            if (SelectedSaveFile != null)
            {
                // In a real implementation, we'd pass the current save as part of the comparison
                await Shell.Current.DisplayAlert("Coming Soon", "Save comparison functionality will be available in a future update", "OK");
            }
        }

        [RelayCommand]
        private async Task CloseAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
