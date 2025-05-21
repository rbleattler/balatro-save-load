using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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

        public ReactiveCommand<Unit, Unit> LoadContentCommand { get; }

        public SaveContentViewModel()
        {
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
                // TODO: Inject and use IFileSystemService for real file loading
                // For now, simulate loading
                await Task.Delay(300); // Simulate IO
                // RawContent = await fileSystemService.GetSaveFileContentAsync(FilePath);
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
