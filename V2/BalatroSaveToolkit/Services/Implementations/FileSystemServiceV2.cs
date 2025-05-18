using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BalatroSaveToolkit.Services.Interfaces;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Storage;

namespace BalatroSaveToolkit.Services.Implementations
{
    /// <summary>
    /// Updated implementation of IFileSystemService that leverages MAUI's built-in FileSystem APIs
    /// and integrates with the CommunityToolkit.Maui.Storage for file picking operations.
    /// </summary>
    public class FileSystemService : IFileSystemService
    {
        private readonly Dictionary<string, FileSystemWatcher> _watchers = new();
        private readonly ILogService _logService;
        private readonly IErrorHandlingService _errorHandler;
        private readonly IFileSaver _fileSaver;
        private readonly IFolderPicker _folderPicker;
        private readonly IPathProvider _pathProvider;
        private const string LogTag = nameof(FileSystemService);

        /// <summary>
        /// Event raised when a monitored file or directory changes.
        /// </summary>
        public event EventHandler<FileSystemEventArgs>? FileChanged;

        /// <summary>
        /// Initializes a new instance of FileSystemService.
        /// </summary>
        public FileSystemService(
            ILogService logService,
            IErrorHandlingService errorHandler,
            IFileSaver fileSaver,
            IFolderPicker folderPicker,
            IPathProvider pathProvider)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            _fileSaver = fileSaver ?? throw new ArgumentNullException(nameof(fileSaver));
            _folderPicker = folderPicker ?? throw new ArgumentNullException(nameof(folderPicker));
            _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
        }

        /// <inheritdoc />
        public virtual async Task<string> GetBalatroSaveDirectoryAsync()
        {
            try
            {
                // Delegate to the path provider to get the platform-specific save directory
                return await _pathProvider.GetBalatroSaveDirectoryAsync();
            }
            catch (PlatformNotSupportedException ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Platform not supported for Balatro save files", ErrorSeverity.Error, false);
                throw;
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, ex.Message, ErrorSeverity.Warning, false);
                throw;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error getting Balatro save directory", ErrorSeverity.Error, false);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> GetApplicationDataDirectoryAsync(string? appName = null)
        {
            try
            {
                // Use MAUI's built-in cross-platform folder
                string baseDir = FileSystem.AppDataDirectory;

                if (!string.IsNullOrEmpty(appName))
                {
                    baseDir = Path.Combine(baseDir, appName);
                    if (!Directory.Exists(baseDir))
                    {
                        Directory.CreateDirectory(baseDir);
                    }
                }

                return baseDir;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error getting application data directory", ErrorSeverity.Error, false);
                // Fallback to temp directory
                return Path.GetTempPath();
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> CreateTempFileAsync(string? extension = null)
        {
            try
            {
                string tempPath;
                if (string.IsNullOrEmpty(extension))
                {
                    tempPath = Path.GetTempFileName();
                }
                else
                {
                    // Ensure extension starts with a dot
                    if (!extension.StartsWith('.'))
                    {
                        extension = $".{extension}";
                    }

                    string tempDir = Path.GetTempPath();
                    tempPath = Path.Combine(tempDir, $"{Path.GetRandomFileName()}{extension}");

                    // Create an empty file
                    using FileStream fs = File.Create(tempPath);
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error creating temporary file", ErrorSeverity.Warning, false);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> CreateTempDirectoryAsync()
        {
            try
            {
                string tempDir = Path.GetTempPath();
                string randomDir = Path.Combine(tempDir, Path.GetRandomFileName());

                Directory.CreateDirectory(randomDir);
                return randomDir;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error creating temporary directory", ErrorSeverity.Warning, false);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> PickFileAsync(string title, string filter)
        {
            try
            {
                // Use the built-in FilePicker from MAUI
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { filter } },
                        { DevicePlatform.MacCatalyst, new[] { filter } },
                        { DevicePlatform.Android, new[] { filter } },
                        { DevicePlatform.iOS, new[] { filter } }
                    });

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
                _errorHandler.HandleException(ex, LogTag, "Error picking file", ErrorSeverity.Warning, false);
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> PickFolderAsync(string title)
        {
            try
            {
                // Use the CommunityToolkit FolderPicker
                var result = await _folderPicker.PickAsync(new CancellationToken());
                return result.IsSuccessful ? result.Folder.Path : string.Empty;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error picking folder", ErrorSeverity.Warning, false);
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> PickSaveFileAsync(string title, string suggestedName, string filter)
        {
            try
            {
                // Create a temporary file with the content
                var tempFile = await CreateTempFileAsync(Path.GetExtension(suggestedName));

                // Use the CommunityToolkit FileSaver to let the user choose where to save it
                var fileStream = new FileStream(tempFile, FileMode.Open, FileAccess.Read);
                var result = await _fileSaver.SaveAsync(Path.GetDirectoryName(suggestedName) ?? "",
                                                      Path.GetFileName(suggestedName),
                                                      fileStream);

                // Clean up the temp file
                fileStream.Close();
                File.Delete(tempFile);

                return result.IsSuccessful ? result.FilePath : string.Empty;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error with save file dialog", ErrorSeverity.Warning, false);
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> StartMonitoringAsync(string path, bool includeSubdirectories = false, string filter = "*")
        {
            try
            {
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path) ?? throw new ArgumentException("Invalid file path", nameof(path));
                }
                else if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException($"Directory not found: {path}");
                }

                string token = Guid.NewGuid().ToString();

                var watcher = new FileSystemWatcher(path)
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                    Filter = filter,
                    IncludeSubdirectories = includeSubdirectories,
                    EnableRaisingEvents = true
                };

                // Wire up events
                watcher.Changed += OnFileChanged;
                watcher.Created += OnFileChanged;
                watcher.Deleted += OnFileChanged;
                watcher.Renamed += OnFileRenamed;

                _watchers[token] = watcher;
                return token;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error starting file monitoring for {path}", ErrorSeverity.Error, false);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task StopMonitoringAsync(string monitorToken)
        {
            try
            {
                if (!_watchers.TryGetValue(monitorToken, out var watcher))
                {
                    throw new ArgumentException("Invalid monitoring token", nameof(monitorToken));
                }

                // Unwire events
                watcher.Changed -= OnFileChanged;
                watcher.Created -= OnFileChanged;
                watcher.Deleted -= OnFileChanged;
                watcher.Renamed -= OnFileRenamed;

                // Dispose and remove
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                _watchers.Remove(monitorToken);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error stopping file monitoring", ErrorSeverity.Warning, false);
                throw;
            }
        }

        private void OnFileChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            try
            {
                FileSystemChangeType changeType = e.ChangeType switch
                {
                    WatcherChangeTypes.Created => FileSystemChangeType.Created,
                    WatcherChangeTypes.Deleted => FileSystemChangeType.Deleted,
                    WatcherChangeTypes.Changed => FileSystemChangeType.Modified,
                    _ => FileSystemChangeType.Modified
                };

                FileChanged?.Invoke(this, new FileSystemEventArgs(e.FullPath, changeType, DateTime.Now));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error in file change notification", ErrorSeverity.Warning, false);
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                FileChanged?.Invoke(this, new FileSystemEventArgs(
                    e.FullPath,
                    FileSystemChangeType.Renamed,
                    DateTime.Now));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error in file rename notification", ErrorSeverity.Warning, false);
            }
        }
    }
}
