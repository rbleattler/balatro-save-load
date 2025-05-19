using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Service interface for file system operations.
    /// Provides platform-agnostic access to files and directories.
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// Gets the application data directory path for the application.
        /// </summary>
        string ApplicationDataDirectory { get; }

        /// <summary>
        /// Gets the Balatro save file directory path.
        /// </summary>
        string BalatroSaveDirectory { get; }

        /// <summary>
        /// Gets the path to the current Balatro save file.
        /// </summary>
        /// <param name="profileNumber">The profile number (1-based).</param>
        /// <returns>The path to the current Balatro save file.</returns>
        string GetCurrentSaveFilePath(int profileNumber);

        /// <summary>
        /// Gets a list of all saved backup files.
        /// </summary>
        /// <returns>A list of file paths to the backup files.</returns>
        Task<List<string>> GetSavedBackupFilesAsync();

        /// <summary>
        /// Saves the current Balatro save file to a backup.
        /// </summary>
        /// <param name="profileNumber">The profile number (1-based).</param>
        /// <returns>The path to the saved backup file, or null if unsuccessful.</returns>
        Task<string> SaveBackupAsync(int profileNumber);

        /// <summary>
        /// Loads a backup file into the current Balatro save slot.
        /// </summary>
        /// <param name="backupFilePath">The path to the backup file.</param>
        /// <param name="profileNumber">The profile number (1-based).</param>
        /// <returns>True if successful, false otherwise.</returns>
        Task<bool> LoadBackupAsync(string backupFilePath, int profileNumber);

        /// <summary>
        /// Opens the save directory in the platform's file explorer.
        /// </summary>
        void OpenSaveDirectory();

        /// <summary>
        /// Gets the content of a save file as a string.
        /// </summary>
        /// <param name="filePath">The path to the save file.</param>
        /// <returns>The content of the save file as a string.</returns>
        Task<string> GetSaveFileContentAsync(string filePath);
    }
}
