using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations
{
    /// <summary>
    /// Base implementation of IPathProvider with common functionality across platforms.
    /// Platform-specific implementations should inherit from this class.
    /// </summary>
    public abstract class BasePathProvider : IPathProvider
    {
        protected readonly ILogService _logService;
        protected readonly IErrorHandlingService _errorHandler;
        protected const string AppName = "BalatroSaveToolkit";
        protected const string BackupFolderName = "SaveBackups";
        protected const string LogsFolderName = "Logs";
        protected const string ExportFolderName = "Exports";

        /// <summary>
        /// Gets the platform-appropriate path separator character.
        /// </summary>
        public virtual char PathSeparator => Path.DirectorySeparatorChar;

        /// <summary>
        /// Determines if the platform's file system is case-sensitive.
        /// Overridden by platform-specific implementations.
        /// </summary>
        public abstract bool IsCaseSensitive { get; }

        /// <summary>
        /// Initializes a new instance of BasePathProvider.
        /// </summary>
        /// <param name="logService">The logging service.</param>
        /// <param name="errorHandler">The error handling service.</param>
        public BasePathProvider(ILogService logService, IErrorHandlingService errorHandler)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        }

        /// <summary>
        /// Gets the path to the Balatro save directory for the current platform.
        /// Must be implemented by platform-specific providers.
        /// </summary>
        public abstract Task<string> GetBalatroSaveDirectoryAsync(bool steamInstallation = false);

        /// <summary>
        /// Gets the path to the application data directory.
        /// </summary>
        public virtual async Task<string> GetApplicationDataDirectoryAsync(string? appName = null)
        {
            try
            {
                // Use MAUI's built-in cross-platform folder
                string baseDir = FileSystem.AppDataDirectory;

                if (!string.IsNullOrEmpty(appName))
                {
                    baseDir = Path.Combine(baseDir, appName);
                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(baseDir))
                    {
                        Directory.CreateDirectory(baseDir);
                    }
                }

                return baseDir;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), "Error getting application data directory", ErrorSeverity.Error, false);
                // Fallback to temp directory
                return Path.GetTempPath();
            }
        }

        /// <summary>
        /// Gets the path to the temporary directory.
        /// </summary>
        public virtual Task<string> GetTempDirectoryAsync()
        {
            try
            {
                return Task.FromResult(Path.GetTempPath());
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), "Error getting temporary directory", ErrorSeverity.Warning, false);
                // Return a subfolder of the app data directory as a fallback
                return GetApplicationDataDirectoryAsync("Temp");
            }
        }

        /// <summary>
        /// Gets the path to the user's documents directory.
        /// </summary>
        public virtual Task<string> GetDocumentsDirectoryAsync()
        {
            try
            {
                return Task.FromResult(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), "Error getting documents directory", ErrorSeverity.Warning, false);
                throw new DirectoryNotFoundException("Could not locate the documents directory", ex);
            }
        }

        /// <summary>
        /// Gets the path to the user's desktop directory.
        /// </summary>
        public virtual Task<string> GetDesktopDirectoryAsync()
        {
            try
            {
                return Task.FromResult(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), "Error getting desktop directory", ErrorSeverity.Warning, false);
                throw new DirectoryNotFoundException("Could not locate the desktop directory", ex);
            }
        }

        /// <summary>
        /// Gets the path to the user's home directory.
        /// </summary>
        public virtual Task<string> GetUserHomeDirectoryAsync()
        {
            try
            {
                return Task.FromResult(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), "Error getting user home directory", ErrorSeverity.Warning, false);
                throw new DirectoryNotFoundException("Could not locate the user home directory", ex);
            }
        }

        /// <summary>
        /// Gets the path to the directory where save backups should be stored.
        /// </summary>
        public virtual async Task<string> GetSaveBackupsDirectoryAsync()
        {
            try
            {
                string appDataDir = await GetApplicationDataDirectoryAsync(AppName);
                string backupDir = Path.Combine(appDataDir, BackupFolderName);

                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                return backupDir;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), "Error getting save backups directory", ErrorSeverity.Error, false);
                // Fallback to a subfolder in the temp directory
                string tempDir = await GetTempDirectoryAsync();
                string backupDir = Path.Combine(tempDir, $"{AppName}_{BackupFolderName}");

                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                return backupDir;
            }
        }

        /// <summary>
        /// Gets the path to the logs directory.
        /// </summary>
        public virtual async Task<string> GetLogsDirectoryAsync()
        {
            try
            {
                string appDataDir = await GetApplicationDataDirectoryAsync(AppName);
                string logsDir = Path.Combine(appDataDir, LogsFolderName);

                if (!Directory.Exists(logsDir))
                {
                    Directory.CreateDirectory(logsDir);
                }

                return logsDir;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), "Error getting logs directory", ErrorSeverity.Error, false);
                // Fallback to a subfolder in the temp directory
                string tempDir = await GetTempDirectoryAsync();
                string logsDir = Path.Combine(tempDir, $"{AppName}_{LogsFolderName}");

                if (!Directory.Exists(logsDir))
                {
                    Directory.CreateDirectory(logsDir);
                }

                return logsDir;
            }
        }

        /// <summary>
        /// Gets the path to the export directory for save files.
        /// </summary>
        public virtual async Task<string> GetExportDirectoryAsync()
        {
            try
            {
                string appDataDir = await GetApplicationDataDirectoryAsync(AppName);
                string exportDir = Path.Combine(appDataDir, ExportFolderName);

                if (!Directory.Exists(exportDir))
                {
                    Directory.CreateDirectory(exportDir);
                }

                return exportDir;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), "Error getting export directory", ErrorSeverity.Error, false);

                // Try to use Documents folder
                try
                {
                    string docsDir = await GetDocumentsDirectoryAsync();
                    string exportDir = Path.Combine(docsDir, AppName, ExportFolderName);

                    if (!Directory.Exists(exportDir))
                    {
                        Directory.CreateDirectory(exportDir);
                    }

                    return exportDir;
                }
                catch
                {
                    // Last resort fallback to temp directory
                    string tempDir = await GetTempDirectoryAsync();
                    string exportDir = Path.Combine(tempDir, $"{AppName}_{ExportFolderName}");

                    if (!Directory.Exists(exportDir))
                    {
                        Directory.CreateDirectory(exportDir);
                    }

                    return exportDir;
                }
            }
        }

        /// <summary>
        /// Resolves platform-specific special paths.
        /// </summary>
        public virtual Task<string> GetSpecialFolderPathAsync(Environment.SpecialFolder specialFolder)
        {
            try
            {
                return Task.FromResult(Environment.GetFolderPath(specialFolder));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), $"Error getting special folder: {specialFolder}", ErrorSeverity.Warning, false);
                throw new DirectoryNotFoundException($"Could not locate the special folder: {specialFolder}", ex);
            }
        }

        /// <summary>
        /// Checks if the provided path appears to be a valid Balatro save file.
        /// </summary>
        public virtual async Task<bool> IsBalatroSaveFilePathAsync(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return false;
            }

            try
            {
                // Check file extension
                string ext = Path.GetExtension(path).ToLower();
                if (ext != ".save" && ext != ".json")
                {
                    return false;
                }

                // Check if it's in the Balatro save directory
                string saveDir = await GetBalatroSaveDirectoryAsync();
                if (Path.GetDirectoryName(path)?.StartsWith(saveDir, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }

                // If not in the save directory, do a basic content check
                // We only check the beginning of the file to see if it looks like a Balatro save
                const int bytesToCheck = 1000; // Just check the first 1000 bytes

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (fs.Length == 0)
                    {
                        return false;
                    }

                    byte[] buffer = new byte[Math.Min(bytesToCheck, fs.Length)];
                    await fs.ReadAsync(buffer, 0, buffer.Length);

                    string content = System.Text.Encoding.UTF8.GetString(buffer);

                    // Look for typical Balatro save file contents
                    return content.Contains("\"seed\"") &&
                           content.Contains("\"deck\"") &&
                           (content.Contains("\"hand\"") || content.Contains("\"jokers\""));
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(BasePathProvider), $"Error checking Balatro save file: {path}", ErrorSeverity.Information, false);
                return false; // If we get any error, assume it's not a valid save file
            }
        }
    }
}
