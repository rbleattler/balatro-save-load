using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Tests.Mocks
{
    /// <summary>
    /// A mock implementation of IFileSystemService for testing.
    /// </summary>
    public class MockFileSystemService : IFileSystemService
    {
        private Dictionary<int, string> _currentSaveFiles = new Dictionary<int, string>();
        private List<string> _backupFiles = new List<string>();
        private Dictionary<string, string> _fileContents = new Dictionary<string, string>();

        /// <summary>
        /// Gets a value indicating whether the save directory was opened.
        /// </summary>
        public bool SaveDirectoryOpened { get; private set; }

        /// <summary>
        /// Gets the application data directory path for the application.
        /// </summary>
        public string ApplicationDataDirectory => "/mock/appdata";

        /// <summary>
        /// Gets the Balatro save file directory path.
        /// </summary>
        public string BalatroSaveDirectory => "/mock/balatro/saves";

        /// <summary>
        /// Sets up mock backup files for testing.
        /// </summary>
        /// <param name="backupFiles">The list of mock backup files.</param>
        public void SetupBackupFiles(List<string> backupFiles)
        {
            _backupFiles = backupFiles ?? new List<string>();
        }

        /// <summary>
        /// Sets up mock current save files for testing.
        /// </summary>
        /// <param name="profileNumber">The profile number.</param>
        /// <param name="filePath">The mock file path.</param>
        public void SetupCurrentSaveFile(int profileNumber, string filePath)
        {
            _currentSaveFiles[profileNumber] = filePath;
        }

        /// <summary>
        /// Sets up mock file content for testing.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="content">The mock content.</param>
        public void SetupFileContent(string filePath, string content)
        {
            _fileContents[filePath] = content;
        }

        /// <summary>
        /// Gets the path to the current Balatro save file.
        /// </summary>
        /// <param name="profileNumber">The profile number (1-based).</param>
        /// <returns>The path to the current Balatro save file.</returns>
        public string GetCurrentSaveFilePath(int profileNumber)
        {
            return _currentSaveFiles.TryGetValue(profileNumber, out string filePath)
                ? filePath
                : $"/mock/balatro/saves/profile{profileNumber}.sav";
        }

        /// <summary>
        /// Gets a list of all saved backup files.
        /// </summary>
        /// <returns>A list of file paths to the backup files.</returns>
        public Task<List<string>> GetSavedBackupFilesAsync()
        {
            return Task.FromResult(_backupFiles);
        }

        /// <summary>
        /// Creates a backup of the current save file.
        /// </summary>
        /// <param name="profileNumber">The profile number to backup.</param>
        /// <returns>The path to the created backup file.</returns>
        public string BackupSaveFile(int profileNumber)
        {
            string currentSavePath = GetCurrentSaveFilePath(profileNumber);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupPath = $"/mock/balatro/backups/profile{profileNumber}_{timestamp}.sav";

            _backupFiles.Add(backupPath);

            if (_fileContents.TryGetValue(currentSavePath, out string content))
            {
                _fileContents[backupPath] = content;
            }

            return backupPath;
        }

        /// <summary>
        /// Restores a save file from a backup.
        /// </summary>
        /// <param name="backupFilePath">The path to the backup file.</param>
        /// <param name="profileNumber">The profile number to restore to.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RestoreSaveFile(string backupFilePath, int profileNumber)
        {
            if (!_backupFiles.Contains(backupFilePath))
            {
                return false;
            }

            string currentSavePath = GetCurrentSaveFilePath(profileNumber);

            if (_fileContents.TryGetValue(backupFilePath, out string content))
            {
                _fileContents[currentSavePath] = content;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the current Balatro save file to a backup.
        /// </summary>
        /// <param name="profileNumber">The profile number (1-based).</param>
        /// <returns>The path to the saved backup file, or null if unsuccessful.</returns>
        public Task<string> SaveBackupAsync(int profileNumber)
        {
            string backupPath = BackupSaveFile(profileNumber);
            return Task.FromResult(backupPath);
        }

        /// <summary>
        /// Loads a backup file into the current Balatro save slot.
        /// </summary>
        /// <param name="backupFilePath">The path to the backup file.</param>
        /// <param name="profileNumber">The profile number (1-based).</param>
        /// <returns>True if successful, false otherwise.</returns>
        public Task<bool> LoadBackupAsync(string backupFilePath, int profileNumber)
        {
            bool result = RestoreSaveFile(backupFilePath, profileNumber);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Opens the save directory in the platform's file explorer.
        /// </summary>
        public void OpenSaveDirectory()
        {
            SaveDirectoryOpened = true;
        }

        /// <summary>
        /// Gets the content of a save file as a string.
        /// </summary>
        /// <param name="filePath">The path to the save file.</param>
        /// <returns>The content of the save file as a string.</returns>
        public Task<string> GetSaveFileContentAsync(string filePath)
        {
            return Task.FromResult(_fileContents.TryGetValue(filePath, out string content)
                ? content
                : string.Empty);
        }
    }
}
