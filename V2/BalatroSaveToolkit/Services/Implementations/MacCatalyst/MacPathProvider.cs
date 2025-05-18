using BalatroSaveToolkit.Services.Interfaces;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;

namespace BalatroSaveToolkit.Services.Implementations.MacCatalyst
{
    /// <summary>
    /// macOS-specific implementation of IPathProvider.
    /// Handles macOS-specific file paths, special folders, and Balatro save locations
    /// </summary>
    [SupportedOSPlatform("maccatalyst")]
    public class MacPathProvider : BasePathProvider
    {
        // Steam installation paths to check
        private static readonly string[] SteamInstallationPaths = {
            "~/Library/Application Support/Steam",
            "/Applications/Steam.app",
            "/Volumes/*/Steam",
            "/Users/*/Library/Application Support/Steam"
        };

        /// <summary>
        /// macOS file systems are case-sensitive by default on APFS
        /// but can be configured to be case-insensitive
        /// </summary>
        public override bool IsCaseSensitive => true;

        // Path to the macOS Library folder
        private readonly string _libraryPath;

        // Path to the user home directory
        private readonly string _homePath;

        /// <summary>
        /// Initializes a new instance of MacPathProvider
        /// </summary>
        /// <param name="logService">The logging service</param>
        /// <param name="errorHandler">The error handling service</param>
        public MacPathProvider(ILogService logService, IErrorHandlingService errorHandler)
            : base(logService, errorHandler)
        {
            _homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _libraryPath = Path.Combine(_homePath, "Library");

            _logService.LogDebug(nameof(MacPathProvider), "Initialized macOS path provider");
            _logService.LogDebug(nameof(MacPathProvider), $"Home path: {_homePath}");
            _logService.LogDebug(nameof(MacPathProvider), $"Library path: {_libraryPath}");
        }

        /// <summary>
        /// Gets the path to the Balatro save directory on macOS
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

                    _logService.LogWarning(nameof(MacPathProvider),
                        "Steam Balatro save directory not found, falling back to standard location");
                }

                // Standard location for macOS: ~/Library/Application Support/Balatro
                string applicationSupport = Path.Combine(_libraryPath, "Application Support");
                saveDir = Path.Combine(applicationSupport, "Balatro");

                // Check if directory exists, if not try to create it
                if (!Directory.Exists(saveDir))
                {
                    _logService.LogInfo(nameof(MacPathProvider),
                        $"Balatro save directory not found, creating: {saveDir}");
                    Directory.CreateDirectory(saveDir);
                }

