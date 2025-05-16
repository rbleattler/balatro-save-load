using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations
{
    public class FileService : IFileService
    {
        public async Task<bool> FileExistsAsync(string filePath)
        {
            return File.Exists(filePath);
        }

        public async Task<string> GetApplicationDataDirectoryAsync()
        {
            return FileSystem.AppDataDirectory;
        }

        public async Task<string> ReadTextAsync(string filePath)
        {
            if (await FileExistsAsync(filePath))
            {
                return await File.ReadAllTextAsync(filePath);
            }
            return string.Empty;
        }

        public async Task WriteTextAsync(string filePath, string content)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, content);
        }        public async Task<string> PickFileAsync(string title, string filter)
        {
            try
            {
                var options = new PickOptions
                {
                    PickerTitle = title,
                    FileTypes = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.WinUI, new[] { filter } },
                            { DevicePlatform.MacCatalyst, new[] { filter } },
                            { DevicePlatform.Android, new[] { filter } },
                            { DevicePlatform.iOS, new[] { filter } }
                        })
                };

                var result = await FilePicker.Default.PickAsync(options);
                return result?.FullPath ?? string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"File picking failed: {ex.Message}");
                return string.Empty;
            }
        }        public async Task<string> PickFolderAsync(string title)
        {
            try
            {
                // TODO: Implement platform-specific folder picker (TSK006)
                // MAUI doesn't have a direct FolderPicker, so we need to use platform-specific implementations
                // For now, we'll return the app data directory as a fallback

                return await GetApplicationDataDirectoryAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Folder picking failed: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<string> PickSaveFileAsync(string title, string suggestedName, string filter)
        {
            try
            {
                // MAUI doesn't have a FileSavePicker, so we need to use platform-specific implementations
                // For now, we'll create a file in the app data directory
                var appDataDir = await GetApplicationDataDirectoryAsync();
                string filePath = Path.Combine(appDataDir, suggestedName);

                // Create the directory if it doesn't exist
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save file picking failed: {ex.Message}");
                return string.Empty;
            }
        }        public async Task CopyFileAsync(string sourcePath, string destinationPath)
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
                string directory = Path.GetDirectoryName(destinationPath);
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
                System.Diagnostics.Debug.WriteLine($"File copy failed: {ex.Message}");
                throw; // Rethrow to let caller handle as appropriate
            }
        }
    }
}
