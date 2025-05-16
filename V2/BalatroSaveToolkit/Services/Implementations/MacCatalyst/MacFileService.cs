using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations.MacCatalyst
{
    /// <summary>
    /// MacOS specific implementation of IFileService
    /// </summary>
    public class MacFileService : FileService
    {
        public MacFileService(IErrorHandlingService errorHandler) : base(errorHandler)
        {
        }

        /// <summary>
        /// Pick a folder using native macOS folder picker
        /// </summary>
        public override async Task<string> PickFolderAsync(string title)
        {
            try
            {
                // On macOS, we'll use the NSOpenPanel API through ObjC bindings
                // Note: This is simplified and assumes the necessary entitlements are set in Info.plist
                // A more complete implementation would require deeper platform-specific code
                
                // For now, we'll use the FilePicker as a workaround
                _errorHandler.LogError(nameof(MacFileService), 
                    "Native macOS folder picker not fully implemented yet. Using fallback method.", 
                    ErrorSeverity.Information);
                
                // Use the base implementation as fallback
                return await base.PickFolderAsync(title);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacFileService), 
                    "Error picking folder on macOS", ErrorSeverity.Warning);
                return await base.PickFolderAsync(title);
            }
        }

        /// <summary>
        /// Pick a save file location using native macOS save dialog
        /// </summary>
        public override async Task<string> PickSaveFileAsync(string title, string suggestedName, string filter)
        {
            try
            {
                // On macOS, we'd use the NSSavePanel API through ObjC bindings
                // This is a placeholder for when we implement the full macOS-specific code
                
                _errorHandler.LogError(nameof(MacFileService), 
                    "Native macOS save file picker not fully implemented yet. Using fallback method.", 
                    ErrorSeverity.Information);
                
                // Use the base implementation as fallback
                return await base.PickSaveFileAsync(title, suggestedName, filter);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacFileService), 
                    "Error saving file on macOS", ErrorSeverity.Warning);
                return await base.PickSaveFileAsync(title, suggestedName, filter);
            }
        }
        
        /// <summary>
        /// Get the application data directory on macOS
        /// </summary>
        public override async Task<string> GetApplicationDataDirectoryAsync()
        {
            try
            {
                // On macOS, app data is typically stored in ~/Library/Application Support/{BundleIdentifier}
                // MAUI's FileSystem.AppDataDirectory should map to this location
                return await base.GetApplicationDataDirectoryAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacFileService), 
                    "Error getting application data directory on macOS", ErrorSeverity.Error);
                
                // Try to get the home directory as a fallback
                var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(homeDir, "Library/Application Support/BalatroSaveToolkit");
            }
        }
    }
}
