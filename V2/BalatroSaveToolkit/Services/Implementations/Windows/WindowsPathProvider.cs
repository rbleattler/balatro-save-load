using BalatroSaveToolkit.Services.Interfaces;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace BalatroSaveToolkit.Services.Implementations.Windows
{
    /// <summary>
    /// Windows-specific implementation of IPathProvider.
    /// Handles Windows-specific file paths, special folders, and Balatro save locations
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class WindowsPathProvider : BasePathProvider
    {
        // Steam installation paths to check
        private static readonly string[] SteamInstallationPaths = {
            @"C:\Program Files (x86)\Steam",
            @"C:\Program Files\Steam",
            @"D:\Steam",
            @"D:\Program Files (x86)\Steam",
            @"D:\Program Files\Steam",
            @"E:\Steam",
            @"E:\Program Files (x86)\Steam",
            @"E:\Program Files\Steam"
            // Additional paths can be added here
        };

        /// <summary>
        /// Windows file systems are not case-sensitive
        /// </summary>
        public override bool IsCaseSensitive => false;

        /// <summary>
        /// Initializes a new instance of WindowsPathProvider
        /// </summary>
        /// <param name="logService">The logging service</param>
        /// <param name="errorHandler">The error handling service</param>
        public WindowsPathProvider(ILogService logService, IErrorHandlingService errorHandler)
            : base(logService, errorHandler)
        {
            _logService.LogDebug(nameof(WindowsPathProvider), "Initialized Windows path provider");
        }

        /// <summary>
        /// Gets the path to the Balatro save directory on Windows
        /// </summary>
        /// <param name="steamInstallation">If true, attempts to locate Steam installation save files</param>
        /// <returns>Path to the Balatro save directory</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the save directory cannot be found</exception>
        public override async Task<string> GetBalatroSaveDirectoryAsync(bool steamInstallation = false)
        {
            try
            {
                string saveDir;

                if (steamInstallation)
                {
                    // Try to find Steam installation
                    saveDir = await GetSteamBalatroSaveDirectoryAsync();
                    if (!string.IsNullOrEmpty(saveDir) && Directory.Exists(saveDir))
                    {
                        return saveDir;
                    }

                    _logService.LogWarning(nameof(WindowsPathProvider),
                        "Steam Balatro save directory not found, falling back to standard location");
                }

                // Standard location for Windows
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                saveDir = Path.Combine(localAppData, "Balatro");

                // Check if directory exists, if not try to create it
                if (!Directory.Exists(saveDir))
                {
                    _logService.LogInfo(nameof(WindowsPathProvider),
                        $"Balatro save directory not found, creating: {saveDir}");
                    Directory.CreateDirectory(saveDir);
                }

                return saveDir;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(WindowsPathProvider),
                    "Error getting Balatro save directory", ErrorSeverity.Error, false);
                throw new DirectoryNotFoundException("Could not locate Balatro save directory on Windows", ex);
            }
        }

        /// <summary>
        /// Attempt to find the Steam installation path and related Balatro save directory
        /// </summary>
        /// <returns>Path to the Steam Balatro save directory or null if not found</returns>
        private async Task<string?> GetSteamBalatroSaveDirectoryAsync()
        {
            try
            {
                // Try to get Steam installation from registry first
                string? steamPath = await GetSteamPathFromRegistryAsync();

                // If found in registry, check for Balatro save location
                if (!string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath))
                {
                    string saveDir = Path.Combine(steamPath, "userdata");
                    if (Directory.Exists(saveDir))
                    {
                        // Steam userdata structure is: Steam\userdata\[user-id]\[app-id]
                        // Balatro's Steam AppID is 2379780
                        const string balatroAppId = "2379780";

                        // Search for the first user that has Balatro data
                        foreach (string userDir in Directory.GetDirectories(saveDir))
                        {
                            string potentialPath = Path.Combine(userDir, balatroAppId);
                            if (Directory.Exists(potentialPath))
                            {
                                _logService.LogInfo(nameof(WindowsPathProvider),
                                    $"Found Steam Balatro save directory: {potentialPath}");
                                return potentialPath;
                            }
                        }
                    }
                }

                // If not found via registry, check standard installation locations
                foreach (string installPath in SteamInstallationPaths)
                {
                    if (!Directory.Exists(installPath))
                        continue;

                    string saveDir = Path.Combine(installPath, "userdata");
                    if (!Directory.Exists(saveDir))
                        continue;

                    // Check each user ID folder for Balatro save data
                    const string balatroAppId = "2379780";
                    foreach (string userDir in Directory.GetDirectories(saveDir))
                    {
                        string potentialPath = Path.Combine(userDir, balatroAppId);
                        if (Directory.Exists(potentialPath))
                        {
                            _logService.LogInfo(nameof(WindowsPathProvider),
                                $"Found Steam Balatro save directory: {potentialPath}");
                            return potentialPath;
                        }
                    }
                }

                return null; // Not found
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(WindowsPathProvider),
                    "Error finding Steam Balatro save directory", ErrorSeverity.Warning, false);
                return null;
            }
        }

        /// <summary>
        /// Attempt to read the Steam installation path from the Windows registry
        /// </summary>
        /// <returns>Steam installation path or null if not found</returns>
        private Task<string?> GetSteamPathFromRegistryAsync()
        {
            try
            {
                // Try to get Steam installation path from registry
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
                {
                    if (key != null)
                    {
                        string? path = key.GetValue("SteamPath") as string;
                        if (!string.IsNullOrEmpty(path))
                        {
                            // Normalize backslashes to forward slashes for consistency
                            path = path.Replace('/', '\\');
                            _logService.LogDebug(nameof(WindowsPathProvider),
                                $"Found Steam installation path in registry: {path}");
                            return Task.FromResult<string?>(path);
                        }
                    }
                }

                // Try the 64-bit registry location as well
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam"))
                {
                    if (key != null)
                    {
                        string? path = key.GetValue("InstallPath") as string;
                        if (!string.IsNullOrEmpty(path))
                        {
                            path = path.Replace('/', '\\');
                            _logService.LogDebug(nameof(WindowsPathProvider),
                                $"Found Steam installation path in 64-bit registry: {path}");
                            return Task.FromResult<string?>(path);
                        }
                    }
                }

                return Task.FromResult<string?>(null); // Not found
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(WindowsPathProvider),
                    "Error reading Steam path from registry", ErrorSeverity.Information, false);
                return Task.FromResult<string?>(null);
            }
        }

        /// <summary>
        /// Override of GetApplicationDataDirectoryAsync for Windows-specific path
        /// </summary>
        public override async Task<string> GetApplicationDataDirectoryAsync(string? appName = null)
        {
            try
            {
                // Use LocalApplicationData folder on Windows for app data
                string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                // If appName is provided, append it to the path
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
                _errorHandler.HandleException(ex, nameof(WindowsPathProvider),
                    "Error getting Windows application data directory", ErrorSeverity.Error, false);

                // Fall back to base implementation
                return await base.GetApplicationDataDirectoryAsync(appName);
            }
        }

        /// <summary>
        /// Normalizes a Windows file path to ensure consistent format
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>The normalized path</returns>
        public string NormalizeWindowsPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            try
            {
                // Convert all separators to backslashes (Windows standard)
                path = path.Replace('/', '\\');

                // Handle UNC paths (network shares) correctly
                if (path.StartsWith("\\\\"))
                {
                    // Keep UNC path prefix
                    return path;
                }

                // Handle drive letter paths
                if (path.Length >= 2 && path[1] == ':')
                {
                    // Capitalize drive letter for consistency
                    path = char.ToUpper(path[0]) + path.Substring(1);
                }

                // Remove trailing backslash unless it's a drive root (e.g., C:\)
                if (path.EndsWith("\\") && !Regex.IsMatch(path, @"^[A-Z]:\\$"))
                {
                    path = path.TrimEnd('\\');
                }

                // Handle long paths (>260 characters) if needed
                if (path.Length >= 260 && !path.StartsWith(@"\\?\"))
                {
                    path = @"\\?\" + path;
                }

                return path;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(WindowsPathProvider),
                    $"Error normalizing Windows path: {path}", ErrorSeverity.Information, false);
                return path; // Return original path if normalization fails
            }
        }

        /// <summary>
        /// Checks if a path is on a removable drive
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>True if the path is on a removable drive; otherwise, false</returns>
        public bool IsOnRemovableDrive(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || path.Length < 2 || path[1] != ':')
                {
                    return false;
                }

                string driveLetter = path.Substring(0, 1).ToUpper();
                string rootPath = $"{driveLetter}:\\";

                DriveInfo drive = new DriveInfo(driveLetter);
                return drive.DriveType == DriveType.Removable;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(WindowsPathProvider),
                    $"Error checking if path is on removable drive: {path}", ErrorSeverity.Information, false);
                return false;
            }
        }
    }
}