                return NormalizeMacPath(saveDir);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacPathProvider),
                    "Error getting Balatro save directory", ErrorSeverity.Error, false);
                throw new DirectoryNotFoundException("Could not locate Balatro save directory on macOS", ex);
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
                                _logService.LogInfo(nameof(MacPathProvider),
                                    $"Found Steam Balatro save directory: {potentialPath}");
                                return NormalizeMacPath(potentialPath);
                            }
                        }
                    }

                    // Also check for Steam's cloud sync folder for Balatro
                    string steamCloudPath = Path.Combine(steamPath, "Steam", "userdata");
                    if (Directory.Exists(steamCloudPath))
                    {
                        const string balatroAppId = "2379780";
                        foreach (string userDir in Directory.GetDirectories(steamCloudPath))
                        {
                            string potentialPath = Path.Combine(userDir, balatroAppId, "remote");
                            if (Directory.Exists(potentialPath))
                            {
                                _logService.LogInfo(nameof(MacPathProvider),
                                    $"Found Steam Cloud Balatro save directory: {potentialPath}");
                                return NormalizeMacPath(potentialPath);
                            }
                        }
                    }
                }

                return null; // Not found
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacPathProvider),
                    "Error finding Steam Balatro save directory", ErrorSeverity.Warning, false);
                return null;
            }
        }

        /// <summary>
        /// Find Steam installation path on macOS
        /// </summary>
        private Task<string?> FindSteamInstallationPathAsync()
        {
            try
            {
                // Check common Steam installation locations on macOS
                foreach (string pathPattern in SteamInstallationPaths)
                {
                    string path = ResolveMacOSPath(pathPattern);

                    // Handle wildcard paths
                    if (path.Contains("*"))
                    {
                        // Get the directory before the wildcard
                        string directoryPart = Path.GetDirectoryName(path) ?? string.Empty;
                        string searchPattern = Path.GetFileName(path);

                        // If the directory exists, search for matching subdirectories
                        if (!string.IsNullOrEmpty(directoryPart) && Directory.Exists(directoryPart))
                        {
                            foreach (string dir in Directory.GetDirectories(directoryPart, searchPattern))
                            {
                                if (IsSteamDirectory(dir))
                                {
                                    _logService.LogDebug(nameof(MacPathProvider),
                                        $"Found Steam installation at: {dir}");
                                    return Task.FromResult<string?>(dir);
                                }
                            }
                        }
                    }
                    // Handle direct paths
                    else if (Directory.Exists(path) && IsSteamDirectory(path))
                    {
                        _logService.LogDebug(nameof(MacPathProvider),
                            $"Found Steam installation at: {path}");
                        return Task.FromResult<string?>(path);
                    }
                }

                return Task.FromResult<string?>(null);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacPathProvider),
                    "Error searching for Steam installation", ErrorSeverity.Information, false);
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
                                         File.Exists(Path.Combine(directory, "Steam")) ||
                                         File.Exists(Path.Combine(directory, "steam.sh")) ||
                                         Directory.Exists(Path.Combine(directory, "Steam.app"));

                // If any two of these are true, it's likely a Steam directory
                return (hasSteamApps && hasUserData) ||
                       (hasSteamApps && hasSteamExecutable) ||
                       (hasUserData && hasSteamExecutable);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacPathProvider),
                    $"Error checking if directory is a Steam installation: {directory}", ErrorSeverity.Information, false);
                return false;
            }
        }

        /// <summary>
        /// Override of GetApplicationDataDirectoryAsync for macOS-specific path
        /// </summary>
        public override async Task<string> GetApplicationDataDirectoryAsync(string? appName = null)
        {
            try
            {
                // macOS convention: ~/Library/Application Support/{AppName}
                string applicationSupport = Path.Combine(_libraryPath, "Application Support");
                string baseDir = applicationSupport;

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

                return NormalizeMacPath(baseDir);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacPathProvider),
                    "Error getting macOS application data directory", ErrorSeverity.Error, false);

                // Fall back to base implementation
                return await base.GetApplicationDataDirectoryAsync(appName);
            }
        }

        /// <summary>
        /// Gets the path to the user's documents directory.
        /// macOS specific implementation.
        /// </summary>
        public override Task<string> GetDocumentsDirectoryAsync()
        {
            try
            {
                string documentsDir = Path.Combine(_homePath, "Documents");
                return Task.FromResult(NormalizeMacPath(documentsDir));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacPathProvider),
                    "Error getting Documents directory", ErrorSeverity.Warning, false);
                return base.GetDocumentsDirectoryAsync();
            }
        }

        /// <summary>
        /// Gets the path to the user's desktop directory.
        /// macOS specific implementation.
        /// </summary>
        public override Task<string> GetDesktopDirectoryAsync()
        {
            try
            {
                string desktopDir = Path.Combine(_homePath, "Desktop");
                return Task.FromResult(NormalizeMacPath(desktopDir));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacPathProvider),
                    "Error getting Desktop directory", ErrorSeverity.Warning, false);
                return base.GetDesktopDirectoryAsync();
            }
        }

        /// <summary>
        /// Resolves macOS paths with tilde notation for the home directory
        /// </summary>
        /// <param name="path">Path potentially containing tilde</param>
        /// <returns>Resolved path</returns>
        public string ResolveMacOSPath(string path)
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
        /// Normalizes a macOS file path to ensure consistent format and Unicode normalization
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>The normalized path</returns>
        public string NormalizeMacPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            try
            {
                // Convert all separators to forward slashes (macOS standard)
                path = path.Replace('\\', '/');

                // Remove trailing slash unless it's a root directory (e.g., /)
                if (path.EndsWith("/") && path != "/")
                {
                    path = path.TrimEnd('/');
                }

                // macOS uses NFD Unicode normalization for file names
                // Convert to NFD for consistent handling
                return NormalizeToNfd(path);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacPathProvider),
                    $"Error normalizing macOS path: {path}", ErrorSeverity.Information, false);
                return path; // Return original path if normalization fails
            }
        }

        /// <summary>
        /// Normalizes a string to NFD (Normalization Form D) as used by macOS file system
        /// </summary>
        /// <param name="input">String to normalize</param>
        /// <returns>NFD normalized string</returns>
        private string NormalizeToNfd(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Apply Unicode normalization to match macOS file system behavior
            return input.Normalize(NormalizationForm.FormD);
        }

        /// <summary>
        /// Checks if the path has macOS resource fork or extended attributes
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the file has resource fork or extended attributes</returns>
        public bool HasResourceFork(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return false;

            try
            {
                // Check for resource fork file
                string resourceForkPath = Path.Combine(
                    Path.GetDirectoryName(path) ?? string.Empty,
                    "._" + Path.GetFileName(path));

                return File.Exists(resourceForkPath);
            }
            catch (Exception ex)
            {
                _errorHandler.HandleException(ex, nameof(MacPathProvider),
                    $"Error checking resource fork: {path}", ErrorSeverity.Information, false);
                return false;
            }
        }
    }
}
