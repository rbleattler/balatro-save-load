using BalatroSaveToolkit.Services.Interfaces;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;

namespace BalatroSaveToolkit.Services.Implementations.Windows
{
    /// <summary>
    /// Windows-specific implementation of IFileService
    /// </summary>
    public class WindowsFileService : FileService
    {
        // Keep the error handler from the base class
        public WindowsFileService(IErrorHandlingService errorHandler) : base(errorHandler)
        {
        }

        /// <summary>
        /// Pick a folder using the Windows folder picker
        /// </summary>
        public override async Task<string> PickFolderAsync(string title)
        {
            try
            {
                var folderPicker = new global::Windows.Storage.Pickers.FolderPicker();
                folderPicker.SuggestedStartLocation = global::Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                folderPicker.FileTypeFilter.Add("*");
                folderPicker.ViewMode = global::Windows.Storage.Pickers.PickerViewMode.List;
                folderPicker.CommitButtonText = title;

                // Initialize the WinRT object with a valid window handle
                WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, GetActiveWindowHandle());

                // Show the picker and get the folder
                var folder = await folderPicker.PickSingleFolderAsync();

                return folder != null ? folder.Path : string.Empty;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(WindowsFileService), "Error picking folder on Windows", ErrorSeverity.Warning);
                return await base.PickFolderAsync(title);
            }
        }        /// <summary>
        /// Pick a save file location using the Windows file save picker
        /// </summary>
        public override async Task<string> PickSaveFileAsync(string title, string suggestedName, string filter)
        {
            try
            {
                var savePicker = new global::Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = global::Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                savePicker.SuggestedFileName = suggestedName;
                savePicker.FileTypeChoices.Add("Files", new List<string>() { filter });

                // Initialize the WinRT object with a valid window handle
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, GetActiveWindowHandle());

                // Show the picker and get the file
                var file = await savePicker.PickSaveFileAsync();

                return file != null ? file.Path : string.Empty;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(WindowsFileService), "Error saving file on Windows", ErrorSeverity.Warning);
                return await base.PickSaveFileAsync(title, suggestedName, filter);
            }
        }        // Helper method to get the active window handle for Windows UI operations
        private IntPtr GetActiveWindowHandle()
        {
            // Get the window from the current UI implementation
            var window = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView;

            if (window == null)
            {
                throw new InvalidOperationException("Could not get the active window handle. The application window is not available.");
            }

            // Get the window handle
            return WinRT.Interop.WindowNative.GetWindowHandle(window);
        }
    }
}
