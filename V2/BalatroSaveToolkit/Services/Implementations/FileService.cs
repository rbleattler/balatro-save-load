using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations
{
    /// <summary>
    /// Implementation of IFileService that uses the MAUI FileSystem APIs
    /// with proper error handling. This base class can be extended by platform-specific implementations.
    /// </summary>
    public class FileService : IFileService
    {
        protected readonly IErrorHandlingService _errorHandler;

        public FileService(IErrorHandlingService errorHandler)
        {
            _errorHandler = errorHandler;
        }        public virtual async Task<bool> FileExistsAsync(string filePath)
        {
            try
            {
                // Not truly async, but keeping the method signature consistent
                return await Task.FromResult(File.Exists(filePath));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(FileService), $"Error checking if file exists: {filePath}", ErrorSeverity.Warning, false);
                return false;
            }
        }

        public virtual async Task<string> GetApplicationDataDirectoryAsync()
        {
            try
            {
                // Not truly async, but keeping the method signature consistent
                return await Task.FromResult(FileSystem.AppDataDirectory);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(FileService), "Error getting application data directory", ErrorSeverity.Error);
                return Path.GetTempPath(); // Fallback to temp directory
            }
        }        public virtual async Task<string> ReadTextAsync(string filePath)
        {
            try
            {
                if (await FileExistsAsync(filePath))
                {
                    return await File.ReadAllTextAsync(filePath);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(FileService), $"Error reading file: {filePath}", ErrorSeverity.Error);
                return string.Empty;
            }
        }

        public virtual async Task WriteTextAsync(string filePath, string content)
        {
            try
            {
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(filePath, content);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(FileService), $"Error writing to file: {filePath}", ErrorSeverity.Error);
                throw; // Rethrow since this is a critical operation
            }
        }public virtual async Task<string> PickFileAsync(string title, string filter)
        {
            try
            {
                // Use FilePicker from MAUI's Microsoft.Maui.Storage namespace
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { filter } },
                        { DevicePlatform.MacCatalyst, new[] { filter } },
                        { DevicePlatform.Android, new[] { filter } },
                        { DevicePlatform.iOS, new[] { filter } }
                    }
                );

                var options = new PickOptions
                {
                    PickerTitle = title,
                    FileTypes = customFileType
                };

                var result = await FilePicker.Default.PickAsync(options);
                return result?.FullPath ?? string.Empty;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(FileService), "Unable to pick a file", ErrorSeverity.Warning);
                return string.Empty;
            }
        }        public virtual async Task<string> PickFolderAsync(string title)
        {
            try
            {
                // Base implementation uses fallback mechanism
                // Platform-specific implementations should override this method
                _errorHandler.LogError(nameof(FileService), "Folder picker not fully implemented yet. Using app data directory instead.", ErrorSeverity.Information);
                return await GetApplicationDataDirectoryAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(FileService), "Unable to pick a folder", ErrorSeverity.Warning);
                return string.Empty;
            }
        }        public virtual async Task<string> PickSaveFileAsync(string title, string suggestedName, string filter)
        {
            try
            {
                // Base implementation uses app data directory as a fallback
                // Platform-specific implementations should override this method
                _errorHandler.LogError(nameof(FileService), "Save file picker not fully implemented yet. Using app data directory instead.", ErrorSeverity.Information);

                var appDataDir = await GetApplicationDataDirectoryAsync();
                string filePath = Path.Combine(appDataDir, suggestedName);

                // Create the directory if it doesn't exist
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(FileService), "Unable to create a save file", ErrorSeverity.Warning);
                return string.Empty;
            }
        }        public virtual async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            try
            {
                if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath))
                {
                    throw new ArgumentException("Source or destination path cannot be null or empty.");
                }

                if (!await FileExistsAsync(sourcePath))
                {
                    throw new FileNotFoundException($"Source file not found: {sourcePath}");
                }

                // Create the destination directory if it doesn't exist
                string? directory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Use async file operations
                using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(FileService), $"Failed to copy file from {sourcePath} to {destinationPath}", ErrorSeverity.Error);
                throw; // Rethrow to let caller handle as appropriate
            }
        }
    }
}
