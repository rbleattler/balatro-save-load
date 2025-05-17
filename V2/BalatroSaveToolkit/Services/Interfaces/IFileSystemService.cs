namespace BalatroSaveToolkit.Services.Interfaces
{
    /// <summary>
    /// Comprehensive interface for file system operations that abstracts platform-specific details.
    /// This interface provides methods for handling files, directories, paths, and monitoring.
    /// </summary>
    public interface IFileSystemService
    {
        #region File Operations

        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        /// <exception cref="FileSystemException">Thrown when the check cannot be completed due to an error.</exception>
        Task<bool> FileExistsAsync(string path);

        /// <summary>
        /// Reads all text from a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to read from.</param>
        /// <param name="encoding">The encoding to use, or null to use the default encoding.</param>
        /// <returns>The contents of the file as a string.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
        /// <exception cref="FileAccessException">Thrown when the file cannot be accessed.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> ReadTextAsync(string path, System.Text.Encoding? encoding = null);

        /// <summary>
        /// Reads all bytes from a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to read from.</param>
        /// <returns>The contents of the file as a byte array.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
        /// <exception cref="FileAccessException">Thrown when the file cannot be accessed.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<byte[]> ReadBytesAsync(string path);

        /// <summary>
        /// Writes all text to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        /// <param name="content">The string content to write.</param>
        /// <param name="encoding">The encoding to use, or null to use the default encoding.</param>
        /// <param name="overwrite">Whether to overwrite the file if it already exists.</param>
        /// <exception cref="FileAccessException">Thrown when the file cannot be accessed.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        /// <exception cref="FileAlreadyExistsException">Thrown when the file exists and overwrite is false.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task WriteTextAsync(string path, string content, System.Text.Encoding? encoding = null, bool overwrite = true);

        /// <summary>
        /// Writes all bytes to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        /// <param name="bytes">The byte array to write.</param>
        /// <param name="overwrite">Whether to overwrite the file if it already exists.</param>
        /// <exception cref="FileAccessException">Thrown when the file cannot be accessed.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        /// <exception cref="FileAlreadyExistsException">Thrown when the file exists and overwrite is false.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task WriteBytesAsync(string path, byte[] bytes, bool overwrite = true);

        /// <summary>
        /// Appends text to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to append to.</param>
        /// <param name="content">The string content to append.</param>
        /// <param name="encoding">The encoding to use, or null to use the default encoding.</param>
        /// <exception cref="FileAccessException">Thrown when the file cannot be accessed.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task AppendTextAsync(string path, string content, System.Text.Encoding? encoding = null);

        /// <summary>
        /// Copies a file from one location to another asynchronously.
        /// </summary>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it already exists.</param>
        /// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
        /// <exception cref="FileAccessException">Thrown when the file cannot be accessed.</exception>
        /// <exception cref="FileAlreadyExistsException">Thrown when the destination file exists and overwrite is false.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite = false);

        /// <summary>
        /// Moves a file from one location to another asynchronously.
        /// </summary>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it already exists.</param>
        /// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
        /// <exception cref="FileAccessException">Thrown when the file cannot be accessed.</exception>
        /// <exception cref="FileAlreadyExistsException">Thrown when the destination file exists and overwrite is false.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task MoveFileAsync(string sourcePath, string destinationPath, bool overwrite = false);

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
        /// <exception cref="FileAccessException">Thrown when the file cannot be accessed or deleted.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task DeleteFileAsync(string path);

        /// <summary>
        /// Gets information about a file asynchronously.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>A FileInfo object containing information about the file.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
        /// <exception cref="FileAccessException">Thrown when the file information cannot be accessed.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<FileInfo> GetFileInfoAsync(string path);

        /// <summary>
        /// Gets a hash of the file contents asynchronously.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="algorithm">The hash algorithm to use (MD5, SHA1, SHA256, etc.).</param>
        /// <returns>A string representation of the file hash.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
        /// <exception cref="FileAccessException">Thrown when the file cannot be accessed.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> GetFileHashAsync(string path, string algorithm = "SHA256");

        /// <summary>
        /// Creates a file at the specified path if it doesn't already exist.
        /// </summary>
        /// <param name="path">The path of the file to create.</param>
        /// <returns>True if the file was created; false if it already exists.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        /// <exception cref="FileAccessException">Thrown when the file cannot be created.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<bool> CreateFileAsync(string path);

        #endregion

        #region Directory Operations

        /// <summary>
        /// Checks if a directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the directory exists; otherwise, false.</returns>
        /// <exception cref="FileSystemException">Thrown when the check cannot be completed due to an error.</exception>
        Task<bool> DirectoryExistsAsync(string path);

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        /// <param name="recursive">Whether to create all parent directories if they don't exist.</param>
        /// <returns>True if the directory was created; false if it already exists.</returns>
        /// <exception cref="DirectoryAccessException">Thrown when the directory cannot be created.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<bool> CreateDirectoryAsync(string path, bool recursive = true);

        /// <summary>
        /// Deletes a directory asynchronously.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <param name="recursive">Whether to delete subdirectories and files.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        /// <exception cref="DirectoryAccessException">Thrown when the directory cannot be accessed or deleted.</exception>
        /// <exception cref="DirectoryNotEmptyException">Thrown when the directory is not empty and recursive is false.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task DeleteDirectoryAsync(string path, bool recursive = false);

        /// <summary>
        /// Gets all files in a directory asynchronously.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <param name="searchPattern">The search pattern to match against file names.</param>
        /// <param name="recursive">Whether to include files in subdirectories.</param>
        /// <returns>A collection of file paths.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        /// <exception cref="DirectoryAccessException">Thrown when the directory cannot be accessed.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<IEnumerable<string>> GetFilesAsync(string path, string searchPattern = "*", bool recursive = false);

        /// <summary>
        /// Gets all subdirectories in a directory asynchronously.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <param name="searchPattern">The search pattern to match against directory names.</param>
        /// <param name="recursive">Whether to include directories in subdirectories.</param>
        /// <returns>A collection of directory paths.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        /// <exception cref="DirectoryAccessException">Thrown when the directory cannot be accessed.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<IEnumerable<string>> GetDirectoriesAsync(string path, string searchPattern = "*", bool recursive = false);

        /// <summary>
        /// Gets information about a directory asynchronously.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <returns>A DirectoryInfo object containing information about the directory.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        /// <exception cref="DirectoryAccessException">Thrown when the directory information cannot be accessed.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<DirectoryInfo> GetDirectoryInfoAsync(string path);

        #endregion

        #region Path Operations

        /// <summary>
        /// Combines multiple path parts into a single path.
        /// </summary>
        /// <param name="parts">The path parts to combine.</param>
        /// <returns>A combined path.</returns>
        string CombinePaths(params string[] parts);

        /// <summary>
        /// Gets the directory name from a path.
        /// </summary>
        /// <param name="path">The path to extract from.</param>
        /// <returns>The directory name.</returns>
        string GetDirectoryName(string path);

        /// <summary>
        /// Gets the file name from a path.
        /// </summary>
        /// <param name="path">The path to extract from.</param>
        /// <returns>The file name.</returns>
        string GetFileName(string path);

        /// <summary>
        /// Gets the file name without extension from a path.
        /// </summary>
        /// <param name="path">The path to extract from.</param>
        /// <returns>The file name without extension.</returns>
        string GetFileNameWithoutExtension(string path);

        /// <summary>
        /// Gets the extension from a path.
        /// </summary>
        /// <param name="path">The path to extract from.</param>
        /// <returns>The extension.</returns>
        string GetExtension(string path);

        /// <summary>
        /// Normalizes a path to the current platform's format.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>A normalized path.</returns>
        string NormalizePath(string path);

        /// <summary>
        /// Gets the absolute path from a relative path.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="basePath">The base path (optional).</param>
        /// <returns>The absolute path.</returns>
        string GetAbsolutePath(string relativePath, string? basePath = null);

        /// <summary>
        /// Gets the relative path from one path to another.
        /// </summary>
        /// <param name="relativeTo">The path to make relative to.</param>
        /// <param name="path">The path to make relative.</param>
        /// <returns>The relative path.</returns>
        string GetRelativePath(string relativeTo, string path);

        #endregion

        #region Platform-Specific Operations

        /// <summary>
        /// Gets the path to the Balatro save directory for the current platform.
        /// </summary>
        /// <returns>The path to the Balatro save directory.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the save directory cannot be found.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> GetBalatroSaveDirectoryAsync();

        /// <summary>
        /// Gets the path to the application data directory for the current platform.
        /// </summary>
        /// <param name="appName">The application name to use for the directory (optional).</param>
        /// <returns>The path to the application data directory.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> GetApplicationDataDirectoryAsync(string? appName = null);

        /// <summary>
        /// Gets the path to the temporary directory for the current platform.
        /// </summary>
        /// <returns>The path to the temporary directory.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> GetTempDirectoryAsync();

        /// <summary>
        /// Creates a unique temporary file and returns its path.
        /// </summary>
        /// <param name="extension">The extension for the temporary file.</param>
        /// <returns>The path to the temporary file.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> CreateTempFileAsync(string? extension = null);

        /// <summary>
        /// Creates a unique temporary directory and returns its path.
        /// </summary>
        /// <returns>The path to the temporary directory.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> CreateTempDirectoryAsync();

        #endregion

        #region UI Operations

        /// <summary>
        /// Shows a file picker dialog and returns the selected file path.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="filter">The file filter to apply.</param>
        /// <returns>The selected file path, or an empty string if canceled.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> PickFileAsync(string title, string filter);

        /// <summary>
        /// Shows a folder picker dialog and returns the selected folder path.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <returns>The selected folder path, or an empty string if canceled.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> PickFolderAsync(string title);

        /// <summary>
        /// Shows a save file dialog and returns the selected file path.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="suggestedName">The suggested file name.</param>
        /// <param name="filter">The file filter to apply.</param>
        /// <returns>The selected file path, or an empty string if canceled.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> PickSaveFileAsync(string title, string suggestedName, string filter);

        /// <summary>
        /// Shows multiple file picker dialog and returns the selected file paths.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="filter">The file filter to apply.</param>
        /// <returns>The selected file paths, or an empty array if canceled.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string[]> PickFilesAsync(string title, string filter);

        #endregion

        #region File Monitoring

        /// <summary>
        /// Starts monitoring a file or directory for changes.
        /// </summary>
        /// <param name="path">The path to monitor.</param>
        /// <param name="includeSubdirectories">Whether to monitor subdirectories.</param>
        /// <param name="filter">The file pattern to filter.</param>
        /// <returns>A monitoring token that can be used to stop monitoring.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory does not exist.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task<string> StartMonitoringAsync(string path, bool includeSubdirectories = false, string filter = "*");

        /// <summary>
        /// Stops monitoring a file or directory for changes.
        /// </summary>
        /// <param name="monitorToken">The monitoring token returned from StartMonitoringAsync.</param>
        /// <exception cref="ArgumentException">Thrown when the monitoring token is invalid.</exception>
        /// <exception cref="FileSystemException">Thrown for other file system errors.</exception>
        Task StopMonitoringAsync(string monitorToken);

        /// <summary>
        /// Event raised when a monitored file or directory changes.
        /// </summary>
        event EventHandler<FileSystemEventArgs> FileChanged;

        #endregion
    }

    #region Exception Types

    /// <summary>
    /// Base exception for file system errors.
    /// </summary>
    public class FileSystemException : Exception
    {
        public FileSystemException(string message) : base(message) { }
        public FileSystemException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when a file cannot be accessed due to permissions or locks.
    /// </summary>
    public class FileAccessException : FileSystemException
    {
        public string FilePath { get; private set; }

        public FileAccessException(string filePath, string message) : base(message)
        {
            FilePath = filePath;
        }

        public FileAccessException(string filePath, string message, Exception innerException) : base(message, innerException)
        {
            FilePath = filePath;
        }
    }

    /// <summary>
    /// Exception thrown when a directory cannot be accessed due to permissions or locks.
    /// </summary>
    public class DirectoryAccessException : FileSystemException
    {
        public string DirectoryPath { get; private set; }

        public DirectoryAccessException(string directoryPath, string message) : base(message)
        {
            DirectoryPath = directoryPath;
        }

        public DirectoryAccessException(string directoryPath, string message, Exception innerException) : base(message, innerException)
        {
            DirectoryPath = directoryPath;
        }
    }

    /// <summary>
    /// Exception thrown when a file already exists and cannot be overwritten.
    /// </summary>
    public class FileAlreadyExistsException : FileSystemException
    {
        public string FilePath { get; private set; }

        public FileAlreadyExistsException(string filePath, string message) : base(message)
        {
            FilePath = filePath;
        }

        public FileAlreadyExistsException(string filePath, string message, Exception innerException) : base(message, innerException)
        {
            FilePath = filePath;
        }
    }

    /// <summary>
    /// Exception thrown when a directory is not empty and cannot be deleted.
    /// </summary>
    public class DirectoryNotEmptyException : FileSystemException
    {
        public string DirectoryPath { get; private set; }

        public DirectoryNotEmptyException(string directoryPath, string message) : base(message)
        {
            DirectoryPath = directoryPath;
        }

        public DirectoryNotEmptyException(string directoryPath, string message, Exception innerException) : base(message, innerException)
        {
            DirectoryPath = directoryPath;
        }
    }

    /// <summary>
    /// Exception thrown when a path is too long for the platform.
    /// </summary>
    public class PathTooLongException : FileSystemException
    {
        public string Path { get; private set; }
        public int MaxPathLength { get; private set; }

        public PathTooLongException(string path, int maxPathLength, string message) : base(message)
        {
            Path = path;
            MaxPathLength = maxPathLength;
        }

        public PathTooLongException(string path, int maxPathLength, string message, Exception innerException) : base(message, innerException)
        {
            Path = path;
            MaxPathLength = maxPathLength;
        }
    }

    /// <summary>
    /// Exception thrown when a file name contains invalid characters.
    /// </summary>
    public class InvalidFileNameException : FileSystemException
    {
        public string FileName { get; private set; }

        public InvalidFileNameException(string fileName, string message) : base(message)
        {
            FileName = fileName;
        }

        public InvalidFileNameException(string fileName, string message, Exception innerException) : base(message, innerException)
        {
            FileName = fileName;
        }
    }

    #endregion

    /// <summary>
    /// Arguments for file system change events.
    /// </summary>
    public class FileSystemEventArgs : EventArgs
    {
        /// <summary>
        /// The path of the file or directory that changed.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The type of change that occurred.
        /// </summary>
        public FileSystemChangeType ChangeType { get; }

        /// <summary>
        /// The time the change occurred.
        /// </summary>
        public DateTime ChangeTime { get; }

        public FileSystemEventArgs(string path, FileSystemChangeType changeType, DateTime changeTime)
        {
            Path = path;
            ChangeType = changeType;
            ChangeTime = changeTime;
        }
    }

    /// <summary>
    /// Enum representing different types of file system changes.
    /// </summary>
    public enum FileSystemChangeType
    {
        /// <summary>
        /// A file or directory was created.
        /// </summary>
        Created,

        /// <summary>
        /// A file or directory was deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// A file or directory was renamed.
        /// </summary>
        Renamed,

        /// <summary>
        /// A file or directory was modified.
        /// </summary>
        Modified,

        /// <summary>
        /// A file or directory had its attributes changed.
        /// </summary>
        AttributeChanged,

        /// <summary>
        /// A file or directory had its security settings changed.
        /// </summary>
        SecurityChanged,

        /// <summary>
        /// A file or directory monitoring error occurred.
        /// </summary>
        Error
    }
}
