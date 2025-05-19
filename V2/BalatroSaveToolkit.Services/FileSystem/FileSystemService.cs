using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Services.FileSystem
{
    /// <summary>
    /// Base implementation of the IFileSystemService interface.
    /// </summary>
    public abstract class FileSystemService : IFileSystemService
    {
        private const string APP_DIRECTORY_NAME = "BalatroSaveAndLoad";
        
        /// <inheritdoc/>
        public string ApplicationDataDirectory { get; }
        
        /// <inheritdoc/>
        public abstract string BalatroSaveDirectory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemService"/> class.
        /// </summary>
        protected FileSystemService()
        {
            ApplicationDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                APP_DIRECTORY_NAME);
            
            Directory.CreateDirectory(ApplicationDataDirectory);
        }

        /// <inheritdoc/>
        public string GetCurrentSaveFilePath(int profileNumber)
        {
            if (profileNumber < 1 || profileNumber > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(profileNumber), "Profile number must be between 1 and 4.");
            }

            return Path.Combine(BalatroSaveDirectory, $"profile{profileNumber}.userdata");
        }

        /// <inheritdoc/>
        public string GetBackupFilePath(int profileNumber, DateTime timestamp)
        {
            var fileName = $"profile{profileNumber}_{timestamp:yyyyMMdd_HHmmss}.userdata";
            return Path.Combine(ApplicationDataDirectory, fileName);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SaveFileInfo>> GetSaveFilesAsync(int profileNumber)
        {
            var result = new List<SaveFileInfo>();
            var directory = new DirectoryInfo(ApplicationDataDirectory);

            if (!directory.Exists)
            {
                return result;
            }

            var pattern = $"profile{profileNumber}_*.userdata";
            var files = directory.GetFiles(pattern);
            
            foreach (var file in files)
            {
                try
                {
                    // Parse the timestamp from the filename
                    var nameParts = Path.GetFileNameWithoutExtension(file.Name).Split('_');
                    if (nameParts.Length < 3)
                    {
                        continue;
                    }

                    if (!DateTime.TryParseExact(
                        $"{nameParts[1]}_{nameParts[2]}", 
                        "yyyyMMdd_HHmmss", 
                        null, 
                        System.Globalization.DateTimeStyles.None, 
                        out var timestamp))
                    {
                        continue;
                    }

                    result.Add(new SaveFileInfo
                    {
                        FilePath = file.FullName,
                        ProfileNumber = profileNumber,
                        Timestamp = timestamp,
                        FileSize = file.Length
                    });
                }
                catch
                {
                    // Skip files that don't match the expected format
                    continue;
                }
            }

            // Sort by timestamp (newest first)
            result.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            
            return await Task.FromResult(result);
        }

        /// <inheritdoc/>
        public async Task<bool> SaveFileExistsAsync(int profileNumber)
        {
            var path = GetCurrentSaveFilePath(profileNumber);
            return await Task.FromResult(File.Exists(path));
        }

        /// <inheritdoc/>
        public async Task<byte[]> ReadSaveFileAsync(int profileNumber)
        {
            var path = GetCurrentSaveFilePath(profileNumber);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Save file not found", path);
            }

            return await File.ReadAllBytesAsync(path);
        }

        /// <inheritdoc/>
        public async Task WriteSaveFileAsync(int profileNumber, byte[] data)
        {
            var path = GetCurrentSaveFilePath(profileNumber);
            await File.WriteAllBytesAsync(path, data);
        }

        /// <inheritdoc/>
        public async Task BackupSaveFileAsync(int profileNumber)
        {
            if (!await SaveFileExistsAsync(profileNumber))
            {
                throw new FileNotFoundException("Save file not found");
            }

            var saveData = await ReadSaveFileAsync(profileNumber);
            var backupPath = GetBackupFilePath(profileNumber, DateTime.Now);
            await File.WriteAllBytesAsync(backupPath, saveData);
        }

        /// <inheritdoc/>
        public async Task RestoreSaveFileAsync(int profileNumber, string backupFilePath)
        {
            if (!File.Exists(backupFilePath))
            {
                throw new FileNotFoundException("Backup file not found", backupFilePath);
            }

            var backupData = await File.ReadAllBytesAsync(backupFilePath);
            var currentPath = GetCurrentSaveFilePath(profileNumber);
            await File.WriteAllBytesAsync(currentPath, backupData);
        }

        /// <inheritdoc/>
        public async Task CleanupOldSavesAsync(TimeSpan maxAge)
        {
            var cutoffDate = DateTime.Now - maxAge;
            var directory = new DirectoryInfo(ApplicationDataDirectory);
            
            if (!directory.Exists)
            {
                return;
            }

            var files = directory.GetFiles("profile*_*.userdata");
            foreach (var file in files)
            {
                try
                {
                    // Parse the timestamp from the filename
                    var nameParts = Path.GetFileNameWithoutExtension(file.Name).Split('_');
                    if (nameParts.Length < 3)
                    {
                        continue;
                    }

                    if (!DateTime.TryParseExact(
                        $"{nameParts[1]}_{nameParts[2]}", 
                        "yyyyMMdd_HHmmss", 
                        null, 
                        System.Globalization.DateTimeStyles.None, 
                        out var timestamp))
                    {
                        continue;
                    }

                    if (timestamp < cutoffDate)
                    {
                        file.Delete();
                    }
                }
                catch
                {
                    // Skip files that cause errors
                    continue;
                }
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<long> GetAvailableDiskSpaceAsync(string path)
        {
            var driveInfo = new DriveInfo(Path.GetPathRoot(path));
            return await Task.FromResult(driveInfo.AvailableFreeSpace);
        }

        /// <inheritdoc/>
        public void OpenFileExplorer(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            if (!Directory.Exists(path) && !File.Exists(path))
            {
                throw new DirectoryNotFoundException($"Path not found: {path}");
            }

            OpenFileExplorerPlatformSpecific(path);
        }

        /// <summary>
        /// Platform-specific implementation to open a file explorer.
        /// </summary>
        /// <param name="path">The path to open in the file explorer.</param>
        protected abstract void OpenFileExplorerPlatformSpecific(string path);

        /// <inheritdoc/>
        public abstract void StartFileWatcher(int profileNumber, Action onChanged);

        /// <inheritdoc/>
        public abstract void StopFileWatcher();
    }
}
