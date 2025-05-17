using BalatroSaveToolkit.Services.Interfaces;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace BalatroSaveToolkit.Services.Implementations.Linux
{
    /// <summary>
    /// Linux-specific implementation of IPathProvider.
    /// Handles Linux-specific file paths, special folders, and Balatro save locations
    /// </summary>
    [SupportedOSPlatform("linux")]
    public class LinuxPathProvider : BasePathProvider
    {
        // Steam installation paths to check
        private static readonly string[] SteamInstallationPaths = {
            "~/.steam/steam",
            "~/.local/share/Steam",
            "~/.steam/debian-installation",
            "/usr/lib/steam",
            "/opt/steam"
        };

        // Flatpak Steam installation paths
        private static readonly string[] FlatpakSteamPaths = {
            "~/.var/app/com.valvesoftware.Steam/.steam/steam",
            "~/.var/app/com.valvesoftware.Steam/.local/share/Steam"
        };

        /// <summary>
        /// Linux file systems are case-sensitive
        /// </summary>
        public override bool IsCaseSensitive => true;

        // Path to the home directory
        private readonly string _homePath;

        /// <summary>
        /// Path to XDG_DATA_HOME (defaults to ~/.local/share)
        /// </summary>
        private readonly string _xdgDataHome;

        /// <summary>
        /// Path to XDG_CONFIG_HOME (defaults to ~/.config)
        /// </summary>
        private readonly string _xdgConfigHome;

        /// <summary>
        /// Path to XDG_CACHE_HOME (defaults to ~/.cache)
        /// </summary>
        private readonly string _xdgCacheHome;

        /// <summary>
        /// Initializes a new instance of LinuxPathProvider
        /// </summary>
        /// <param name="logService">The logging service</param>
        /// <param name="errorHandler">The error handling service</param>
        public LinuxPathProvider(ILogService logService, IErrorHandlingService errorHandler)
            : base(logService, errorHandler)
        {
            _homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Get XDG paths from environment variables or use defaults
            _xdgDataHome = GetEnvVarOrDefault("XDG_DATA_HOME", Path.Combine(_homePath, ".local", "share"));
            _xdgConfigHome = GetEnvVarOrDefault("XDG_CONFIG_HOME", Path.Combine(_homePath, ".config"));
            _xdgCacheHome = GetEnvVarOrDefault("XDG_CACHE_HOME", Path.Combine(_homePath, ".cache"));

            _logService.LogDebug(nameof(LinuxPathProvider), "Initialized Linux path provider");
            _logService.LogDebug(nameof(LinuxPathProvider), $"Home path: {_homePath}");
            _logService.LogDebug(nameof(LinuxPathProvider), $"XDG_DATA_HOME: {_xdgDataHome}");
            _logService.LogDebug(nameof(LinuxPathProvider), $"XDG_CONFIG_HOME: {_xdgConfigHome}");
            _logService.LogDebug(nameof(LinuxPathProvider), $"XDG_CACHE_HOME: {_xdgCacheHome}");
        }

        /// <summary>
        /// Gets the path to the Balatro save directory on Linux
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

                    _logService.LogWarning(nameof(LinuxPathProvider),
                        "Steam Balatro save directory not found, falling back to standard location");
                }

                // Standard location for Linux: ~/.local/share/Balatro
                saveDir = Path.Combine(_xdgDataHome, "Balatro");

                // Check if directory exists, if not try to create it
                if (!Directory.Exists(saveDir))
                {
                    // Also check for Proton/Wine installations which might create Windows-style paths
                    string protonSaveDir = Path.Combine(_homePath, ".wine", "drive_c", "users",
                        Environment.UserName, "AppData", "Local", "Balatro");

                    if (Directory.Exists(protonSaveDir))
                    {
                        _logService.LogInfo(nameof(LinuxPathProvider),
                            $"Found Proton/Wine Balatro save directory: {protonSaveDir}");
                        return NormalizeLinuxPath(protonSaveDir);
                    }

                    _logService.LogInfo(nameof(LinuxPathProvider),
                        $"Balatro save directory not found, creating: {saveDir}");
                    Directory.CreateDirectory(saveDir);
                }

                return NormalizeLinuxPath(saveDir);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    "Error getting Balatro save directory", ErrorSeverity.Error, false);
                throw new DirectoryNotFoundException("Could not locate Balatro save directory on Linux", ex);
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
                // Check for Steam installation in common locations
                // For actual Steam path, we need to check userdata folders
                string? steamPath = await FindSteamInstallationPathAsync();

                if (!string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath))
                {
                    string userDataDir = Path.Combine(steamPath, "userdata");
                    if (Directory.Exists(userDataDir))
                    {
                        // Steam userdata structure is: Steam/userdata/[user-id]/[app-id]
                        // Balatro's Steam AppID is 2379780
                        const string balatroAppId = "2379780";

                        // Search for the first user that has Balatro data
                        foreach (string userDir in Directory.GetDirectories(userDataDir))
                        {
                            string potentialPath = Path.Combine(userDir, balatroAppId);
                            if (Directory.Exists(potentialPath))
                            {
                                _logService.LogInfo(nameof(LinuxPathProvider),
                                    $"Found Steam Balatro save directory: {potentialPath}");
                                return NormalizeLinuxPath(potentialPath);
                            }
                        }
                    }

                    // Also check for the remote/cloud save directory
                    string remoteSavePath = Path.Combine(steamPath, "userdata");
                    if (Directory.Exists(remoteSavePath))
                    {
                        const string balatroAppId = "2379780";
                        foreach (string userDir in Directory.GetDirectories(remoteSavePath))
                        {
                            string potentialPath = Path.Combine(userDir, balatroAppId, "remote");
                            if (Directory.Exists(potentialPath))
                            {
                                _logService.LogInfo(nameof(LinuxPathProvider),
                                    $"Found Steam Cloud Balatro save directory: {potentialPath}");
                                return NormalizeLinuxPath(potentialPath);
                            }
                        }
                    }

                    // For Proton (Windows games on Linux), check the Proton compatibility folder
                    string protonPrefix = Path.Combine(steamPath, "steamapps", "compatdata", "2379780");
                    if (Directory.Exists(protonPrefix))
                    {
                        string protonSavePath = Path.Combine(protonPrefix, "pfx", "drive_c", "users",
                            "steamuser", "AppData", "Local", "Balatro");

                        if (Directory.Exists(protonSavePath))
                        {
                            _logService.LogInfo(nameof(LinuxPathProvider),
                                $"Found Steam Proton Balatro save directory: {protonSavePath}");
                            return NormalizeLinuxPath(protonSavePath);
                        }
                    }
                }

                // Additional check for Flatpak Steam installations
                foreach (string flatpakPath in FlatpakSteamPaths)
                {
                    string path = ResolveLinuxPath(flatpakPath);

                    if (Directory.Exists(path))
                    {
                        string userDataDir = Path.Combine(path, "userdata");
                        if (Directory.Exists(userDataDir))
                        {
                            const string balatroAppId = "2379780";
                            foreach (string userDir in Directory.GetDirectories(userDataDir))
                            {
                                string potentialPath = Path.Combine(userDir, balatroAppId);
                                if (Directory.Exists(potentialPath))
                                {
                                    _logService.LogInfo(nameof(LinuxPathProvider),
                                        $"Found Flatpak Steam Balatro save directory: {potentialPath}");
                                    return NormalizeLinuxPath(potentialPath);
                                }
                            }
                        }
                    }
                }

                return null; // Not found
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    "Error finding Steam Balatro save directory", ErrorSeverity.Warning, false);
                return null;
            }
        }

        /// <summary>
        /// Find Steam installation path on Linux
        /// </summary>
        private Task<string?> FindSteamInstallationPathAsync()
        {
            try
            {
                // Check common Steam installation locations on Linux
                foreach (string pathPattern in SteamInstallationPaths)
                {
                    string path = ResolveLinuxPath(pathPattern);

                    // If path exists and looks like a Steam directory
                    if (Directory.Exists(path) && IsSteamDirectory(path))
                    {
                        _logService.LogDebug(nameof(LinuxPathProvider),
                            $"Found Steam installation at: {path}");
                        return Task.FromResult<string?>(path);
                    }
                }

                // For systems using Flatpak
                foreach (string flatpakPath in FlatpakSteamPaths)
                {
                    string path = ResolveLinuxPath(flatpakPath);
                    if (Directory.Exists(path) && IsSteamDirectory(path))
                    {
                        _logService.LogDebug(nameof(LinuxPathProvider),
                            $"Found Flatpak Steam installation at: {path}");
                        return Task.FromResult<string?>(path);
                    }
                }

                return Task.FromResult<string?>(null);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    "Error searching for Steam installation", ErrorSeverity.Debug, false);
                return Task.FromResult<string?>(null);
            }
        }

        /// <summary>
        /// Check if a directory is a valid Steam installation
        /// </summary>
        private bool IsSteamDirectory(string directory)
        {
            try
            {
                // Check for key indicators that this is a Steam directory
                bool hasSteamApps = Directory.Exists(Path.Combine(directory, "steamapps")) ||
                                   Directory.Exists(Path.Combine(directory, "SteamApps"));

                bool hasUserData = Directory.Exists(Path.Combine(directory, "userdata"));

                bool hasSteamExecutable = File.Exists(Path.Combine(directory, "steam")) ||
                                         File.Exists(Path.Combine(directory, "steam.sh"));

                // If any two of these are true, it's likely a Steam directory
                return (hasSteamApps && hasUserData) ||
                       (hasSteamApps && hasSteamExecutable) ||
                       (hasUserData && hasSteamExecutable);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    $"Error checking if directory is a Steam installation: {directory}", ErrorSeverity.Debug, false);
                return false;
            }
        }

        /// <summary>
        /// Override of GetApplicationDataDirectoryAsync for Linux-specific path
        /// </summary>
        public override async Task<string> GetApplicationDataDirectoryAsync(string? appName = null)
        {
            try
            {
                // Linux convention following XDG spec: ~/.local/share/{AppName}
                string baseDir = _xdgDataHome;

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

                return NormalizeLinuxPath(baseDir);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    "Error getting Linux application data directory", ErrorSeverity.Error, false);

                // Fall back to base implementation
                return await base.GetApplicationDataDirectoryAsync(appName);
            }
        }

        /// <summary>
        /// Gets the path to the user's documents directory.
        /// Linux specific implementation using XDG directories.
        /// </summary>
        public override Task<string> GetDocumentsDirectoryAsync()
        {
            try
            {
                // Try to use XDG_DOCUMENTS_DIR from user-dirs.dirs if it exists
                string userDirsConfig = Path.Combine(_xdgConfigHome, "user-dirs.dirs");
                if (File.Exists(userDirsConfig))
                {
                    string[] lines = File.ReadAllLines(userDirsConfig);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("XDG_DOCUMENTS_DIR="))
                        {
                            string path = line.Split('=')[1].Trim('"');
                            // Replace $HOME with actual home path
                            path = path.Replace("$HOME", _homePath);
                            if (Directory.Exists(path))
                            {
                                return Task.FromResult(NormalizeLinuxPath(path));
                            }
                        }
                    }
                }

                // Fallback to standard Documents folder
                string documentsDir = Path.Combine(_homePath, "Documents");
                if (!Directory.Exists(documentsDir))
                {
                    // Create it if it doesn't exist
                    Directory.CreateDirectory(documentsDir);
                }

                return Task.FromResult(NormalizeLinuxPath(documentsDir));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    "Error getting Documents directory", ErrorSeverity.Warning, false);
                return base.GetDocumentsDirectoryAsync();
            }
        }

        /// <summary>
        /// Gets the path to the user's desktop directory.
        /// Linux specific implementation using XDG directories.
        /// </summary>
        public override Task<string> GetDesktopDirectoryAsync()
        {
            try
            {
                // Try to use XDG_DESKTOP_DIR from user-dirs.dirs if it exists
                string userDirsConfig = Path.Combine(_xdgConfigHome, "user-dirs.dirs");
                if (File.Exists(userDirsConfig))
                {
                    string[] lines = File.ReadAllLines(userDirsConfig);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("XDG_DESKTOP_DIR="))
                        {
                            string path = line.Split('=')[1].Trim('"');
                            // Replace $HOME with actual home path
                            path = path.Replace("$HOME", _homePath);
                            if (Directory.Exists(path))
                            {
                                return Task.FromResult(NormalizeLinuxPath(path));
                            }
                        }
                    }
                }

                // Fallback to standard Desktop folder
                string desktopDir = Path.Combine(_homePath, "Desktop");
                if (!Directory.Exists(desktopDir))
                {
                    // Create it if it doesn't exist
                    Directory.CreateDirectory(desktopDir);
                }

                return Task.FromResult(NormalizeLinuxPath(desktopDir));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    "Error getting Desktop directory", ErrorSeverity.Warning, false);
                return base.GetDesktopDirectoryAsync();
            }
        }

        /// <summary>
        /// Override to use XDG cache directory for temp files
        /// </summary>
        public override Task<string> GetTempDirectoryAsync()
        {
            try
            {
                string tempDir = Path.Combine(_xdgCacheHome, AppName);
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                return Task.FromResult(NormalizeLinuxPath(tempDir));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    "Error getting temp directory", ErrorSeverity.Warning, false);
                return base.GetTempDirectoryAsync();
            }
        }

        /// <summary>
        /// Resolves Linux paths with tilde notation for the home directory
        /// </summary>
        /// <param name="path">Path potentially containing tilde</param>
        /// <returns>Resolved path</returns>
        public string ResolveLinuxPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            // Replace ~ with the home directory path
            if (path.StartsWith("~"))
            {
                return path.Replace("~", _homePath);
            }

            return path;
        }

        /// <summary>
        /// Gets environment variable value or returns default if not set
        /// </summary>
        private string GetEnvVarOrDefault(string varName, string defaultValue)
        {
            string? value = Environment.GetEnvironmentVariable(varName);
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>
        /// Normalizes a Linux file path to ensure consistent format
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>The normalized path</returns>
        public string NormalizeLinuxPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            try
            {
                // Convert all separators to forward slashes (Linux standard)
                path = path.Replace('\\', '/');

                // Resolve any symlinks to their real paths
                // Note: This requires platform-specific code that might not be fully
                // available in .NET MAUI, so we'll do basic normalization

                // Remove consecutive slashes
                path = Regex.Replace(path, "//+", "/");

                // Remove trailing slash unless it's the root directory
                if (path.EndsWith("/") && path != "/")
                {
                    path = path.TrimEnd('/');
                }

                return path;
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    $"Error normalizing Linux path: {path}", ErrorSeverity.Debug, false);
                return path; // Return original path if normalization fails
            }
        }

        /// <summary>
        /// Checks if a path is a symbolic link
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the path is a symbolic link</returns>
        public bool IsSymbolicLink(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return false;

                FileInfo fileInfo = new FileInfo(path);
                return fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(LinuxPathProvider),
                    $"Error checking if path is a symbolic link: {path}", ErrorSeverity.Debug, false);
                return false;
            }
        }
    }
}
