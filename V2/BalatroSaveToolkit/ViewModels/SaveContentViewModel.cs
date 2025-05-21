using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using BalatroSaveToolkit.Core.Services;
using Splat;

namespace BalatroSaveToolkit.ViewModels
{
    /// <summary>
    /// ViewModel for viewing the content of a save file.
    /// </summary>
    public class SaveContentViewModel : ReactiveObject
    {
        [Reactive]
        public string FilePath { get; set; } = string.Empty;

        [Reactive]
        public string RawContent { get; set; } = string.Empty;

        [Reactive]
        public string DisplayContent { get; set; } = string.Empty;

        [Reactive]
        public bool IsLoading { get; set; }

        [Reactive]
        public string? ErrorMessage { get; set; }

        private readonly IFileSystemService _fileSystemService;

        public ReactiveCommand<Unit, Unit> LoadContentCommand { get; }

        public SaveContentViewModel()
        {
            _fileSystemService = Locator.Current.GetService<IFileSystemService>()!;
            LoadContentCommand = ReactiveCommand.CreateFromTask(LoadContentAsync);
        }

        private async Task LoadContentAsync()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                ErrorMessage = "No file selected.";
                return;
            }
            IsLoading = true;
            ErrorMessage = null;
            try
            {
                RawContent = await _fileSystemService.GetSaveFileContentAsync(FilePath);
                DisplayContent = RawContent; // TODO: Format for display
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load save content: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
