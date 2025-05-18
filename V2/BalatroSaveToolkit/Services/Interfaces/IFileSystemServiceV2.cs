using System.IO;
using Microsoft.Maui.Storage;

namespace BalatroSaveToolkit.Services.Interfaces
{
    /// <summary>
    /// Interface for platform-specific file system operations that complement MAUI's built-in FileSystem APIs.
    /// This focused interface handles operations specific to the Balatro Save Toolkit that aren't covered by
    /// standard .NET APIs or MAUI's FileSystem.
    /// </summary>
    /// <remarks>
    /// For standard file operations, use .NET's File and Directory classes directly.
    /// For common directories, use MAUI's FileSystem.Current (AppDataDirectory, CacheDirectory).
    /// </remarks>
    public interface IFileSystemService
    {
        /// <summary>
        /// Gets the path to the Balatro save directory for the current platform.
        /// </summary>
        /// <returns>The path to the Balatro save directory.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the save directory cannot be found.</exception>
        Task<string> GetBalatroSaveDirectoryAsync();

        /// <summary>
        /// Gets an application-specific directory that is appropriate for storing user settings and files.
        /// This is a wrapper around MAUI's FileSystem.AppDataDirectory with optional subfolder creation.
        /// </summary>
        /// <param name="appName">Optional application name for a subdirectory.</param>
        /// <returns>The path to the application data directory.</returns>
        Task<string> GetApplicationDataDirectoryAsync(string? appName = null);

        /// <summary>
        /// Creates a unique temporary file and returns its path.
        /// </summary>
        /// <param name="extension">The extension for the temporary file.</param>
        /// <returns>The path to the temporary file.</returns>
        Task<string> CreateTempFileAsync(string? extension = null);

        /// <summary>
        /// Creates a unique temporary directory and returns its path.
        /// </summary>
        /// <returns>The path to the temporary directory.</returns>
        Task<string> CreateTempDirectoryAsync();

        /// <summary>
        /// Shows a file picker dialog and returns the selected file path.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="filter">The file filter to apply.</param>
        /// <returns>The selected file path, or an empty string if canceled.</returns>
        Task<string> PickFileAsync(string title, string filter);

        /// <summary>
        /// Shows a folder picker dialog and returns the selected folder path.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <returns>The selected folder path, or an empty string if canceled.</returns>
        Task<string> PickFolderAsync(string title);

        /// <summary>
        /// Shows a save file dialog and returns the selected file path.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="suggestedName">The suggested file name.</param>
        /// <param name="filter">The file filter to apply.</param>
        /// <returns>The selected file path, or an empty string if canceled.</returns>
        Task<string> PickSaveFileAsync(string title, string suggestedName, string filter);

        /// <summary>
        /// Starts monitoring a file or directory for changes.
        /// </summary>
        /// <param name="path">The path to monitor.</param>
        /// <param name="includeSubdirectories">Whether to monitor subdirectories.</param>
        /// <param name="filter">The file pattern to filter.</param>
        /// <returns>A monitoring token that can be used to stop monitoring.</returns>
        Task<string> StartMonitoringAsync(string path, bool includeSubdirectories = false, string filter = "*");

        /// <summary>
        /// Stops monitoring a file or directory for changes.
        /// </summary>
        /// <param name="monitorToken">The monitoring token returned from StartMonitoringAsync.</param>
        Task StopMonitoringAsync(string monitorToken);

        /// <summary>
        /// Event raised when a monitored file or directory changes.
        /// </summary>
        event EventHandler<FileSystemEventArgs> FileChanged;
    }

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
        Modified
    }
}
