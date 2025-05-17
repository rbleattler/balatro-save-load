using System.Security.Cryptography;
using System.Text;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations
{
    /// <summary>
    /// Base implementation of IFileSystemService with platform-agnostic functionality.
    /// This class serves as the foundation for platform-specific implementations.
    /// </summary>
    public class BaseFileSystemService : IFileSystemService
    {
        private const string LogTag = nameof(BaseFileSystemService);

        protected readonly ILogService _logService;
        protected readonly IErrorHandlingService _errorHandler;

        // Track file monitoring registrations
        private readonly Dictionary<string, FileSystemWatcher> _watchers = new();

        /// <summary>
        /// Event raised when a monitored file or directory changes.
        /// </summary>
        public event EventHandler<FileSystemEventArgs>? FileChanged;

        /// <summary>
        /// Initializes a new instance of BaseFileSystemService.
        /// </summary>
        /// <param name="logService">The logging service.</param>
        /// <param name="errorHandler">The error handling service.</param>
        public BaseFileSystemService(ILogService logService, IErrorHandlingService errorHandler)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        }

        #region File Operations

        /// <inheritdoc />
        public virtual async Task<bool> FileExistsAsync(string path)
        {
            try
            {
                // File.Exists is not truly async, but we keep the method signature consistent
                return await Task.FromResult(File.Exists(path));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error checking if file exists: {path}", ErrorSeverity.Warning, false);
                return false;
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> ReadTextAsync(string path, Encoding? encoding = null)
        {
            try
            {
                if (await FileExistsAsync(path))
                {
                    return encoding != null
                        ? await File.ReadAllTextAsync(path, encoding)
                        : await File.ReadAllTextAsync(path);
                }

                throw new FileNotFoundException($"File not found: {path}", path);
            }
            catch (FileNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"File not found: {path}", ErrorSeverity.Warning, false);
                throw new FileNotFoundException($"File not found: {path}", path, ex);
            }
            catch (IOException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error accessing file: {path}", ErrorSeverity.Warning, false);
                throw new FileAccessException(path, $"Cannot access file: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error reading file: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error reading file: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<byte[]> ReadBytesAsync(string path)
        {
            try
            {
                if (await FileExistsAsync(path))
                {
                    return await File.ReadAllBytesAsync(path);
                }

                throw new FileNotFoundException($"File not found: {path}", path);
            }
            catch (FileNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"File not found: {path}", ErrorSeverity.Warning, false);
                throw new FileNotFoundException($"File not found: {path}", path, ex);
            }
            catch (IOException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error accessing file: {path}", ErrorSeverity.Warning, false);
                throw new FileAccessException(path, $"Cannot access file: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error reading file: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error reading file: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task WriteTextAsync(string path, string content, Encoding? encoding = null, bool overwrite = true)
        {
            try
            {
                // Ensure directory exists
                var directory = GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    await CreateDirectoryAsync(directory);
                }

                // Check if we should overwrite
                if (!overwrite && await FileExistsAsync(path))
                {
                    throw new FileAlreadyExistsException(path, $"File already exists and overwrite is false: {path}");
                }

                // Write the file
                if (encoding != null)
                {
                    await File.WriteAllTextAsync(path, content, encoding);
                }
                else
                {
                    await File.WriteAllTextAsync(path, content);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied writing to file: {path}", ErrorSeverity.Error, false);
                throw new FileAccessException(path, $"Access denied writing to file: {path}", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Directory not found for file: {path}", ErrorSeverity.Error, false);
                throw new DirectoryNotFoundException($"Directory not found for file: {path}", ex);
            }
            catch (FileAlreadyExistsException ex)
            {
                _errorHandler.HandleException(ex, LogTag, ex.Message, ErrorSeverity.Warning, false);
                throw;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error writing to file: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error writing to file: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task WriteBytesAsync(string path, byte[] bytes, bool overwrite = true)
        {
            try
            {
                // Ensure directory exists
                var directory = GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    await CreateDirectoryAsync(directory);
                }

                // Check if we should overwrite
                if (!overwrite && await FileExistsAsync(path))
                {
                    throw new FileAlreadyExistsException(path, $"File already exists and overwrite is false: {path}");
                }

                await File.WriteAllBytesAsync(path, bytes);
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied writing to file: {path}", ErrorSeverity.Error, false);
                throw new FileAccessException(path, $"Access denied writing to file: {path}", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Directory not found for file: {path}", ErrorSeverity.Error, false);
                throw new DirectoryNotFoundException($"Directory not found for file: {path}", ex);
            }
            catch (FileAlreadyExistsException ex)
            {
                _errorHandler.HandleException(ex, LogTag, ex.Message, ErrorSeverity.Warning, false);
                throw;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error writing to file: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error writing to file: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task AppendTextAsync(string path, string content, Encoding? encoding = null)
        {
            try
            {
                // Ensure directory exists
                var directory = GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    await CreateDirectoryAsync(directory);
                }

                // Append the text
                if (encoding != null)
                {
                    await File.AppendAllTextAsync(path, content, encoding);
                }
                else
                {
                    await File.AppendAllTextAsync(path, content);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied appending to file: {path}", ErrorSeverity.Error, false);
                throw new FileAccessException(path, $"Access denied appending to file: {path}", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Directory not found for file: {path}", ErrorSeverity.Error, false);
                throw new DirectoryNotFoundException($"Directory not found for file: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error appending to file: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error appending to file: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite = false)
        {
            try
            {
                if (!await FileExistsAsync(sourcePath))
                {
                    throw new FileNotFoundException($"Source file not found: {sourcePath}", sourcePath);
                }

                // Ensure destination directory exists
                var directory = GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    await CreateDirectoryAsync(directory);
                }

                // Check if we should overwrite
                if (!overwrite && await FileExistsAsync(destinationPath))
                {
                    throw new FileAlreadyExistsException(destinationPath, $"Destination file already exists and overwrite is false: {destinationPath}");
                }

                await Task.Run(() => File.Copy(sourcePath, destinationPath, overwrite));
            }
            catch (FileNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Source file not found: {sourcePath}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (FileAlreadyExistsException ex)
            {
                _errorHandler.HandleException(ex, LogTag, ex.Message, ErrorSeverity.Warning, false);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied copying file: {sourcePath} to {destinationPath}", ErrorSeverity.Error, false);
                throw new FileAccessException(sourcePath, $"Access denied copying file: {sourcePath} to {destinationPath}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error copying file: {sourcePath} to {destinationPath}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error copying file: {sourcePath} to {destinationPath}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task MoveFileAsync(string sourcePath, string destinationPath, bool overwrite = false)
        {
            try
            {
                if (!await FileExistsAsync(sourcePath))
                {
                    throw new FileNotFoundException($"Source file not found: {sourcePath}", sourcePath);
                }

                // Ensure destination directory exists
                var directory = GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    await CreateDirectoryAsync(directory);
                }

                // Check if we should overwrite
                if (!overwrite && await FileExistsAsync(destinationPath))
                {
                    throw new FileAlreadyExistsException(destinationPath, $"Destination file already exists and overwrite is false: {destinationPath}");
                }

                // If overwrite is true and the file exists, we need to delete it first
                if (overwrite && await FileExistsAsync(destinationPath))
                {
                    await DeleteFileAsync(destinationPath);
                }

                await Task.Run(() => File.Move(sourcePath, destinationPath));
            }
            catch (FileNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Source file not found: {sourcePath}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (FileAlreadyExistsException ex)
            {
                _errorHandler.HandleException(ex, LogTag, ex.Message, ErrorSeverity.Warning, false);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied moving file: {sourcePath} to {destinationPath}", ErrorSeverity.Error, false);
                throw new FileAccessException(sourcePath, $"Access denied moving file: {sourcePath} to {destinationPath}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error moving file: {sourcePath} to {destinationPath}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error moving file: {sourcePath} to {destinationPath}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteFileAsync(string path)
        {
            try
            {
                if (!await FileExistsAsync(path))
                {
                    throw new FileNotFoundException($"File not found: {path}", path);
                }

                await Task.Run(() => File.Delete(path));
            }
            catch (FileNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"File not found: {path}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied deleting file: {path}", ErrorSeverity.Error, false);
                throw new FileAccessException(path, $"Access denied deleting file: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error deleting file: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error deleting file: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<FileInfo> GetFileInfoAsync(string path)
        {
            try
            {
                if (!await FileExistsAsync(path))
                {
                    throw new FileNotFoundException($"File not found: {path}", path);
                }

                return await Task.FromResult(new FileInfo(path));
            }
            catch (FileNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"File not found: {path}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied getting file info: {path}", ErrorSeverity.Error, false);
                throw new FileAccessException(path, $"Access denied getting file info: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting file info: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error getting file info: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> GetFileHashAsync(string path, string algorithm = "SHA256")
        {
            try
            {
                if (!await FileExistsAsync(path))
                {
                    throw new FileNotFoundException($"File not found: {path}", path);
                }

                using HashAlgorithm hashAlgorithm = algorithm.ToUpperInvariant() switch
                {
                    "MD5" => MD5.Create(),
                    "SHA1" => SHA1.Create(),
                    "SHA256" => SHA256.Create(),
                    "SHA384" => SHA384.Create(),
                    "SHA512" => SHA512.Create(),
                    _ => SHA256.Create() // Default to SHA256
                };

                using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] hashBytes = await hashAlgorithm.ComputeHashAsync(fileStream);

                // Convert to hex string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
            catch (FileNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"File not found: {path}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied getting file hash: {path}", ErrorSeverity.Error, false);
                throw new FileAccessException(path, $"Access denied getting file hash: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting file hash: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error getting file hash: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> CreateFileAsync(string path)
        {
            try
            {
                if (await FileExistsAsync(path))
                {
                    return false;
                }

                // Ensure directory exists
                var directory = GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    await CreateDirectoryAsync(directory);
                }

                // Create empty file
                using FileStream fs = File.Create(path);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied creating file: {path}", ErrorSeverity.Error, false);
                throw new FileAccessException(path, $"Access denied creating file: {path}", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Directory not found for file: {path}", ErrorSeverity.Error, false);
                throw new DirectoryNotFoundException($"Directory not found for file: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error creating file: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error creating file: {path}", ex);
            }
        }

        #endregion

        #region Directory Operations

        /// <inheritdoc />
        public virtual async Task<bool> DirectoryExistsAsync(string path)
        {
            try
            {
                return await Task.FromResult(Directory.Exists(path));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error checking if directory exists: {path}", ErrorSeverity.Warning, false);
                return false;
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> CreateDirectoryAsync(string path, bool recursive = true)
        {
            try
            {
                if (await DirectoryExistsAsync(path))
                {
                    return false;
                }

                if (recursive)
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    // For non-recursive create, we need to check if parent exists
                    string? parent = Path.GetDirectoryName(path);
                    if (string.IsNullOrEmpty(parent) || !Directory.Exists(parent))
                    {
                        throw new DirectoryNotFoundException($"Parent directory does not exist and recursive is false: {parent}");
                    }

                    Directory.CreateDirectory(path);
                }

                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied creating directory: {path}", ErrorSeverity.Error, false);
                throw new DirectoryAccessException(path, $"Access denied creating directory: {path}", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Parent directory not found: {path}", ErrorSeverity.Error, false);
                throw;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error creating directory: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error creating directory: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteDirectoryAsync(string path, bool recursive = false)
        {
            try
            {
                if (!await DirectoryExistsAsync(path))
                {
                    throw new DirectoryNotFoundException($"Directory not found: {path}");
                }

                if (!recursive)
                {
                    // Check if directory is empty
                    var dirInfo = new DirectoryInfo(path);
                    if (dirInfo.GetFileSystemInfos().Length > 0)
                    {
                        throw new DirectoryNotEmptyException(path, $"Directory is not empty and recursive is false: {path}");
                    }
                }

                Directory.Delete(path, recursive);
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Directory not found: {path}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (DirectoryNotEmptyException ex)
            {
                _errorHandler.HandleException(ex, LogTag, ex.Message, ErrorSeverity.Warning, false);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied deleting directory: {path}", ErrorSeverity.Error, false);
                throw new DirectoryAccessException(path, $"Access denied deleting directory: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error deleting directory: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error deleting directory: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<string>> GetFilesAsync(string path, string searchPattern = "*", bool recursive = false)
        {
            try
            {
                if (!await DirectoryExistsAsync(path))
                {
                    throw new DirectoryNotFoundException($"Directory not found: {path}");
                }

                SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                return await Task.FromResult(Directory.GetFiles(path, searchPattern, searchOption));
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Directory not found: {path}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied getting files from directory: {path}", ErrorSeverity.Error, false);
                throw new DirectoryAccessException(path, $"Access denied getting files from directory: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting files from directory: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error getting files from directory: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<string>> GetDirectoriesAsync(string path, string searchPattern = "*", bool recursive = false)
        {
            try
            {
                if (!await DirectoryExistsAsync(path))
                {
                    throw new DirectoryNotFoundException($"Directory not found: {path}");
                }

                SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                return await Task.FromResult(Directory.GetDirectories(path, searchPattern, searchOption));
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Directory not found: {path}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied getting subdirectories from directory: {path}", ErrorSeverity.Error, false);
                throw new DirectoryAccessException(path, $"Access denied getting subdirectories from directory: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting subdirectories from directory: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error getting subdirectories from directory: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<DirectoryInfo> GetDirectoryInfoAsync(string path)
        {
            try
            {
                if (!await DirectoryExistsAsync(path))
                {
                    throw new DirectoryNotFoundException($"Directory not found: {path}");
                }

                return await Task.FromResult(new DirectoryInfo(path));
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Directory not found: {path}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Access denied getting directory info: {path}", ErrorSeverity.Error, false);
                throw new DirectoryAccessException(path, $"Access denied getting directory info: {path}", ex);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting directory info: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error getting directory info: {path}", ex);
            }
        }

        #endregion

        #region Path Operations

        /// <inheritdoc />
        public virtual string CombinePaths(params string[] parts)
        {
            try
            {
                return Path.Combine(parts);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error combining paths", ErrorSeverity.Warning, false);
                // Fall back to manual path combination
                return string.Join(Path.DirectorySeparatorChar.ToString(), parts.Select(p => p.TrimEnd('/', '\\')));
            }
        }

        /// <inheritdoc />
        public virtual string GetDirectoryName(string path)
        {
            try
            {
                return Path.GetDirectoryName(path) ?? string.Empty;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting directory name from path: {path}", ErrorSeverity.Warning, false);
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public virtual string GetFileName(string path)
        {
            try
            {
                return Path.GetFileName(path);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting file name from path: {path}", ErrorSeverity.Warning, false);
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public virtual string GetFileNameWithoutExtension(string path)
        {
            try
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting file name without extension from path: {path}", ErrorSeverity.Warning, false);
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public virtual string GetExtension(string path)
        {
            try
            {
                return Path.GetExtension(path);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting extension from path: {path}", ErrorSeverity.Warning, false);
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public virtual string NormalizePath(string path)
        {
            try
            {
                return Path.GetFullPath(path)
                    .Replace('\\', Path.DirectorySeparatorChar)
                    .Replace('/', Path.DirectorySeparatorChar);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error normalizing path: {path}", ErrorSeverity.Warning, false);
                return path;
            }
        }

        /// <inheritdoc />
        public virtual string GetAbsolutePath(string relativePath, string? basePath = null)
        {
            try
            {
                if (Path.IsPathFullyQualified(relativePath))
                {
                    return relativePath;
                }

                if (basePath == null)
                {
                    basePath = Directory.GetCurrentDirectory();
                }

                return Path.GetFullPath(Path.Combine(basePath, relativePath));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting absolute path for: {relativePath}", ErrorSeverity.Warning, false);
                return relativePath;
            }
        }

        /// <inheritdoc />
        public virtual string GetRelativePath(string relativeTo, string path)
        {
            try
            {
                return Path.GetRelativePath(relativeTo, path);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error getting relative path from {relativeTo} to {path}", ErrorSeverity.Warning, false);
                return path;
            }
        }

        #endregion

        #region Platform-Specific Operations

        /// <inheritdoc />
        public virtual async Task<string> GetBalatroSaveDirectoryAsync()
        {
            // This should be overridden by platform-specific implementations
            throw new PlatformNotSupportedException("Getting Balatro save directory is not implemented in the base class. A platform-specific implementation is required.");
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
        public virtual async Task<string> GetTempDirectoryAsync()
        {
            try
            {
                return await Task.FromResult(Path.GetTempPath());
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error getting temporary directory", ErrorSeverity.Warning, false);
                // Return the application data directory as a fallback
                return await GetApplicationDataDirectoryAsync("Temp");
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

                    string tempDir = await GetTempDirectoryAsync();
                    tempPath = Path.Combine(tempDir, $"{Path.GetRandomFileName()}{extension}");

                    // Create an empty file
                    using FileStream fs = File.Create(tempPath);
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error creating temporary file", ErrorSeverity.Warning, false);
                throw new FileSystemException("Error creating temporary file", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task<string> CreateTempDirectoryAsync()
        {
            try
            {
                string tempDir = await GetTempDirectoryAsync();
                string randomDir = Path.Combine(tempDir, Path.GetRandomFileName());

                Directory.CreateDirectory(randomDir);
                return randomDir;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error creating temporary directory", ErrorSeverity.Warning, false);
                throw new FileSystemException("Error creating temporary directory", ex);
            }
        }

        #endregion

        #region UI Operations

        /// <inheritdoc />
        public virtual async Task<string> PickFileAsync(string title, string filter)
        {
            // This should be overridden by platform-specific implementations
            throw new PlatformNotSupportedException("File picking is not implemented in the base class. A platform-specific implementation is required.");
        }

        /// <inheritdoc />
        public virtual async Task<string> PickFolderAsync(string title)
        {
            // This should be overridden by platform-specific implementations
            throw new PlatformNotSupportedException("Folder picking is not implemented in the base class. A platform-specific implementation is required.");
        }

        /// <inheritdoc />
        public virtual async Task<string> PickSaveFileAsync(string title, string suggestedName, string filter)
        {
            // This should be overridden by platform-specific implementations
            throw new PlatformNotSupportedException("Save file picking is not implemented in the base class. A platform-specific implementation is required.");
        }

        /// <inheritdoc />
        public virtual async Task<string[]> PickFilesAsync(string title, string filter)
        {
            // This should be overridden by platform-specific implementations
            throw new PlatformNotSupportedException("Multiple file picking is not implemented in the base class. A platform-specific implementation is required.");
        }

        #endregion

        #region File Monitoring

        /// <inheritdoc />
        public virtual async Task<string> StartMonitoringAsync(string path, bool includeSubdirectories = false, string filter = "*")
        {
            try
            {
                // Validate the path exists
                bool isDirectory = await DirectoryExistsAsync(path);
                bool isFile = !isDirectory && await FileExistsAsync(path);

                if (!isDirectory && !isFile)
                {
                    throw new FileNotFoundException($"Path not found: {path}", path);
                }

                // Generate a unique token
                string monitorToken = Guid.NewGuid().ToString();
                string pathToMonitor = path;

                // If this is a file, monitor the directory containing the file
                if (isFile)
                {
                    pathToMonitor = GetDirectoryName(path);
                    filter = GetFileName(path);
                }

                // Create watcher
                FileSystemWatcher watcher = new FileSystemWatcher(pathToMonitor)
                {
                    Filter = filter,
                    IncludeSubdirectories = includeSubdirectories,
                    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
                };

                // Register for events
                watcher.Changed += (sender, e) => OnFileChanged(e.FullPath, FileSystemChangeType.Modified, DateTime.Now);
                watcher.Created += (sender, e) => OnFileChanged(e.FullPath, FileSystemChangeType.Created, DateTime.Now);
                watcher.Deleted += (sender, e) => OnFileChanged(e.FullPath, FileSystemChangeType.Deleted, DateTime.Now);
                watcher.Renamed += (sender, e) => OnFileChanged(e.FullPath, FileSystemChangeType.Renamed, DateTime.Now);
                watcher.Error += (sender, e) => OnFileChanged(path, FileSystemChangeType.Error, DateTime.Now);

                // Start monitoring
                watcher.EnableRaisingEvents = true;

                // Store the watcher
                _watchers[monitorToken] = watcher;

                return monitorToken;
            }
            catch (FileNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Path not found for monitoring: {path}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (DirectoryNotFoundException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Directory not found for monitoring: {path}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error starting monitoring for: {path}", ErrorSeverity.Error, false);
                throw new FileSystemException($"Error starting monitoring for: {path}", ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task StopMonitoringAsync(string monitorToken)
        {
            try
            {
                if (_watchers.TryGetValue(monitorToken, out var watcher))
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    _watchers.Remove(monitorToken);
                    await Task.CompletedTask;
                }
                else
                {
                    throw new ArgumentException($"Monitoring token not found: {monitorToken}", nameof(monitorToken));
                }
            }
            catch (ArgumentException ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Invalid monitoring token: {monitorToken}", ErrorSeverity.Warning, false);
                throw;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, $"Error stopping monitoring for token: {monitorToken}", ErrorSeverity.Warning, false);
                throw new FileSystemException($"Error stopping monitoring for token: {monitorToken}", ex);
            }
        }

        /// <summary>
        /// Raises the FileChanged event.
        /// </summary>
        /// <param name="path">The path of the file that changed.</param>
        /// <param name="changeType">The type of change.</param>
        /// <param name="changeTime">The time the change occurred.</param>
        protected virtual void OnFileChanged(string path, FileSystemChangeType changeType, DateTime changeTime)
        {
            try
            {
                _logService.LogDebug(LogTag, $"File changed: {path}, Type: {changeType}, Time: {changeTime}");
                FileChanged?.Invoke(this, new FileSystemEventArgs(path, changeType, changeTime));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, LogTag, "Error handling file change event", ErrorSeverity.Error, false);
            }
        }

        #endregion
    }
}
