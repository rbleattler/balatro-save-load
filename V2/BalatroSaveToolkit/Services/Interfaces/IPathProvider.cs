namespace BalatroSaveToolkit.Services.Interfaces
{
    /// <summary>
    /// Interface for platform-specific file path resolution.
    /// This interface provides methods to resolve common application paths
    /// and Balatro save file locations on different platforms.
    /// </summary>
    public interface IPathProvider
    {
        /// <summary>
        /// Gets the path to the Balatro save directory for the current platform.
        /// </summary>
        /// <param name="steamInstallation">If true, attempts to locate Steam installation save files.</param>
        /// <returns>The path to the Balatro save directory.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the save directory cannot be found.</exception>
        Task<string> GetBalatroSaveDirectoryAsync(bool steamInstallation = false);

        /// <summary>
        /// Gets the path to the application data directory for this application.
        /// </summary>
        /// <param name="appName">The application name to use for the directory (optional).</param>
        /// <returns>The path to the application data directory.</returns>
        /// <exception cref="FileSystemException">Thrown for file system errors.</exception>
        Task<string> GetApplicationDataDirectoryAsync(string? appName = null);

        /// <summary>
        /// Gets the path to the temporary directory for the current platform.
        /// </summary>
        /// <returns>The path to the temporary directory.</returns>
        /// <exception cref="FileSystemException">Thrown for file system errors.</exception>
        Task<string> GetTempDirectoryAsync();

        /// <summary>
        /// Gets the path to the user's documents directory.
        /// </summary>
        /// <returns>The path to the documents directory.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the documents directory cannot be found.</exception>
        Task<string> GetDocumentsDirectoryAsync();

        /// <summary>
        /// Gets the path to the user's desktop directory.
        /// </summary>
        /// <returns>The path to the desktop directory.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the desktop directory cannot be found.</exception>
        Task<string> GetDesktopDirectoryAsync();

        /// <summary>
        /// Gets the path to the user's home directory.
        /// </summary>
        /// <returns>The path to the user's home directory.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the home directory cannot be found.</exception>
        Task<string> GetUserHomeDirectoryAsync();

        /// <summary>
        /// Gets the path to the directory where save backups should be stored.
        /// </summary>
        /// <returns>The path to the directory for storing save backups.</returns>
        /// <exception cref="FileSystemException">Thrown for file system errors.</exception>
        Task<string> GetSaveBackupsDirectoryAsync();

        /// <summary>
        /// Gets the path to the logs directory.
        /// </summary>
        /// <returns>The path to the logs directory.</returns>
        /// <exception cref="FileSystemException">Thrown for file system errors.</exception>
        Task<string> GetLogsDirectoryAsync();

        /// <summary>
        /// Gets the path to the export directory for save files.
        /// </summary>
        /// <returns>The path to the export directory.</returns>
        /// <exception cref="FileSystemException">Thrown for file system errors.</exception>
        Task<string> GetExportDirectoryAsync();

        /// <summary>
        /// Resolves platform-specific special paths.
        /// </summary>
        /// <param name="specialFolder">The special folder to resolve.</param>
        /// <returns>The path to the special folder.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the special folder cannot be found.</exception>
        Task<string> GetSpecialFolderPathAsync(Environment.SpecialFolder specialFolder);

        /// <summary>
        /// Checks if the provided path appears to be a valid Balatro save file.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the path appears to be a valid Balatro save file; otherwise, false.</returns>
        Task<bool> IsBalatroSaveFilePathAsync(string path);

        /// <summary>
        /// Gets the platform-appropriate path separator character.
        /// </summary>
        char PathSeparator { get; }

        /// <summary>
        /// Determines if the platform's file system is case-sensitive.
        /// </summary>
        bool IsCaseSensitive { get; }
    }
}
