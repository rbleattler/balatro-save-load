using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations.Linux
{
    /// <summary>
    /// Linux specific implementation of IFileService
    /// </summary>
    public class LinuxFileService : FileService
    {
        public LinuxFileService(IErrorHandlingService errorHandler) : base(errorHandler)
        {
        }

        /// <summary>
        /// Pick a folder using native Linux folder picker
        /// </summary>
        public override async Task<string> PickFolderAsync(string title)
        {
            try
            {
                // For Linux, we need to use platform-specific APIs or external tools
                // This could involve spawning a process for a native file picker like zenity or kdialog
                // Here we're showing a placeholder implementation
                
                _errorHandler.LogError(nameof(LinuxFileService), 
                    "Native Linux folder picker not fully implemented yet. Using fallback method.", 
                    ErrorSeverity.Information);
                
                // Use the base implementation as fallback
                return await base.PickFolderAsync(title);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxFileService), 
                    "Error picking folder on Linux", ErrorSeverity.Warning);
                return await base.PickFolderAsync(title);
            }
        }

        /// <summary>
        /// Pick a save file location using native Linux save dialog
        /// </summary>
        public override async Task<string> PickSaveFileAsync(string title, string suggestedName, string filter)
        {
            try
            {
                // For Linux, we would typically use a native file picker through GTK# or similar
                // This is a placeholder for when we implement the full Linux-specific code
                
                _errorHandler.LogError(nameof(LinuxFileService), 
                    "Native Linux save file picker not fully implemented yet. Using fallback method.", 
                    ErrorSeverity.Information);
                
                // Use the base implementation as fallback
                return await base.PickSaveFileAsync(title, suggestedName, filter);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxFileService), 
                    "Error saving file on Linux", ErrorSeverity.Warning);
                return await base.PickSaveFileAsync(title, suggestedName, filter);
            }
        }
        
        /// <summary>
        /// Get the application data directory on Linux
        /// </summary>
        public override async Task<string> GetApplicationDataDirectoryAsync()
        {
            try
            {
                // On Linux, app data is typically stored in ~/.local/share/{AppName}
                // MAUI's FileSystem.AppDataDirectory should map to this location
                return await base.GetApplicationDataDirectoryAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxFileService), 
                    "Error getting application data directory on Linux", ErrorSeverity.Error);
                
                // Try to get the home directory as a fallback
                var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(homeDir, ".local/share/BalatroSaveToolkit");
            }
        }
    }
}
