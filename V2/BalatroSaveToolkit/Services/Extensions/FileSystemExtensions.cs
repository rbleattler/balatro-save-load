using System.Text;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Extensions
{
    /// <summary>
    /// Extension methods for common file operations that build on standard .NET APIs.
    /// These methods provide a convenient, async-friendly interface for file operations.
    /// </summary>
    public static class FileSystemExtensions
    {
        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        public static async Task<bool> FileExistsAsync(string path)
        {
            return await Task.FromResult(File.Exists(path));
        }

        /// <summary>
        /// Checks if a directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the directory exists; otherwise, false.</returns>
        public static async Task<bool> DirectoryExistsAsync(string path)
        {
            return await Task.FromResult(Directory.Exists(path));
        }

        /// <summary>
        /// Creates a directory at the specified path if it doesn't already exist.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        /// <param name="recursive">Whether to create all parent directories if they don't exist.</param>
        /// <returns>True if the directory was created; false if it already exists.</returns>
        public static async Task<bool> CreateDirectoryAsync(string path, bool recursive = true)
        {
            if (Directory.Exists(path))
            {
                return await Task.FromResult(false);
            }

            Directory.CreateDirectory(path);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Reads all text from a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to read from.</param>
        /// <param name="encoding">The encoding to use, or null to use the default encoding.</param>
        /// <returns>The contents of the file as a string.</returns>
        public static async Task<string> ReadTextAsync(string path, Encoding? encoding = null)
        {
            return encoding != null
                ? await File.ReadAllTextAsync(path, encoding)
                : await File.ReadAllTextAsync(path);
        }

        /// <summary>
        /// Reads all bytes from a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to read from.</param>
        /// <returns>The contents of the file as a byte array.</returns>
        public static async Task<byte[]> ReadBytesAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }

        /// <summary>
        /// Writes all text to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        /// <param name="content">The string content to write.</param>
        /// <param name="encoding">The encoding to use, or null to use the default encoding.</param>
        /// <param name="overwrite">Whether to overwrite the file if it already exists.</param>
        /// <exception cref="IOException">Thrown when the file exists and overwrite is false.</exception>
        public static async Task WriteTextAsync(string path, string content, Encoding? encoding = null, bool overwrite = true)
        {
            if (!overwrite && File.Exists(path))
            {
                throw new IOException($"File already exists: {path}");
            }

            // Ensure directory exists
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (encoding != null)
            {
                await File.WriteAllTextAsync(path, content, encoding);
            }
            else
            {
                await File.WriteAllTextAsync(path, content);
            }
        }

        /// <summary>
        /// Writes all bytes to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        /// <param name="bytes">The byte array to write.</param>
        /// <param name="overwrite">Whether to overwrite the file if it already exists.</param>
        /// <exception cref="IOException">Thrown when the file exists and overwrite is false.</exception>
        public static async Task WriteBytesAsync(string path, byte[] bytes, bool overwrite = true)
        {
            if (!overwrite && File.Exists(path))
            {
                throw new IOException($"File already exists: {path}");
            }

            // Ensure directory exists
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllBytesAsync(path, bytes);
        }

        /// <summary>
        /// Appends text to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path to append to.</param>
        /// <param name="content">The string content to append.</param>
        /// <param name="encoding">The encoding to use, or null to use the default encoding.</param>
        public static async Task AppendTextAsync(string path, string content, Encoding? encoding = null)
        {
            if (encoding != null)
            {
                await File.AppendAllTextAsync(path, content, encoding);
            }
            else
            {
                await File.AppendAllTextAsync(path, content);
            }
        }

        /// <summary>
        /// Copies a file from one location to another asynchronously.
        /// </summary>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it already exists.</param>
        public static async Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite = false)
        {
            await Task.Run(() => File.Copy(sourcePath, destinationPath, overwrite));
        }

        /// <summary>
        /// Moves a file from one location to another asynchronously.
        /// </summary>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="overwrite">Whether to overwrite the destination file if it already exists.</param>
        public static async Task MoveFileAsync(string sourcePath, string destinationPath, bool overwrite = false)
        {
            // Ensure directory exists
            string? directory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (overwrite && File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            await Task.Run(() => File.Move(sourcePath, destinationPath));
        }

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        public static async Task DeleteFileAsync(string path)
        {
            await Task.Run(() => {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            });
        }

        /// <summary>
        /// Gets all files in a directory asynchronously.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <param name="searchPattern">The search pattern to match against file names.</param>
        /// <param name="recursive">Whether to include files in subdirectories.</param>
        /// <returns>A collection of file paths.</returns>
        public static async Task<IEnumerable<string>> GetFilesAsync(string path, string searchPattern = "*", bool recursive = false)
        {
            return await Task.Run(() => Directory.GetFiles(
                path,
                searchPattern,
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
        }

        /// <summary>
        /// Gets all subdirectories in a directory asynchronously.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <param name="searchPattern">The search pattern to match against directory names.</param>
        /// <param name="recursive">Whether to include directories in subdirectories.</param>
        /// <returns>A collection of directory paths.</returns>        public static async Task<IEnumerable<string>> GetDirectoriesAsync(string path, string searchPattern = "*", bool recursive = false)
        {
            return await Task.Run(() => Directory.GetDirectories(
                path,
                searchPattern,
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
        }

        #region IFileSystemService Extensions

        /// <summary>
        /// Gets a unique file path by appending a number if the file already exists.
        /// </summary>
        /// <param name="fileSystemService">The file system service.</param>
        /// <param name="filePath">The original file path.</param>
        /// <returns>A unique file path.</returns>
        public static Task<string> GetUniqueFilePathAsync(this IFileSystemService fileSystemService, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                return Task.FromResult(filePath);

            string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            int counter = 1;

            string newPath;
            do
            {
                newPath = Path.Combine(directory, $"{fileNameWithoutExtension} ({counter}){extension}");
                counter++;
            }
            while (File.Exists(newPath));

            return Task.FromResult(newPath);
        }

        /// <summary>
        /// Cleans up files older than a specified age in a directory.
        /// </summary>
        /// <param name="fileSystemService">The file system service.</param>
        /// <param name="directory">The directory to clean.</param>
        /// <param name="maxAgeInDays">The maximum age in days for files to keep.</param>
        /// <param name="filePattern">The file pattern to match (default: "*").</param>
        /// <returns>The number of files deleted.</returns>
        public static Task<int> CleanupOldFilesAsync(this IFileSystemService fileSystemService, string directory, int maxAgeInDays, string filePattern = "*")
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentException("Directory cannot be null or empty", nameof(directory));

            if (maxAgeInDays <= 0)
                throw new ArgumentException("Max age must be positive", nameof(maxAgeInDays));

            if (!Directory.Exists(directory))
                return Task.FromResult(0);

            DateTime cutoffDate = DateTime.Now.AddDays(-maxAgeInDays);
            string[] files = Directory.GetFiles(directory, filePattern);
            int deletedCount = 0;

            foreach (string file in files)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < cutoffDate)
                    {
                        fileInfo.Delete();
                        deletedCount++;
                    }
                }
                catch
                {
                    // Skip files that can't be deleted
                }
            }

            return Task.FromResult(deletedCount);
        }

        /// <summary>
        /// Gets information about a save file profile.
        /// </summary>
        /// <param name="fileSystemService">The file system service.</param>
        /// <param name="profileNumber">The profile number (1-3).</param>
        /// <returns>The save file information.</returns>
        public static async Task<SaveFileInfo> GetSaveFileInfoAsync(this IFileSystemService fileSystemService, int profileNumber)
        {
            if (profileNumber < 1 || profileNumber > 3)
                throw new ArgumentException("Profile number must be between 1 and 3", nameof(profileNumber));

            var balatroDir = await fileSystemService.GetBalatroSaveDirectoryAsync();
            var profileDir = Path.Combine(balatroDir, profileNumber.ToString());
            var savePath = Path.Combine(profileDir, "save.jkr");

            if (!File.Exists(savePath))
                return new SaveFileInfo {
                    ProfileNumber = profileNumber,
                    Exists = false
                };

            var fileInfo = new FileInfo(savePath);
            return new SaveFileInfo
            {
                ProfileNumber = profileNumber,
                Exists = true,
                Path = savePath,
                LastModified = fileInfo.LastWriteTime,
                SizeInBytes = fileInfo.Length
            };
        }

        #endregion
    }

    /// <summary>
    /// Information about a save file.
    /// </summary>
    public class SaveFileInfo
    {
        /// <summary>
        /// The profile number (1-3).
        /// </summary>
        public int ProfileNumber { get; set; }

        /// <summary>
        /// Whether the save file exists.
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// The full path to the save file.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// When the save file was last modified.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// The size of the save file in bytes.
        /// </summary>
        public long SizeInBytes { get; set; }
    }
}
